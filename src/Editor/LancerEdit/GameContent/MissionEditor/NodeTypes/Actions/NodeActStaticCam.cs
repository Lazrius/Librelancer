﻿using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActStaticCamera : BlueprintNode
{
    protected override string Name => "Set Static Camera";

    private readonly Act_StaticCam data;
    public NodeActStaticCamera(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        data = new Act_StaticCam(action);

        Inputs.Add(new NodePin(id++, this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, MissionIni missionIni)
    {
        ImGui.InputFloat3("Position", ref data.Position);
        Controls.InputFlQuaternion("Orientation", ref data.Orientation);
    }
}
