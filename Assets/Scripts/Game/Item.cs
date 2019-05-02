using UnityEngine;

public class InventoryItem
{
    public string ItemName;
    public Sprite Icon;
    public float Strength;
    public float TravelSpeed;
}

public class Item : MonoBehaviour
{
    public string ItemName;
    public Sprite Icon;
    public float Strength;
    public float TravelSpeed;

    public InventoryItem CreateInventoryItem()
    {
        var clone = new InventoryItem();
        clone.ItemName = ItemName;
        clone.Icon = Icon;
        clone.Strength = Strength;
        clone.TravelSpeed = TravelSpeed;

        return clone;
    }
}
