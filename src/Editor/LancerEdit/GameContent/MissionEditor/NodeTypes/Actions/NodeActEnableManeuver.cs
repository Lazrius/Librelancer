﻿using System;
using System.Linq;
using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;
using LibreLancer.Missions;
using LibreLancer.Missions.Actions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActEnableManeuver : TriggerEntryNode
{
    protected override string Name => "Enable Maneuver";

    public readonly Act_EnableManeuver Data;
    public NodeActEnableManeuver(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        Data = action is null ? new() : new Act_EnableManeuver(action);

        Inputs.Add(new NodePin(this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        ref NodeLookups lookups)
    {
        nodePopups.Combo("Maneuver", Data.Maneuver, x => Data.Maneuver = x);
        ImGui.Checkbox("Lock", ref Data.Lock);
    }

    public override void WriteEntry(IniBuilder.IniSectionBuilder sectionBuilder)
    {
        Data.Write(sectionBuilder);
    }
}
