using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using LibreLancer.GameData.Items;
using LibreLancer.Net;
using LibreLancer.Net.Protocol;
using LibreLancer.World;
using LibreLancer.World.Components;

namespace LibreLancer.Server.Components
{
    public record struct FetchedDelta(int Priority, uint Tick, ObjectUpdate Update);
    /*
     * Component that handles a remote player controlling this GameObject
     * Stores a reference to the Player class, buffers input
     * and keeps track of cargo
     */
    public class SPlayerComponent : AbstractCargoComponent
    {
        private PriorityQueue<NetInputControls, uint> inputs = new();

        record SavedTick(uint Tick, PlayerAuthState Player, Dictionary<int, ObjectUpdate> Updates);

        //Used for compressing delta info
        private CircularBuffer<SavedTick> oldStates = new(64);

        public UpdateAck MostRecentAck = default;
        public Player Player { get; private set; }

        public GameObject SelectedObject { get; private set; }

        public SPlayerComponent(Player player, GameObject parent) : base(parent)
        {
            this.Player = player;
        }

        public void Killed()
        {
            Player.Killed();
        }

        private Dictionary<int, int> priorities = new();
        private BitArray found = new BitArray(512);

        public void GetUpdates(GameObject[] objs, FetchedDelta[] deltas)
        {
            if(objs.Length > found.Length)
                found = new BitArray(found.Length);
            found.SetAll(false);
            int foundCount = 0;
            if (deltas.Length != objs.Length) {
                throw new InvalidOperationException("Bad number of updates");
            }
            for (int i = 0; i < deltas.Length; i++) {
                deltas[i] = new FetchedDelta(100, 0, ObjectUpdate.Blank);
                if (objs[i] == Parent) {
                    found[i] = true;
                    foundCount++;
                }
            }
            for (int i = 0; i < oldStates.Count; i++)
            {
                if (!MostRecentAck[oldStates[i].Tick])
                    continue;
                for (int j = 0; j < objs.Length; j++)
                {
                    if (found[j]) continue;
                    if (oldStates[i].Updates.TryGetValue(objs[j].Unique, out var upd))
                    {
                        priorities.TryGetValue(objs[j].Unique, out var priority);
                        deltas[j] = new FetchedDelta(priority, oldStates[i].Tick, upd);
                        found[j] = true;
                        foundCount++;
                        if (foundCount == objs.Length)
                        {
                            priorities.Clear();
                            return;
                        }
                    }
                }
            }
            priorities.Clear();
        }

        public void SetPriority(GameObject obj, int priority)
        {
            priorities[obj.Unique] = priority;
        }

        public void GetAcknowledgedState(out uint ackTick, out PlayerAuthState authState)
        {
            ackTick = 0;
            authState = new PlayerAuthState();
            if (MostRecentAck.Tick == 0) return;
            for (int i = 0; i < oldStates.Count; i++) {
                if (oldStates[i].Tick == MostRecentAck.Tick)
                {
                    ackTick = MostRecentAck.Tick;
                    authState = oldStates[i].Player;
                    break;
                }
            }
        }

        public void EnqueueState(uint tick, PlayerAuthState auth, Dictionary<int, ObjectUpdate> updates)
        {
            oldStates.Enqueue(new SavedTick(tick, auth, updates));
        }

        public uint LatestReceived;

        public void QueueInput(InputUpdatePacket input)
        {
            MostRecentAck = input.Acks;
            //Select object immediately
            SelectedObject = Parent.World.GetObject( input.SelectedObject);
            Enqueue(input.HistoryC);
            Enqueue(input.HistoryB);
            Enqueue(input.HistoryA);
            Enqueue(input.Current);
            LatestReceived = input.Current.Tick;
        }

        void Enqueue(NetInputControls controls)
        {
            if (controls.Tick == 0)
                return;
            if(controls.Tick >= GetCurrentTick())
                inputs.Enqueue(controls, controls.Tick);
        }

        uint GetCurrentTick() => Parent.World.Server.CurrentTick;


        bool GetInput(out NetInputControls packet)
        {
            NetInputControls currentTick = default;
            bool found = false;
            var tick = GetCurrentTick();
            while (inputs.TryDequeue(out var entry, out _)) {
                if (entry.Tick == tick) {
                    currentTick = entry;
                    found = true;
                } else if (entry.Tick > tick) {
                    inputs.Enqueue(entry, entry.Tick);
                    break;
                }
            }
            packet = currentTick;
            return found;
        }

        private ulong formationHash = 0;
        public override void Update(double time)
        {
            if (Parent.TryGetComponent<ShipPhysicsComponent>(out var phys))
            {
                var wpc = Parent.GetComponent<WeaponControlComponent>();
                if (GetInput(out var input))
                {
                    wpc.AimPoint = input.AimPoint;
                    if (Player.InTradelane)
                    {
                        phys.Steering = Vector3.Zero;
                        phys.CurrentStrafe = StrafeControls.None;
                        phys.EnginePower = 0;
                        phys.ThrustEnabled = false;
                        phys.CruiseEnabled = false;
                    }
                    else
                    {
                        phys.Steering = input.Steering;
                        phys.CurrentStrafe = input.Strafe;
                        phys.EnginePower = input.Throttle;
                        phys.ThrustEnabled = input.Thrust;
                        phys.CruiseEnabled = input.Cruise;
                        if(input.FireCommand != null)
                            Parent.GetWorld().Server.FireProjectiles(input.FireCommand.Value, Player);
                    }
                }
            }
            if (Parent.Formation == null)
            {
                if(formationHash != 0)
                    Player.RpcClient.UpdateFormation(new NetFormation());
                formationHash = 0;
            }
            else if (formationHash != Parent.Formation.Hash)
            {
                formationHash = Parent.Formation.Hash;
                Player.RpcClient.UpdateFormation(Parent.Formation.ToNetFormation(Parent));
            }
        }

        public override int TryConsume(Equipment item, int maxCount = 1)
        {
            var slot = Player.Character.Items.FirstOrDefault(x => x.Equipment == item);
            if (slot != null)
            {
                var c = slot.Count;
                if(slot.Count <= maxCount)
                    Player.RpcClient.DeleteSlot(slot.ID);
                else
                    Player.RpcClient.UpdateSlotCount(slot.ID, slot.Count - maxCount);
                using var t = Player.Character.BeginTransaction();
                t.RemoveCargo(slot, 1);
                return c > maxCount ? maxCount : c;
            }
            return 0;
        }

        public override T FirstOf<T>()
        {
            var slot = Player.Character.Items.FirstOrDefault(x => x.Equipment is T);
            return (T)slot?.Equipment;
        }
    }
}
