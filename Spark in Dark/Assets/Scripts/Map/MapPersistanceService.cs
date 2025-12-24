using System.Collections.Generic;
using UnityEngine;

public class MapPersistenceService
{
    private readonly MapNode nodePrefab;

    public MapPersistenceService(MapNode nodePrefab)
    {
        this.nodePrefab = nodePrefab;
    }

    public MapData Save(IReadOnlyList<MapNode> nodes, MapNode currentNode)
    {
        MapData data = new MapData();
        Dictionary<MapNode, int> idMap = new();

        for (int i = 0; i < nodes.Count; i++)
            idMap[nodes[i]] = i;

        foreach (var node in nodes)
        {
            MapNodeData nodeData = new MapNodeData
            {
                id = idMap[node],
                type = node.Type,
                state = node.State,
                position = node.transform.position,
                previousNodeId = node.PreviousNode != null
                    ? idMap[node.PreviousNode]
                    : null
            };

            if (node.NextNodes != null)
            {
                foreach (var next in node.NextNodes)
                    nodeData.nextNodeIds.Add(idMap[next]);
            }

            data.nodes.Add(nodeData);

            if (node == currentNode)
                data.currentNodeId = nodeData.id;
        }

        return data;
    }

    public IReadOnlyList<MapNode> Restore(MapData data, out MapNode currentNode)
    {
        Dictionary<int, MapNode> created = new();
        List<MapNode> nodes = new();

        foreach (var nodeData in data.nodes)
        {
            MapNode node = Object.Instantiate(
                nodePrefab,
                nodeData.position,
                Quaternion.identity
            );

            node.Initialize(nodeData.type);
            node.SetState(nodeData.state);

            created[nodeData.id] = node;
            nodes.Add(node);
        }

        foreach (var nodeData in data.nodes)
        {
            MapNode node = created[nodeData.id];

            if (nodeData.previousNodeId.HasValue)
                node.SetPreviousNode(created[nodeData.previousNodeId.Value]);

            if (nodeData.nextNodeIds.Count > 0)
            {
                MapNode[] next = nodeData.nextNodeIds
                    .ConvertAll(id => created[id])
                    .ToArray();

                node.SetNextNodes(next);
            }
        }

        currentNode = created[data.currentNodeId];
        return nodes;
    }

}
