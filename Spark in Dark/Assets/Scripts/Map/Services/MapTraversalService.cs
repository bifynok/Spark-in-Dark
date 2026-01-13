using System;
using System.Collections.Generic;

public class MapTraversalService
{
    public MapNode CurrentNode { get; private set; }
    public MapNode SelectedNode { get; private set; }

    private IReadOnlyList<MapNode> allNodes;

    public event Action<MapNode> OnNodeSelected;
    public event Action<MapNode, MapNode> OnNodeConfirmed;
    public event Action OnSelectionCanceled;

    public void Initialize(MapNode startNode)
    {
        CurrentNode = startNode;
        CurrentNode.SetState(NodeState.Current);
    }

    public void SetNodes(IReadOnlyList<MapNode> nodes)
    {
        allNodes = nodes;
    }

    public bool CanSelectNode(MapNode node)
    {
        return node != null && node.State == NodeState.Future;
    }

    public void SelectNode(MapNode node)
    {
        if (!CanSelectNode(node))
            return;

        SelectedNode = node;
        OnNodeSelected?.Invoke(node);
    }

    public void ConfirmSelection()
    {
        if (SelectedNode == null)
            return;

        MapNode from = CurrentNode;
        MapNode to = SelectedNode;

        from.SetState(NodeState.Passed);
        to.SetState(NodeState.Current);

        CurrentNode = to;
        SelectedNode = null;

        RecalculateNodeStates();

        OnNodeConfirmed?.Invoke(from, to);
    }

    public void CancelSelection()
    {
        SelectedNode = null;
        OnSelectionCanceled?.Invoke();
    }

    private void RecalculateNodeStates()
    {
        if (allNodes == null)
            return;

        foreach (var node in allNodes)
        {
            if (node.State == NodeState.Passed ||
                node.State == NodeState.Current)
                continue;

            if (node.PreviousNode == CurrentNode)
                node.SetState(NodeState.Future);
            else
                node.SetState(NodeState.Skipped);
        }
    }
}
