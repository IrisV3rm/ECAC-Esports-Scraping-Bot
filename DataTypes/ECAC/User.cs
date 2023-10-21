using DSharpPlus.Entities;
using ECAC_eSports_Bot.DataTypes.GameAPIHandles.Valorant;

namespace ECAC_eSports_Bot.DataTypes.ECAC
{
    public record User(string? EcacName, string? RoleId, string? UserId, string? DiscordHandle)
    {

        public string? EcacName { get; set; } = EcacName;
        public string? RoleId { get; set; } = RoleId;
        public string? UserId { get; set; } = UserId;
        public string? DiscordHandle { get; set; } = DiscordHandle;
        public string? RiotId { get; set; }

        public ValorantRank? ValorantCurrentRank { get; set; }
        public ValorantRank? ValorantPeakRank { get; set; }
        public ValorantTrackerStats? TrackerStats { get; set; }

        public Classes.GameAPIMethods.TrackerGgCustomData? TrackerCustomGames { get; set; }

        public IEnumerable<DiscordEmbed>? DiscordEmbeds { get; set; }

        public void SetRiotId(string? riotId)
        {
            RiotId = riotId;
        }

        public void SetValorantCurrentRank(ValorantRank valorantRank)
        {
            ValorantCurrentRank = valorantRank;
        }

        public void SetValorantPeakRank(ValorantRank peakRank)
        {
            ValorantPeakRank = peakRank;
        }


        public static User Default()
        {
            User defaultUser = new("N/A", "N/A", "N/A", "N/A");
            defaultUser.SetRiotId("N/A");
            defaultUser.SetValorantCurrentRank(ValorantRank.Default());
            defaultUser.SetValorantPeakRank(ValorantRank.Default());
            return defaultUser;
        }
    }
}
