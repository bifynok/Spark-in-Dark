using UnityEngine;

public abstract class NodeContent : ScriptableObject
{
    [SerializeField] private NodeType type;
    public NodeType Type => type;

    public string Title;
    public string Description;

    [SerializeField] private string id;
    public string Id => id;

    protected abstract NodeType ExpectedType { get; }

    protected virtual void OnValidate()
    {
        type = ExpectedType;

        if (string.IsNullOrWhiteSpace(id))
            id = System.Guid.NewGuid().ToString("N");
    }
}
