﻿using System.Linq;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

// ReSharper disable once InconsistentNaming
public sealed class NodeActSetNNObject : BlueprintNode
{
    protected override string Name => "Set NN Objective";

    public readonly Act_SetNNObj Data;
    public NodeActSetNNObject(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new() : new Act_SetNNObj(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
        var objectives = missionIni.Objectives.Select(x => x.Nickname).ToArray();
        nodePopups.StringCombo("Objective", Data.Objective, s => Data.Objective = s, objectives);
    }
}
