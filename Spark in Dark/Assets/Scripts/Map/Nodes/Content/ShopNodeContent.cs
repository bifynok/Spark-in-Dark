using UnityEngine;

[CreateAssetMenu(menuName = "Map/Node Content/Shop")]
public class ShopNodeContent : NodeContent
{
    protected override NodeType ExpectedType => NodeType.Shop;

    public string shopId;
}