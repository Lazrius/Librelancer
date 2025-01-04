﻿using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActSpwanLoot : BlueprintNode
{
    protected override string Name => "Spawn Loot";

    private readonly Act_SpawnLoot data;
    public NodeActSpwanLoot(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        data = new Act_SpawnLoot(action);

        Inputs.Add(new NodePin(id++, "Trigger", this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, MissionIni missionIni)
    {
        Controls.InputTextId("Loot", ref data.Loot);
    }
}
