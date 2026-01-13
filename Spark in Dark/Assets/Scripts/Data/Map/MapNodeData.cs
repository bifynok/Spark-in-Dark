using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapNodeData
{
    public int id;

    public NodeType type;
    public string contentId;

    public NodeState state;

    public Vector2 position;

    public bool hasPrevious;
    public int previousNodeId;    
    public List<int> nextNodeIds = new();
}
