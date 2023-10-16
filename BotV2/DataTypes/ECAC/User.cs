using BotV2.DataTypes.GameAPIHandles.Valorant;
using DSharpPlus.Entities;

namespace BotV2.DataTypes.ECAC
{
    public record User(string EcacName, string RoleId, string UserId, string DiscordHandle, string RiotId)
    {
        public string EcacName { get; set; } = EcacName;
        public string RoleId { get; set; } = RoleId;
        public string UserId { get; set; } = UserId;
        public string DiscordHandle { get; set; } = DiscordHandle;
        public string RiotId { get; set; } = RiotId;

        public required ValorantRank ValorantCurrentRank { get; set; }
        public required ValorantRank ValorantPeakRank { get; set; }
        public required ValorantTrackerStats TrackerStats { get; set; }

        public required TrackerGgCustomData TrackerCustomGames { get; set; }

        public required IEnumerable<DiscordEmbed> DiscordEmbeds { get; set; }

        public void SetRiotId(string riotId)
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
    }
}
