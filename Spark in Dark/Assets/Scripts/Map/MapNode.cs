using UnityEngine;

public class MapNode : MonoBehaviour
{
    public NodeType Type { get; private set; }

    public NodeState State { get; private set; }

    public MapNode[] NextNodes { get; private set; }

    public MapNode PreviousNode { get; private set; }

    [SerializeField] private MapNodeView mapNodeView;

    public void Initialize(NodeType type)
    {
        Type = type;
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
