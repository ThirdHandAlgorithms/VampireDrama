namespace VampireDrama
{
    using System.Collections.Generic;

    [System.Serializable]
    public struct PlayerStats
    {
        public int Experience;
        public int Bloodfill;
        public List<InventoryItem> Items;
    }
}
