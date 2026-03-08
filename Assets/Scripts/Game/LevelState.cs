namespace VampireDrama
{
    [System.Serializable]
    public class LevelState
    {
        public float Wanted;
        public float Reputation;
        public float Influence;

        public static readonly float MaxWanted = 100f;
        public static readonly float MaxReputation = 100f;
        public static readonly float MaxInfluence = 100f;

        public LevelState()
        {
            Wanted = 0f;
            Reputation = 0f;
            Influence = 0f;
        }

        public void OnKill()
        {
            Wanted = System.Math.Min(MaxWanted, Wanted + 10f);
        }

        public void OnSpotted()
        {
            Wanted = System.Math.Min(MaxWanted, Wanted + 5f);
        }

        public void OnGhoulCaught()
        {
            Wanted = System.Math.Min(MaxWanted, Wanted + 15f);
        }

        public void OnFeedUndetected()
        {
            Influence = System.Math.Min(MaxInfluence, Influence + 2f);
        }

        public void OnGhoulTaskComplete(GhoulTaskType taskType)
        {
            switch (taskType)
            {
                case GhoulTaskType.Stealing:
                    Influence = System.Math.Min(MaxInfluence, Influence + 3f);
                    Wanted = System.Math.Min(MaxWanted, Wanted + 2f);
                    break;
                case GhoulTaskType.GatheringInfo:
                    Reputation = System.Math.Min(MaxReputation, Reputation + 5f);
                    break;
                case GhoulTaskType.Capturing:
                    Influence = System.Math.Min(MaxInfluence, Influence + 5f);
                    Wanted = System.Math.Min(MaxWanted, Wanted + 8f);
                    break;
            }
        }

        public void DecayOverTime(float deltaTime)
        {
            float decayRate = 0.5f * deltaTime;
            Wanted = System.Math.Max(0f, Wanted - decayRate);
        }

        public int GetExtraLawEnforcementCount()
        {
            if (Wanted >= 80f) return 4;
            if (Wanted >= 60f) return 3;
            if (Wanted >= 40f) return 2;
            if (Wanted >= 20f) return 1;
            return 0;
        }

        public float GetHumanSuspicionBonus()
        {
            return Wanted / 5f;
        }

        public float GetSpawnerRateMultiplier()
        {
            return 1f + (Wanted / 100f);
        }
    }
}
