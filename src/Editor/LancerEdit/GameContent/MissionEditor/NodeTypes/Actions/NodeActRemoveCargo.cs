﻿using System.Linq;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActRemoveCargo : BlueprintNode
{
    protected override string Name => "Remove Cargo";

    public readonly Act_RemoveCargo Data;
    public NodeActRemoveCargo(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new() : new Act_RemoveCargo(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
        var objects = gameData.GameData.Goods.Select(x => x.Nickname).ToArray();
        nodePopups.StringCombo("Cargo", Data.Cargo, s => Data.Cargo = s, objects);
    }
}
