using System.Collections.Generic;
using ECAC_eSports_Scraper.Classes.GameAPIMethods;
using ECAC_eSports_Scraper.DataTypes.GameAPIHandles.Valorant;

namespace ECAC_eSports_Scraper.DataTypes.ECAC
{
    public record User(string EcacName, string RoleId, string UserId, string DiscordHandle)
    {
        public string EcacName { get; set; } = EcacName;
        public string RoleId { get; set; } = RoleId;
        public string UserId { get; set; } = UserId;
        public string DiscordHandle { get; set; } = DiscordHandle;
        public string RiotId { get; set; }

        public ValorantRank ValorantCurrentRank { get; set; }
        public ValorantRank ValorantPeakRank { get; set; }
        public ValorantTrackerStats TrackerStats { get; set; }
        public List<MatchDataType.Match> TrackerCustomGames { get; set; }

        //public IEnumerable<Embed> DiscordEmbeds { get; set; }

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
