using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField] private float forwardOffset = 8f;
    [SerializeField] private float verticalSpacing = 2f;
    [SerializeField] private float randomX = 1.5f;
    [SerializeField] private float randomY = 0.8f;

    public List<Vector2> GenerateNextPositions(Vector2 currentNode, int nodeCount)
    {
        List<Vector2> positions = new(nodeCount);

        float baseX = currentNode.x + forwardOffset;

        float startY = currentNode.y - (nodeCount - 1) * verticalSpacing * 0.5f;

        for (int i = 0; i < nodeCount; i++)
        {
            float x = baseX + Random.Range(-randomX, randomX);
            float y = startY + i * verticalSpacing + Random.Range(-randomY, randomY);

            positions.Add(new Vector2(x, y));
        }

        return positions;
    }
}
