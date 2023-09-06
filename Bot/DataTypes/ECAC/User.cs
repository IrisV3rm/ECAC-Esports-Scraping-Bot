using System.Collections.Generic;
using Bot.DataTypes.GameAPIHandles.Valorant;
using DSharpPlus.Entities;

namespace Bot.DataTypes.ECAC
{
    public class User
    {
        public string EcacName { get; set; }
        public string RoleId { get; set; }
        public string UserId { get; set; }
        public string DiscordHandle { get; set; }
        public string RiotId { get; set; }

        public ValorantRank ValorantCurrentRank { get; set; }
        public ValorantRank ValorantPeakRank { get; set; }
        public ValorantTrackerStats TrackerStats { get; set; }

        public IEnumerable<DiscordEmbed> DiscordEmbeds { get; set; }

        public User(string name, string roleId, string userId, string discordHandle)
        {
            EcacName = name;
            RoleId = roleId;
            UserId = userId;
            DiscordHandle = discordHandle;
        }

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
