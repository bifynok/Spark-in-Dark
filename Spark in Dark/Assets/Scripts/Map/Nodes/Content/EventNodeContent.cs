using UnityEngine;

[CreateAssetMenu(menuName = "Map/Node Content/Event")]
public class EventNodeContent : NodeContent
{
    protected override NodeType ExpectedType => NodeType.Event;
    
    public string eventId;

    public bool canRepeat;
}