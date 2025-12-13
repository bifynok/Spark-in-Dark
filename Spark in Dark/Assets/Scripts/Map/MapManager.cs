using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

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
    private readonly List<MapNode> chosenPath = new();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GenerateInitialNode();
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
        currentNode = Instantiate(nodePrefab, Vector2.zero, Quaternion.identity);
        currentNode.Initialize(NodeType.Event);
        currentNode.SetState(NodeState.Current);

        chosenPath.Clear();
        chosenPath.Add(currentNode);

        int nextCount = Random.Range(minNodes, maxNodes + 1);
        GenerateNextNodes(currentNode, nextCount);
    }

    private void GenerateNextNodes(MapNode parent, int count)
    {
        lastGeneratedNodes.Clear();

        List<Vector2> positions = generator.GenerateNextPositions(parent.transform.position, count);

        for (int i = 0; i < count; i++)
        {
            MapNode newNode = Instantiate(nodePrefab, positions[i], Quaternion.identity);
            newNode.Initialize(GetRandomType());
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
        if (selectedNode == null) return;

        currentNode.SetState(NodeState.Passed);

        pathLineBuilder.DrawLine(
            currentNode.transform.position,
            selectedNode.transform.position
        );

        foreach (var node in lastGeneratedNodes)
        {
            if (node != selectedNode)
                node.SetState(NodeState.Skipped);
        }

        selectedNode.SetState(NodeState.Current);
        currentNode = selectedNode;
        selectedNode = null;

        chosenPath.Add(currentNode);

        selectionPanel.Hide();

        int nextCount = Random.Range(minNodes, maxNodes + 1);
        GenerateNextNodes(currentNode, nextCount);
    }

    public void CancelSelection()
    {
        selectedNode = null;
        selectionPanel.Hide();
    }
}
