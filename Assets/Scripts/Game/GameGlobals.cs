namespace VampireDrama
{
    public class GameGlobals
    {
        private static GameGlobals instance = null;
        public PlayerStats PlayerStats;
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
            }

            return instance;
        }

        public void Reset()
        {
            PlayerStats.Bloodfill = 0;
            PlayerStats.Experience = 0;

            LevelCompleted = 0;
            TimeSpentOnLevel = 0;
        }
    }
}