﻿using System.Linq;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActSpawnSolar : BlueprintNode
{
    protected override string Name => "Spawn Solar";

    public readonly Act_SpawnSolar Data;
    public NodeActSpawnSolar(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new() : new Act_SpawnSolar(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
        var solars = missionIni.Solars.Select(x => x.Nickname).ToArray();

        nodePopups.StringCombo("Solar", Data.Solar, s => Data.Solar = s, solars);
    }
}
