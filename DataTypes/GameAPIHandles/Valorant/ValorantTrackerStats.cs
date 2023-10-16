namespace ECAC_eSports.DataTypes.GameAPIHandles.Valorant
{
    public record ValorantTrackerStats(ValorantAgent TopAgent, ValorantAgent.ValorantRole Role, TrackerScore TrackerScore, string WinPercentage, string HeadshotPercentage, string KdRatio, string AverageDamagePerRound, ValorantRank PeakRank, ValorantRank CurrentRank)
    {
        public ValorantAgent TopAgent { get; set; } = TopAgent;
        public ValorantAgent.ValorantRole Role { get; set; } = Role;
        public TrackerScore TrackerScore { get; set; } = TrackerScore;
        public ValorantRank PeakRank { get; set; } = PeakRank;
        public ValorantRank CurrentRank { get; set; } = CurrentRank;

        public string WinPercentage { get; set; } = WinPercentage;
        public string HeadshotPercentage { get; set; } = HeadshotPercentage;
        public string KdRatio { get; set; } = KdRatio;
        public string AverageDamagePerRound { get; set; } = AverageDamagePerRound;
    }
}
