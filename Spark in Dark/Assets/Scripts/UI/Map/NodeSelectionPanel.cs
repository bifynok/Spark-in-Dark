using UnityEngine;
using TMPro;

public class NodeSelectionPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MapManager mapManager;
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    private MapNode node;

    public void Show(MapNode node)
    {
        this.node = node;
        root.SetActive(true);

        titleText.text = node.Type.ToString();
        descriptionText.text = GetDescription(node.Type);
    }

    public void Hide()
    {
        root.SetActive(false);
        node = null;
    }

    public void OnConfirm()
    {
        mapManager.ConfirmSelection();
    }

    public void OnClose()
    {
        mapManager.CancelSelection();
    }

    private string GetDescription(NodeType type)
    {
        return type switch
        {
            NodeType.Battle => "It's time to warm up.",
            NodeType.EliteBattle => "High risk, high reward.",
            NodeType.BossBattle => "A big guy and big problems... for you",
            NodeType.Shop => "Your gold - my gold.",
            NodeType.Event => "What awaits you here?",
            NodeType.Treasure => "Oo, shiny.",
            _ => ""
        };
    }
}
