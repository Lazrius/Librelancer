﻿using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Missions;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Actions;

public sealed class NodeActSetTitle : BlueprintNode
{
    protected override string Name => "Set Mission Title";

    private readonly Act_SetTitle data;
    public NodeActSetTitle(ref int id, MissionAction action) : base(ref id, NodeColours.Action)
    {
        data = new Act_SetTitle(action);

        Inputs.Add(new NodePin(id++, this, LinkType.Action, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, MissionIni missionIni)
    {
        Controls.IdsInputString("IDS", gameData, popup, ref data.Ids, (ids) => data.Ids = ids);
    }
}
