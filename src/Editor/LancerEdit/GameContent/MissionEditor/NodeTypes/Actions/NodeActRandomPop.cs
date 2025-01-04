﻿using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActRandomPop : BlueprintNode
{
    protected override string Name => "Toggle Global Random Population";

    private readonly Act_RandomPopSphere data;
    public NodeActRandomPop(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        data = new Act_RandomPopSphere(action);

        Inputs.Add(new NodePin(id++, "Trigger", this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, MissionIni missionIni)
    {
        ImGui.Checkbox("Enable", ref data.On);
    }
}
