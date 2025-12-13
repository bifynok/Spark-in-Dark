using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MapNode nodePrefab;
    [SerializeField] private MapGenerator generator;
    [SerializeField] private CameraController camController;
    [SerializeField] private PathLineBuilder pathLineBuilder;
    [SerializeField] private NodeSelectionPanel selectionPanel;

    [Header("Generation Settings")]
    [SerializeField] private int minNodes = 3;
    [SerializeField] private int maxNodes = 4;

    private MapNode currentNode;
    private MapNode selectedNode;

    private List<MapNode> lastGeneratedNodes = new();
    private readonly List<MapNode> allNodes = new();

    private bool initialized;

    public void OnSceneActivated()
    {
        if (initialized) return;
        initialized = true;

        if (!RunManager.Instance.HasActiveRun)
        {
            RunManager.Instance.StartNewRun();
            GenerateInitialNode();
            SaveMapData();
            return;
        }

        MapData data = RunManager.Instance.CurrentRun.map;

        if (data == null)
        {
            ClearMap();
            GenerateInitialNode();
        }
        else
        {
            ClearMap();
            RestoreMap(data);
        }
    }

    private void Update()
    {
        if (camController != null && camController.BlocksClick)
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0f));
            worldPos.z = 0f;

            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null)
            {
                MapNode node = hit.collider.GetComponent<MapNode>();
                if (node != null)
                    OnNodeClicked(node);
            }
        }
    }

    private void GenerateInitialNode()
    {
        currentNode = CreateNode(Vector2.zero, NodeType.Event);
        currentNode.SetState(NodeState.Current);

        int nextCount = Random.Range(minNodes, maxNodes + 1);
        GenerateNextNodes(currentNode, nextCount);
    }

    private void GenerateNextNodes(MapNode parent, int count)
    {
        lastGeneratedNodes.Clear();

        List<Vector2> positions = generator.GenerateNextPositions(parent.transform.position, count);

        for (int i = 0; i < count; i++)
        {
            MapNode newNode = CreateNode(positions[i], GetRandomType());
            newNode.SetPreviousNode(parent);

            lastGeneratedNodes.Add(newNode);
        }

        parent.SetNextNodes(lastGeneratedNodes.ToArray());
    }

    private NodeType GetRandomType()
    {
        return (NodeType)Random.Range(0, System.Enum.GetValues(typeof(NodeType)).Length);
    }

    public void OnNodeClicked(MapNode node)
    {
        if (node.State != NodeState.Future)
            return;

        SelectNode(node);
    }

    private void SelectNode(MapNode node)
    {
        selectedNode = node;
        selectionPanel.Show(node);
    }

    public void ConfirmSelection()
{
    if (selectedNode == null)
        return;

    currentNode.SetState(NodeState.Passed);

    pathLineBuilder.DrawLine(
        currentNode.transform.position,
        selectedNode.transform.position
    );

    selectedNode.SetState(NodeState.Current);
    currentNode = selectedNode;
    selectedNode = null;

    selectionPanel.Hide();

    int nextCount = Random.Range(minNodes, maxNodes + 1);
    GenerateNextNodes(currentNode, nextCount);

    RecalculateNodeStates();

    SaveMapData();

    if (currentNode.Type == NodeType.Battle)
    {
        SceneLoader.Instance.SwitchScene("Scene_Battle", "Scene_Map");
    }
}

    public void CancelSelection()
    {
        selectedNode = null;
        selectionPanel.Hide();
    }

    private MapNode CreateNode(Vector2 position, NodeType type)
    {
        MapNode node = Instantiate(nodePrefab, position, Quaternion.identity);
        node.Initialize(type);

        allNodes.Add(node);

        return node;
    }

    private void SaveMapData()
    {
        MapData data = new MapData();

        Dictionary<MapNode, int> idMap = new();

        for (int i = 0; i < allNodes.Count; i++)
            idMap[allNodes[i]] = i;

        foreach (var node in allNodes)
        {
            MapNodeData nodeData = new MapNodeData
            {
                id = idMap[node],
                type = node.Type,
                state = node.State,
                position = node.transform.position,
                previousNodeId = node.PreviousNode != null ? idMap[node.PreviousNode] : null
            };

            if (node.NextNodes != null)
            {
                foreach (var next in node.NextNodes)
                    nodeData.nextNodeIds.Add(idMap[next]);
            }

            data.nodes.Add(nodeData);

            if (node.State == NodeState.Current)
                data.currentNodeId = nodeData.id;
        }

        RunManager.Instance.CurrentRun.map = data;
    }

    private void RestoreMap(MapData data)
    {
        Dictionary<int, MapNode> created = new();

        foreach (var nodeData in data.nodes)
        {
            MapNode node = CreateNode(nodeData.position, nodeData.type);
            node.SetState(nodeData.state);

            created[nodeData.id] = node;
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

        RestorePathLines(created);
    }

    private void RestorePathLines(Dictionary<int, MapNode> nodes)
    {
        foreach (var node in nodes.Values)
        {
            if (node.PreviousNode != null &&
                (node.State == NodeState.Current || node.State == NodeState.Passed))
            {
                pathLineBuilder.DrawLine(
                    node.PreviousNode.transform.position,
                    node.transform.position
                );
            }
        }
    }

    private void ClearMap()
    {
        foreach (var node in allNodes)
            Destroy(node.gameObject);

        allNodes.Clear();
        lastGeneratedNodes.Clear();
        currentNode = null;
        selectedNode = null;
    }

    private void RecalculateNodeStates()
    {
        foreach (var node in allNodes)
        {
            if (node.State == NodeState.Passed || node.State == NodeState.Current)
                continue;

            if (node.PreviousNode == currentNode)
            {
                node.SetState(NodeState.Future);
            }
            else
            {
                node.SetState(NodeState.Skipped);
            }
        }
    }
}
