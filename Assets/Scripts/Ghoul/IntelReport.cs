namespace VampireDrama
{
    using System.Collections.Generic;

    public class IntelReport
    {
        public float WantedSnapshot;
        public float TimeGathered;

        public IntelReport(float wanted, float timeGathered)
        {
            WantedSnapshot = wanted;
            TimeGathered = timeGathered;
        }

        public string GetMessage()
        {
            if (WantedSnapshot >= 80f)
                return "Police whispers about vampires";
            if (WantedSnapshot >= 60f)
                return "Police suspect animal attacks";
            if (WantedSnapshot >= 40f)
                return "Police know about missing people having been murdered";
            if (WantedSnapshot >= 20f)
                return "Police are aware of missing people";

            return "The streets seem calm, nothing to report";
        }
    }

    [System.Serializable]
    public class GhoulIntel
    {
        public List<IntelReport> Reports;
        public float TimeSinceLastReport;
        public float ReportInterval;

        private float wantedAtLastSnapshot;
        private bool hasInitialReport;

        public static readonly float InitialReportTime = 3f * 60f;
        public static readonly float UpdateReportTime = 2f * 60f;

        public GhoulIntel()
        {
            Reports = new List<IntelReport>();
            TimeSinceLastReport = 0f;
            ReportInterval = InitialReportTime;
            hasInitialReport = false;
        }

        public void Update(float deltaTime, float currentWanted)
        {
            TimeSinceLastReport += deltaTime;
            wantedAtLastSnapshot = currentWanted;

            if (!hasInitialReport && TimeSinceLastReport >= InitialReportTime)
            {
                AddReport(currentWanted, TimeSinceLastReport);
                hasInitialReport = true;
                TimeSinceLastReport = 0f;
                ReportInterval = UpdateReportTime;
            }
            else if (hasInitialReport && TimeSinceLastReport >= ReportInterval)
            {
                AddReport(currentWanted, TimeSinceLastReport);
                TimeSinceLastReport = 0f;
            }
        }

        private void AddReport(float wanted, float time)
        {
            Reports.Add(new IntelReport(wanted, time));
        }

        public IntelReport GetLatestReport()
        {
            if (Reports.Count == 0) return null;
            return Reports[Reports.Count - 1];
        }

        public bool HasNewReport(int lastSeenCount)
        {
            return Reports.Count > lastSeenCount;
        }
    }
}
