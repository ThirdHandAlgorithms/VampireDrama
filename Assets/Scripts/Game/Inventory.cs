using UnityEngine;
using VampireDrama;
//using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] Transform itemsParent;
    [SerializeField] ItemSlot[] itemSlots;

    private void OnValidate()
    {
        if (itemsParent != null)
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();
        
        RefreshUI();
    }

    private void RefreshUI()
    {
        var globals = GameGlobals.GetInstance();

        for (var i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].Item = null;
        }

        for (var i = 0; i < globals.PlayerStats.Items.Count && i < itemSlots.Length; i++)
        {
            itemSlots[i].Item = globals.PlayerStats.Items[i];
        }
    }

    public bool IsFull()
    {
        var globals = GameGlobals.GetInstance();
        return (globals.PlayerStats.Items.Count >= 2);
    }

    // todo: make removeitem()

    public bool AddItem(InventoryItem item)
    {
        if (IsFull())
            return false;

        var globals = GameGlobals.GetInstance();
        globals.PlayerStats.Items.Add(item);
        RefreshUI();

        return true;
    }
}
