using UnityEngine;

public class MapNode : MonoBehaviour
{
    public NodeType Type { get; private set; }

    public NodeState State { get; private set; }

    public MapNode[] NextNodes { get; private set; }

    public MapNode PreviousNode { get; private set; }

    public NodeContent Content {get; private set; }

    [SerializeField] private MapNodeView mapNodeView;

    public void Initialize(NodeContent content)
    {
        if (content == null)
        {
            Debug.LogError("MapNode.Initialize called with null content", this);
            return;
        }

        Content = content;
        Type = content.Type;
        State = NodeState.Future;
        UpdateView();
    }

    public void SetState(NodeState newState)
    {
        State = newState;
        UpdateView();
    }

    public void SetNextNodes(MapNode[] next)
    {
        NextNodes = next;
    }

    public void SetPreviousNode(MapNode prev)
    {
        PreviousNode = prev;
    }

    public void UpdateView()
    {
        mapNodeView.UpdateView(this);
    }
}
