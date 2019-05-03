using UnityEngine;
using VampireDrama;

[System.Serializable]
public struct ItemStats
{
    public string ItemName;
    public Sprite Icon;
    public float Strength;
    public float TravelSpeed;
    public PossibleEffects[] Effects;
}

public class InventoryItem
{
    public ItemStats Stats;
}

public class Item : MonoBehaviour
{
    public ItemStats Stats;

    public InventoryItem CreateInventoryItem()
    {
        var item = new InventoryItem();
        item.Stats = Stats;

        return item;
    }
}
