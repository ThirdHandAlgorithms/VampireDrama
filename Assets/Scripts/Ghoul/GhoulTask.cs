namespace VampireDrama
{
    [System.Serializable]
    public class GhoulTask
    {
        public GhoulTaskType TaskType;
        public float Duration;
        public float TimeRemaining;
        public bool IsComplete { get { return TimeRemaining <= 0f; } }

        public GhoulTask(GhoulTaskType taskType, float duration)
        {
            TaskType = taskType;
            Duration = duration;
            TimeRemaining = duration;
        }

        public void Tick(float deltaTime)
        {
            if (TimeRemaining > 0f)
            {
                TimeRemaining = System.Math.Max(0f, TimeRemaining - deltaTime);
            }
        }
    }
}
