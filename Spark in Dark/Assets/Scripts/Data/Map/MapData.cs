using System.Collections.Generic;

[System.Serializable]
public class MapData
{
    public List<MapNodeData> nodes = new();
    public int currentNodeId;
}
