using System.Collections.Generic;
using UnityEngine;

public class MapGenerationService
{
    private readonly MapNode nodePrefab;
    private readonly MapGenerator positionGenerator;

    private readonly int minNodes;
    private readonly int maxNodes;

    public MapGenerationService(MapNode nodePrefab, MapGenerator positionGenerator, int minNodes, int maxNodes)
    {
        this.nodePrefab = nodePrefab;
        this.positionGenerator = positionGenerator;
        this.minNodes = minNodes;
        this.maxNodes = maxNodes;
    }

    public MapNode CreateInitialNode()
    {
        MapNode node = Object.Instantiate(nodePrefab, Vector2.zero, Quaternion.identity);
        node.Initialize(NodeType.Event);
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
            node.Initialize(GetRandomType());
            node.SetPreviousNode(parentNode);

            nodes.Add(node);
        }

        parentNode.SetNextNodes(nodes.ToArray());

        return nodes;
    }

    private NodeType GetRandomType()
    {
        return (NodeType)Random.Range(
            0,
            System.Enum.GetValues(typeof(NodeType)).Length
        );
    }
}
