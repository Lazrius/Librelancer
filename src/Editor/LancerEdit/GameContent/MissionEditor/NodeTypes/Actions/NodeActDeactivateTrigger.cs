﻿using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActDeactivateTrigger : BlueprintNode
{
    protected override string Name => "Deactivate Trigger";

    public readonly Act_DeactTrig Data;
    public NodeActDeactivateTrigger(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = new Act_DeactTrig(action);

        Inputs.Add(new NodePin(id++, this, LinkType.Action, PinKind.Input));
        Outputs.Add(new NodePin(id++, this, LinkType.Trigger, PinKind.Output));

    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, MissionIni missionIni)
    {
        Controls.InputTextId("Trigger", ref Data.Trigger);
    }
}
