using System.Collections.Generic;
using UnityEngine;

public class MapGenerationService
{
    private readonly MapNode nodePrefab;
    private readonly MapGenerator positionGenerator;
    private readonly NodeContentDatabase contentDb;

    private readonly int minNodes;
    private readonly int maxNodes;

    public MapGenerationService(MapNode nodePrefab, MapGenerator positionGenerator, NodeContentDatabase contentDb, int minNodes, int maxNodes)
    {
        this.nodePrefab = nodePrefab;
        this.positionGenerator = positionGenerator;
        this.contentDb = contentDb;
        this.minNodes = minNodes;
        this.maxNodes = maxNodes;
    }

    public MapNode CreateInitialNode()
    {
        MapNode node = Object.Instantiate(nodePrefab, Vector2.zero, Quaternion.identity);

        NodeContent content = contentDb.GetRandomByType(NodeType.Event);
        node.Initialize(content);

        return node;
    }

    public IReadOnlyList<MapNode> GenerateNextNodes(MapNode parentNode)
    {
        int count = Random.Range(minNodes, maxNodes + 1);

        List<Vector2> positions =
            positionGenerator.GenerateNextPositions(
                parentNode.transform.position,
                count
            );

        List<MapNode> nodes = new(count);

        for (int i = 0; i < count; i++)
        {
            MapNode node = Object.Instantiate(nodePrefab, positions[i], Quaternion.identity);

            NodeType type = GetRandomType();
            NodeContent content = contentDb.GetRandomByType(type);

            node.Initialize(content);
            node.SetPreviousNode(parentNode);

            nodes.Add(node);
        }

        parentNode.SetNextNodes(nodes.ToArray());

        return nodes;
    }

    private NodeType GetRandomType()
    {
        var values = (NodeType[])System.Enum.GetValues(typeof(NodeType));
        return values[Random.Range(0, values.Length)];
    }
}
