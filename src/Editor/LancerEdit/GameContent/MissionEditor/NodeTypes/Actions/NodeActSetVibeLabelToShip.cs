﻿using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActSetVibeLabelToShip : BlueprintNode
{
    protected override string Name => "Set Vibe Label to Ship";

    public readonly Act_SetVibeLblToShip Data;

    public NodeActSetVibeLabelToShip(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new() : new Act_SetVibeLblToShip(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, MissionIni missionIni)
    {
        NodeActSetVibe.VibeComboBox(ref Data.Vibe);
        Controls.InputTextId("Label", ref Data.Label);
        Controls.InputTextId("Ship", ref Data.Ship);
    }
}
