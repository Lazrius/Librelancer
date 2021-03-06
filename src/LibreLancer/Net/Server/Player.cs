﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using LibreLancer.GameData.Items;
using LiteNetLib;

namespace LibreLancer
{
    public class Player
    {
        public IPacketClient Client;
        GameServer game;
        Guid playerGuid;
        public NetCharacter Character;
        public PlayerAccount Account;
        public string Name = "Player";
        public string System;
        public string Base;
        public Vector3 Position;
        public Quaternion Orientation;
        public ServerWorld World;
        private MissionRuntime msnRuntime;
        public int ID = 0;
        static int _gid = 0;

        public Player(IPacketClient client, GameServer game, Guid playerGuid)
        {
            this.Client = client;
            this.game = game;
            this.playerGuid = playerGuid;
            ID = Interlocked.Increment(ref _gid);
        }

        public void UpdateMissionRuntime(TimeSpan elapsed)
        {
            msnRuntime?.Update(elapsed);
            if (World != null)
            {
                while (worldActions.Count > 0)
                    worldActions.Dequeue()();
            }
        }

        List<string> rtcs = new List<string>();
        public void AddRTC(string rtc)
        {
            lock (rtcs)
            {
                rtcs.Add(rtc);
                Client.SendPacket(new UpdateRTCPacket() { RTCs = rtcs.ToArray()}, PacketDeliveryMethod.ReliableOrdered);
            }
        }

        public void RemoveRTC(string rtc)
        {
            lock (rtcs)
            {
                rtcs.Remove(rtc);
                Client.SendPacket(new UpdateRTCPacket() { RTCs = rtcs.ToArray()}, PacketDeliveryMethod.ReliableOrdered);
            }
        }
        
        public void OpenSaveGame(Data.Save.SaveGame sg)
        {
            Orientation = Quaternion.Identity;
            Position = sg.Player.Position;
            Base = sg.Player.Base;
            System = sg.Player.System;
            Character = new NetCharacter();
            Character.Credits = sg.Player.Money;
            string ps;
            if (sg.Player.ShipArchetype != null)
                ps = sg.Player.ShipArchetype;
            else
                ps = game.GameData.GetShip(sg.Player.ShipArchetypeCrc).Nickname;
            Character.Ship = game.GameData.GetShip(ps);
            Character.Equipment = new List<NetEquipment>();
            foreach (var eq in sg.Player.Equip)
            {
                var hp = eq.Hardpoint;
                Equipment equip;
                if (eq.EquipName != null) equip = game.GameData.GetEquipment(eq.EquipName);
                else equip = game.GameData.GetEquipment(eq.EquipHash);
                if (equip != null)
                {
                    Character.Equipment.Add(new NetEquipment()
                    {
                        Equipment = equip, Hardpoint = hp, Health = 1
                    });
                }
            }
            if (Base != null)
            {
                lock (rtcs)
                {
                    Client.SendPacket(new BaseEnterPacket()
                    {
                        Base = Base,
                        Ship = Character.EncodeLoadout(),
                        RTCs = rtcs.ToArray()
                    }, PacketDeliveryMethod.ReliableOrdered);
                }
            }
            else
            {
                var sys = game.GameData.GetSystem(System);
                game.RequestWorld(sys, (world) =>
                {
                    World = world; 
                    Client.SendPacket(new SpawnPlayerPacket()
                    {
                        System = System,
                        Position = Position,
                        Orientation = Orientation,
                        Ship = Character.EncodeLoadout()
                    }, PacketDeliveryMethod.ReliableOrdered);
                    world.SpawnPlayer(this, Position, Orientation);
                });
            }
            var missionNum = sg.StoryInfo?.MissionNum ?? 0;
            if (game.GameData.Ini.ContentDll.AlwaysMission13) missionNum = 14;
            if (missionNum != 0 && (missionNum - 1) < game.GameData.Ini.Missions.Count)
            {
                msnRuntime = new MissionRuntime(game.GameData.Ini.Missions[missionNum - 1], this);
                msnRuntime.Update(TimeSpan.Zero);
            }
        }

        private Queue<Action> worldActions = new Queue<Action>();
        public void WorldAction(Action a)
        {
            worldActions.Enqueue(a);
        }

        public void DoAuthSuccess()
        {
            try
            {
                FLLog.Info("Server", "Account logged in");
                Client.SendPacket(new LoginSuccessPacket(), PacketDeliveryMethod.ReliableOrdered);
                Account = new PlayerAccount();
                Client.SendPacket(new OpenCharacterListPacket()
                {
                    Info = new CharacterSelectInfo()
                    {
                        ServerName = game.ServerName,
                        ServerDescription = game.ServerDescription,
                        ServerNews = game.ServerNews,
                        Characters = new List<SelectableCharacter>()
                    }
                }, PacketDeliveryMethod.ReliableOrdered);
            }
            catch (Exception ex)
            {
                FLLog.Error("Player",ex.Message);
                FLLog.Error("Player",
                    ex.StackTrace);
            }
        }

        public void SendUpdate(ObjectUpdatePacket update)
        {
            Client.SendPacket(update, PacketDeliveryMethod.SequenceA);
        }

        public void SpawnPlayer(Player p)
        {
            Client.SendPacket(new SpawnObjectPacket()
            {
                ID = p.ID,
                Name = p.Name,
                Position = p.Position,
                Orientation = p.Orientation,
                Loadout = p.Character.EncodeLoadout()
            }, PacketDeliveryMethod.ReliableOrdered);
        }

        public void SendSolars(Dictionary<string, GameObject> solars)
        {
            var pkt = new SpawnSolarPacket() {Solars = new List<SolarInfo>()};
            foreach (var solar in solars)
            {
                var tr = solar.Value.GetTransform();
                pkt.Solars.Add(new SolarInfo()
                {
                    ID = solar.Value.NetID,
                    Archetype = solar.Value.ArchetypeName,
                    Position = Vector3.Transform(Vector3.Zero, tr),
                    Orientation = tr.ExtractRotation()
                });
            }
            Client.SendPacket(pkt, PacketDeliveryMethod.ReliableOrdered);
        }
        
        public void SendDestroyPart(int id, string part)
        {
            Client.SendPacket(new DestroyPartPacket()
            {
                ID = id,
                PartName = part
            }, PacketDeliveryMethod.ReliableOrdered);
        }
        
        public void ProcessPacket(IPacket packet)
        {
            switch(packet)
            {
                case CharacterListActionPacket c:
                    ListAction(c);
                    break;
                case LaunchPacket l:
                    Launch();
                    break;
                case EnterLocationPacket lc:
                    msnRuntime?.EnterLocation(lc.Room, lc.Base);
                    break;
                case PositionUpdatePacket p:
                    World.PositionUpdate(this, p.Position, p.Orientation);
                    break;
                case RTCCompletePacket cp:
                    RemoveRTC(cp.RTC);
                    break;
                case LineSpokenPacket lp:
                    msnRuntime?.LineFinished(lp.Hash);
                    break;
                case ConsoleCommandPacket cmd:
                    HandleConsoleCommand(cmd.Command);
                    break;
            }
        }

        public void HandleConsoleCommand(string cmd)
        {
            if (cmd.StartsWith("base", StringComparison.OrdinalIgnoreCase)) {
                ForceLand(cmd.Substring(4).Trim());
            }
        }
        void ListAction(CharacterListActionPacket pkt)
        {
            switch(pkt.Action)
            {
                case CharacterListAction.RequestCharacterDB:
                {
                    Client.SendPacket(new NewCharacterDBPacket()
                    {
                        Factions = game.GameData.Ini.NewCharDB.Factions,
                        Packages = game.GameData.Ini.NewCharDB.Packages,
                        Pilots = game.GameData.Ini.NewCharDB.Pilots
                    }, PacketDeliveryMethod.ReliableOrdered);
                    break;
                }
                case CharacterListAction.SelectCharacter:
                {
                    var sc = Account.Characters[pkt.IntArg];
                    Character = NetCharacter.FromDb(sc, game.GameData);
                    Base = Character.Base;
                    Client.SendPacket(new BaseEnterPacket()
                    {
                        Base = Character.Base,
                        Ship = Character.EncodeLoadout()
                    }, PacketDeliveryMethod.ReliableOrdered);
                    break;
                }
                case CharacterListAction.CreateNewCharacter:
                {
                    var ac = new ServerCharacter()
                    {
                        Name = pkt.StringArg,
                        Base = "li01_01_base",
                        Credits = 2000,
                        ID = 0,
                        Ship = "ge_fighter"
                    };
                    ac.Equipment.Add(new ServerEquipment()
                    {
                        Equipment = "ge_gf1_engine_01",
                        Hardpoint = "", Health = 1
                    });
                    ac.Equipment.Add(new ServerEquipment()
                        {
                            Equipment = "ge_fighter_power01",
                            Hardpoint = "", Health = 1
                        }
                    );
                    Account.Characters.Add(ac);
                    Client.SendPacket(new AddCharacterPacket()
                    {
                        Character = NetCharacter.FromDb(ac, game.GameData).ToSelectable()
                    }, PacketDeliveryMethod.ReliableOrdered);
                    break;
            }
            }
        }

        public void SpawnDebris(int id, string archetype, string part, Matrix4x4 tr, float mass)
        {
            Client.SendPacket(new SpawnDebrisPacket()
            {
                ID = id,
                Archetype =  archetype,
                Part = part,
                Mass = mass,
                Orientation =  tr.ExtractRotation(),
                Position = Vector3.Transform(Vector3.Zero, tr)
            }, PacketDeliveryMethod.ReliableOrdered);
        }
        public void ForceLand(string target)
        {
            World?.RemovePlayer(this);
            World = null;
            Base = target;
            Client.SendPacket(new BaseEnterPacket()
            {
                Base = Base,
                Ship = Character.EncodeLoadout(),
                RTCs = rtcs.ToArray()
            }, PacketDeliveryMethod.ReliableOrdered);
        }
        
        public void Despawn(int objId)
        {
            Client.SendPacket(new DespawnObjectPacket() { ID = objId }, PacketDeliveryMethod.ReliableOrdered);
        }
        
        public void Disconnected()
        {
            World?.RemovePlayer(this);
        }
        
        public void PlaySound(string sound)
        {
            Client.SendPacket(new PlaySoundPacket() { Sound = sound }, PacketDeliveryMethod.ReliableOrdered);
        }

        public void PlayMusic(string music)
        {
            Client.SendPacket(new PlayMusicPacket() {Music = music }, PacketDeliveryMethod.ReliableOrdered);
        }

        public void PlayDialog(NetDlgLine[] dialog)
        {
            Client.SendPacket(new MsnDialogPacket() { Lines = dialog }, PacketDeliveryMethod.ReliableOrdered);
        }
        public void CallThorn(string thorn)
        {
            Client.SendPacket(new CallThornPacket() { Thorn = thorn }, PacketDeliveryMethod.ReliableOrdered);
        }
        
        void Launch()
        {
            var b = game.GameData.GetBase(Base);
            var sys = game.GameData.GetSystem(b.System);
            game.RequestWorld(sys, (world) =>
            {
                this.World = world;
                var obj = sys.Objects.FirstOrDefault((o) =>
                {
                    return (o.Dock != null &&
                            o.Dock.Kind == DockKinds.Base &&
                            o.Dock.Target.Equals(Base, StringComparison.OrdinalIgnoreCase));
                });
                System = b.System;
                Orientation = Quaternion.Identity;
                Position = Vector3.Zero;
                if (obj == null)
                {
                    FLLog.Error("Base", "Can't find object in " + sys + " docking to " + b);
                }
                else
                {
                    Position = obj.Position;
                    Orientation = (obj.Rotation ?? Matrix4x4.Identity).ExtractRotation();
                    Position = Vector3.Transform(new Vector3(0, 0, 500), Orientation) + obj.Position; //TODO: This is bad
                }
                Client.SendPacket(new SpawnPlayerPacket()
                {
                    System = System,
                    Position = Position,
                    Orientation = Orientation,
                    Ship = Character.EncodeLoadout()
                }, PacketDeliveryMethod.ReliableOrdered);
                world.SpawnPlayer(this, Position, Orientation);
                msnRuntime?.EnteredSpace();
            });
        }
    }
}