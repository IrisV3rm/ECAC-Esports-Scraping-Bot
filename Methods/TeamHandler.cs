using ECAC_eSports_Bot.Classes.ECACMethods;
using ECAC_eSports_Bot.Classes.GameAPIMethods;
using ECAC_eSports_Bot.DataTypes.ECAC;
using ECAC_eSports_Bot.DataTypes.GameAPIHandles.Valorant;
using TrackerGgCustomData = ECAC_eSports_Bot.Classes.GameAPIMethods.TrackerGgCustomData;

namespace ECAC_eSports_Bot.Methods
{
    public class TeamHandler
    {
        public static void FormatTeam(Team? pendingTeam)
        {
            if (pendingTeam?.Members == null) {Program.Log("Cannot find members.");return;}

            foreach (User user in pendingTeam.Members)
            {
                string? linkedHandle = EcacMethods.GetLinkedHandleByType(user.UserId, EcacMethods.HandleTypes.Valorant).Result;

                user.TrackerStats = TrackerGg.GetStats(user.RiotId).Result;

                if (!TrackerGg.IsValidUser(user.RiotId, false).Result || user.TrackerStats?.KdRatio is null)
                    user.TrackerStats = TrackerGg.GetDefaultTrackerStats();
                    
                if (!TrackerGg.IsValidUser(user.RiotId, false).Result || user.TrackerCustomGames?.Matches?.Count is null)
                    user.TrackerCustomGames = TrackerGgCustomData.DefaultStats();
                else
                    user.TrackerCustomGames = TrackerGgCustomData.GetAndParseData(user.RiotId, DateTime.MinValue).Result;

                if (user.ValorantCurrentRank?.Rank is null)
                    user.ValorantCurrentRank = user.TrackerStats.CurrentRank;
                if (user.ValorantPeakRank?.Rank is null)
                    user.ValorantPeakRank = user.TrackerStats.PeakRank;

                ValorantRank currentRank = user.ValorantCurrentRank;
                ValorantRank peakRank = user.ValorantPeakRank;

                user.SetRiotId(linkedHandle);
                user.SetValorantCurrentRank(currentRank);
                user.SetValorantPeakRank(peakRank);
            }

        }
    }
}
