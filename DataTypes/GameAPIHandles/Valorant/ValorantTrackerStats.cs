// ReSharper disable UnusedMember.Global
namespace ECAC_eSports_Bot.DataTypes.GameAPIHandles.Valorant
{
    public record ValorantTrackerStats(ValorantAgent? TopAgent, AgentData.AgentClass Role, TrackerScore TrackerScore, string WinPercentage, string? HeadshotPercentage, string? KdRatio, string AverageDamagePerRound, ValorantRank PeakRank, ValorantRank CurrentRank, bool ValidTracker)
    {
        public ValorantAgent? TopAgent { get; set; } = TopAgent;
        public AgentData.AgentClass Role { get; set; } = Role;
        public TrackerScore TrackerScore { get; set; } = TrackerScore;
        public ValorantRank PeakRank { get; set; } = PeakRank;
        public ValorantRank CurrentRank { get; set; } = CurrentRank;

        public string WinPercentage { get; set; } = WinPercentage;
        public string? HeadshotPercentage { get; set; } = HeadshotPercentage;
        public string? KdRatio { get; set; } = KdRatio;
        public string AverageDamagePerRound { get; set; } = AverageDamagePerRound;

        public bool ValidTracker { get; set; } = ValidTracker;

        public static ValorantTrackerStats Default()
        {
            return new ValorantTrackerStats(
                ValorantAgent.Default(), 
                AgentData.AgentClass.None,
                TrackerScore.Default(),
                "0%",
                "0%",
                "0",
                "0",
                ValorantRank.Default(), 
                ValorantRank.Default(),
                false
            );
        }
    }
}
