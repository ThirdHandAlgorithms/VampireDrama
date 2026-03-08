namespace VampireDrama
{
    using System.Collections.Generic;

    [System.Serializable]
    public class GhoulPack
    {
        public static readonly int MaxGhouls = 3;

        public List<Ghoul> Ghouls;

        public GhoulPack()
        {
            Ghouls = new List<Ghoul>();
        }

        public int Count { get { return Ghouls.Count; } }
        public bool IsFull { get { return Ghouls.Count >= MaxGhouls; } }

        public Ghoul Recruit(string name, float bloodTaxInterval, int bloodTaxCost)
        {
            if (IsFull) return null;

            var ghoul = new Ghoul(name, bloodTaxInterval, bloodTaxCost);
            Ghouls.Add(ghoul);
            return ghoul;
        }

        public void Remove(Ghoul ghoul)
        {
            Ghouls.Remove(ghoul);
        }

        public void Update(float deltaTime, float currentWanted)
        {
            foreach (var ghoul in Ghouls)
            {
                ghoul.Update(deltaTime, currentWanted);
            }
        }

        public int CollectBloodTax()
        {
            int totalTax = 0;
            foreach (var ghoul in Ghouls)
            {
                if (ghoul.BloodTaxDue)
                {
                    totalTax += ghoul.BloodTaxCost;
                    ghoul.PayBloodTax();
                }
            }
            return totalTax;
        }

        public IntelReport GetLatestIntel()
        {
            IntelReport latest = null;
            foreach (var ghoul in Ghouls)
            {
                if (ghoul.Intel != null)
                {
                    var report = ghoul.Intel.GetLatestReport();
                    if (report != null && (latest == null || report.TimeGathered > latest.TimeGathered))
                    {
                        latest = report;
                    }
                }
            }
            return latest;
        }

        public List<GhoulTask> CollectCompletedTasks()
        {
            var completed = new List<GhoulTask>();
            foreach (var ghoul in Ghouls)
            {
                if (ghoul.CurrentTask != null && ghoul.CurrentTask.IsComplete && ghoul.CurrentTask.TaskType != GhoulTaskType.Idle)
                {
                    completed.Add(ghoul.CurrentTask);
                    ghoul.CurrentTask = new GhoulTask(GhoulTaskType.Idle, 0f);
                }
            }
            return completed;
        }
    }
}
