namespace VampireDrama
{
    using System.Collections.Generic;

    public class GameGlobals
    {
        private static GameGlobals instance = null;
        public PlayerStats PlayerStats;
        public GhoulPack GhoulPack;
        public int LevelCompleted;
        public float TimeSpentOnLevel;

        public GameGlobals()
        {
        }

        public static GameGlobals GetInstance()
        {
            if (instance == null)
            {
                instance = new GameGlobals();
                instance.Reset();
            }

            return instance;
        }

        public void Reset()
        {
            PlayerStats.Bloodfill = 0;
            PlayerStats.Experience = 0;
            PlayerStats.Items = new List<InventoryItem>();
            PlayerStats.Abilities = new AbilitySet();
            PlayerStats.Abilities.Unlock(new GlamourAbility());

            GhoulPack = new GhoulPack();

            LevelCompleted = 0;
            TimeSpentOnLevel = 0;
        }
    }
}