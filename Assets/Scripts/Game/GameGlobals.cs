namespace VampireDrama
{
    public class GameGlobals
    {
        private static GameGlobals instance = null;
        public PlayerStats PlayerStats;
        public int LevelCompleted;

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
    }
}