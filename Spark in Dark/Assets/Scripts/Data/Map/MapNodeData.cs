using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapNodeData
{
    public int id;

    public NodeType type;
    public NodeState state;

    public Vector2 position;

    public int? previousNodeId;
    public List<int> nextNodeIds = new();
}
