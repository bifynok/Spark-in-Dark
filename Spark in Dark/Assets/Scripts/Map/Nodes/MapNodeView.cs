using UnityEngine;
using TMPro;

public class MapNodeView : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer circleRenderer;
    [SerializeField] private TextMeshPro textLabel;

    [Header("Colors")]
    [SerializeField] private Color32 passedColor = new Color32(0x22, 0xAD, 0x22, 0xFF);
    [SerializeField] private Color32 currentColor = new Color32(0x1C, 0x79, 0xD7, 0xFF);
    [SerializeField] private Color32 futureColor = new Color32(0xE9, 0xDD, 0x57, 0xFF);
    [SerializeField] private Color32 skippedColor = new Color32(0xB8, 0x37, 0x3B, 0xFF);

    public void UpdateView(MapNode node)
    {
        if (node == null) return;

        switch (node.State)
        {
            case NodeState.Future:
                circleRenderer.color = futureColor;
                break;

            case NodeState.Current:
                circleRenderer.color = currentColor;
                break;

            case NodeState.Passed:
                circleRenderer.color = passedColor;
                break;

            case NodeState.Skipped:
                circleRenderer.color = skippedColor;
                break;
        }

        textLabel.text = node.Type.ToString();
    }
}
