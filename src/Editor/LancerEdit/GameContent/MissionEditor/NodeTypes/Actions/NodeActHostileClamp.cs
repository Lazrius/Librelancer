﻿using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActHostileClamp : BlueprintNode
{
    protected override string Name => "Toggle Hostile Clamp";

    public readonly Act_HostileClamp Data;
    public NodeActHostileClamp(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new() : new Act_HostileClamp(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
        ImGui.Checkbox("Enable", ref Data.Enabled);
    }
}
