using ImGuiNET;
using LibreLancer.Data.Missions;
using LibreLancer.ImUI;
using LibreLancer.ImUI.NodeEditor;
using LibreLancer.Ini;

namespace LancerEdit.GameContent.MissionEditor.NodeTypes.Conditions;

public class NodeCndBaseEnter : BlueprintNode
{
    protected override string Name => "On Base Enter";

    private string @base = string.Empty;
    public NodeCndBaseEnter(ref int id, Entry entry) : base(ref id, NodeColours.Condition)
    {
        if (entry?.Count >= 1)
        {
            @base = entry[0].ToString();
        }

        Inputs.Add(new NodePin(this, LinkType.Condition, PinKind.Input));
    }

    protected override void RenderContent(GameDataContext gameData, PopupManager popup, ref NodePopups nodePopups,
        MissionIni missionIni)
    {
        Controls.InputTextId("Base", ref @base);
    }
}
