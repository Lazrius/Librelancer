﻿using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActSetVibeShipToLabel : BlueprintNode
{
    protected override string Name => "Set Vibe Ship to Label";

    public readonly Act_SetVibeShipToLbl Data;

    public NodeActSetVibeShipToLabel(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new() : new Act_SetVibeShipToLbl(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
        NodeActSetVibe.VibeComboBox(ref Data.Vibe, nodePopups);
        Controls.InputTextId("Ship", ref Data.Ship);
        Controls.InputTextId("Label", ref Data.Label);
    }
}
