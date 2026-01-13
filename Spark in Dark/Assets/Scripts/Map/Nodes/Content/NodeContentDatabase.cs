using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/Node Content/Database")]
public class NodeContentDatabase : ScriptableObject
{
    public NodeContent[] all;

    private Dictionary<NodeType, List<NodeContent>> cache;

    private void OnEnable()
    {
        cache = new Dictionary<NodeType, List<NodeContent>>();
        foreach (var c in all)
        {
            if (c == null) continue;
            if (!cache.TryGetValue(c.Type, out var list))
            {
                list = new List<NodeContent>();
                cache[c.Type] = list;
            }
            list.Add(c);
        }
    }

    public NodeContent GetRandomByType(NodeType type)
    {
        if (cache == null || cache.Count == 0) OnEnable();

        if (!cache.TryGetValue(type, out var list) || list.Count == 0)
            throw new Exception($"No NodeContent for type: {type}. Add assets to NodeContentDatabase.");

        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public NodeContent GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new Exception("NodeContentDatabase.GetById: id is null/empty.");

        foreach (var c in all)
            if (c != null && c.Id == id)
                return c;

        throw new Exception($"NodeContentDatabase: NodeContent not found by id: {id}");
    }
}
