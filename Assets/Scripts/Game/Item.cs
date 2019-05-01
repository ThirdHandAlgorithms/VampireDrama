using UnityEngine;

public class InventoryItem
{
    public string ItemName;
    public Sprite Icon;
}

public class Item : MonoBehaviour
{
    public string ItemName;
    public Sprite Icon;

    public InventoryItem CreateInventoryItem()
    {
        var clone = new InventoryItem();
        clone.ItemName = ItemName;
        clone.Icon = Icon;

        return clone;
    }
}
