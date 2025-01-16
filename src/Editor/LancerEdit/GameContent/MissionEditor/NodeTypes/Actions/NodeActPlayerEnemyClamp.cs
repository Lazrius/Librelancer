﻿using System;
using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActPlayerEnemyClamp : BlueprintNode
{
    protected override string Name => "Clamp Amount of Enemies Attacking Player";

    public readonly Act_PlayerEnemyClamp Data;
    public NodeActPlayerEnemyClamp(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new() : new Act_PlayerEnemyClamp(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, MissionIni missionIni)
    {
        ImGui.InputInt("Min", ref Data.Min, 1, 10);
        ImGui.InputInt("Max", ref Data.Max, 1, 10);

        Data.Min = Math.Clamp(Data.Min, 0, Data.Max);
        Data.Max = Math.Clamp(Data.Max, Data.Min, 100);
    }
}
