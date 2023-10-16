namespace BotV2.DataTypes.GameAPIHandles.Valorant
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

        public static ValorantTrackerStats DefaultStats()
        {
            return new ValorantTrackerStats(
                new ValorantAgent("Sage", ValorantAgent.ValorantRole.Sentinel, "https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png", 0.0, 0.0, 0.0),
                ValorantAgent.ValorantRole.Sentinel,
                new TrackerScore(0, 0.0, 0.0, 0.0, 0.0),
                "0%",
                "0%",
                "0",
                "0",
                new ValorantRank("Unranked", "https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png"),
                new ValorantRank("Unranked", "https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png")
            );
        }
    }
}
