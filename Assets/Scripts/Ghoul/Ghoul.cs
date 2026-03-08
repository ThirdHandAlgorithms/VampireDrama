namespace VampireDrama
{
    [System.Serializable]
    public class Ghoul
    {
        public string Name;
        public float BloodTaxTimer;
        public float BloodTaxInterval;
        public int BloodTaxCost;
        public GhoulTask CurrentTask;
        public GhoulIntel Intel;

        public Ghoul(string name, float bloodTaxInterval, int bloodTaxCost)
        {
            Name = name;
            BloodTaxInterval = bloodTaxInterval;
            BloodTaxCost = bloodTaxCost;
            BloodTaxTimer = bloodTaxInterval;
            CurrentTask = new GhoulTask(GhoulTaskType.Idle, 0f);
        }

        public bool IsIdle
        {
            get { return CurrentTask == null || CurrentTask.TaskType == GhoulTaskType.Idle; }
        }

        public bool BloodTaxDue
        {
            get { return BloodTaxTimer <= 0f; }
        }

        public void AssignTask(GhoulTaskType taskType, float duration)
        {
            CurrentTask = new GhoulTask(taskType, duration);

            if (taskType == GhoulTaskType.GatheringInfo)
            {
                Intel = new GhoulIntel();
            }
        }

        public void Update(float deltaTime, float currentWanted)
        {
            if (BloodTaxTimer > 0f)
            {
                BloodTaxTimer = System.Math.Max(0f, BloodTaxTimer - deltaTime);
            }

            if (CurrentTask != null && !CurrentTask.IsComplete)
            {
                CurrentTask.Tick(deltaTime);
            }

            if (Intel != null && CurrentTask != null && CurrentTask.TaskType == GhoulTaskType.GatheringInfo)
            {
                Intel.Update(deltaTime, currentWanted);
            }
        }

        public void PayBloodTax()
        {
            BloodTaxTimer = BloodTaxInterval;
        }
    }
}
