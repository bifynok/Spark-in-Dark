using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour, ISceneEntryPoint
{
    [Header("Prefabs & References")]
    [SerializeField] private MapNode nodePrefab;
    [SerializeField] private MapGenerator positionGenerator;
    [SerializeField] private MapInputHandler inputHandler;
    [SerializeField] private NodeSelectionPanel selectionPanel;
    [SerializeField] private PathLineBuilder pathLineBuilder;

    [Header("Generation Settings")]
    [SerializeField] private int minNodes = 3;
    [SerializeField] private int maxNodes = 4;

    [Header("Content")]
    [SerializeField] private NodeContentDatabase contentDb;

    private MapTraversalService traversal;
    private MapGenerationService generation;
    private MapPersistenceService persistence;

    private readonly List<MapNode> allNodes = new();

    private bool initialized;

    public void OnSceneEnter()
    {
        if (initialized)
            return;

        initialized = true;

        InitializeServices();
        BindInput();
        BindTraversalEvents();

        if (!RunManager.Instance.HasActiveRun ||
            RunManager.Instance.CurrentRun.map == null)
        {
            StartNewRun();
        }
        else
        {
            RestoreRun();
        }
    }

    private void InitializeServices()
    {
        traversal = new MapTraversalService();

        generation = new MapGenerationService(
            nodePrefab,
            positionGenerator,
            contentDb,
            minNodes,
            maxNodes
        );

        persistence = new MapPersistenceService(nodePrefab, contentDb);
    }

    private void BindInput()
    {
        inputHandler.NodeClicked += traversal.SelectNode;
        inputHandler.ConfirmPressed += traversal.ConfirmSelection;
        inputHandler.CancelPressed += traversal.CancelSelection;
    }

    private void BindTraversalEvents()
    {
        traversal.OnNodeSelected += node =>
        {
            selectionPanel.Show(node);
        };

        traversal.OnSelectionCanceled += () =>
        {
            selectionPanel.Hide();
        };

        traversal.OnNodeConfirmed += OnNodeConfirmed;
    }

    private void StartNewRun()
    {
        RunManager.Instance.StartNewRun();

        MapNode startNode = generation.CreateInitialNode();
        allNodes.Add(startNode);

        traversal.Initialize(startNode);

        GenerateNext(startNode);
        SaveMap();
    }

    private void RestoreRun()
    {
        var nodes = persistence.Restore(RunManager.Instance.CurrentRun.map, out MapNode current);

        allNodes.Clear();
        allNodes.AddRange(nodes);

        traversal.SetNodes(allNodes);
        traversal.Initialize(current);

        RestorePathLines();
    }

    private void OnNodeConfirmed(MapNode from, MapNode to)
    {
        pathLineBuilder.DrawLine(
            from.transform.position,
            to.transform.position
        );

        selectionPanel.Hide();

        GenerateNext(to);
        SaveMap();

        if (to.Type == NodeType.Battle)
        {
            SceneLoader.Instance.SwitchScene("Scene_Battle", "Scene_Map");
        }
    }

    private void GenerateNext(MapNode parent)
    {
        var newNodes = generation.GenerateNextNodes(parent);

        foreach (var node in newNodes)
            allNodes.Add(node);

        traversal.SetNodes(allNodes);
    }

    private void SaveMap()
    {
        RunManager.Instance.CurrentRun.map = persistence.Save(allNodes, traversal.CurrentNode);
    }

    private void RestorePathLines()
    {
        foreach (var node in allNodes)
        {
            if (node.PreviousNode != null &&
                (node.State == NodeState.Current ||
                 node.State == NodeState.Passed))
            {
                pathLineBuilder.DrawLine(
                    node.PreviousNode.transform.position,
                    node.transform.position
                );
            }
        }
    }
}
