using UnityEngine;

[CreateAssetMenu(menuName = "Map/Node Content/Battle")]
public class BattleNodeContent : NodeContent
{
    protected override NodeType ExpectedType => NodeType.Battle;

    public string arenaId;
    
    public int difficulty;
}