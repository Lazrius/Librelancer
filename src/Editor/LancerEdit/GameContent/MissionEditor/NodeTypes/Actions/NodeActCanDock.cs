﻿using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActCanDock : BlueprintNode
{
    protected override string Name => "Toggle Player Docking Ability";

    public readonly Act_PlayerCanDock Data;
    public NodeActCanDock(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new() : new Act_PlayerCanDock(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
        ImGui.Checkbox("Can Dock", ref Data.CanDock);
        Controls.InputStringList("Exceptions", Data.Exceptions);
    }
}
