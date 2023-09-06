namespace ECAC_eSports_Scraper.DataTypes.GameAPIHandles.Valorant
{
    public class ValorantTrackerStats
    {
        public ValorantAgent TopAgent { get; set; }
        public ValorantAgent.ValorantRole Role { get; set; }
        public TrackerScore TrackerScore { get; set; }
        public ValorantRank PeakRank { get; set; }
        public ValorantRank CurrentRank { get; set; }

        public string WinPercentage { get; set; }
        public string HeadshotPercentage { get; set; }
        public string KdRatio { get; set; }
        public string AverageDamagePerRound { get; set; }

        public ValorantTrackerStats(ValorantAgent topAgent, ValorantAgent.ValorantRole role, TrackerScore trackerScore, string winPercentage, string headshotPercentage, string kDRatio, string averageDamagePerRound, ValorantRank peakRank, ValorantRank currentRank)
        {
            TopAgent = topAgent;
            Role = role;
            TrackerScore = trackerScore;
            WinPercentage = winPercentage;
            HeadshotPercentage = headshotPercentage;
            KdRatio = kDRatio;
            AverageDamagePerRound = averageDamagePerRound;
            PeakRank = peakRank;
            CurrentRank = currentRank;
        }
    }
}
