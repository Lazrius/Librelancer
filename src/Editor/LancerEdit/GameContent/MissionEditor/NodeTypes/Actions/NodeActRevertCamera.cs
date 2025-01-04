﻿using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActRevertCamera : BlueprintNode
{
    protected override string Name => "Revert Camera";

    private readonly Act_RevertCam data;
    public NodeActRevertCamera(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        data = new Act_RevertCam(action);

        Inputs.Add(new NodePin(id++, this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, MissionIni missionIni)
    {
    }
}
