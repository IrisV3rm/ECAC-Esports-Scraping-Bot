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
            if (pendingTeam?.Members == null) {Program.LogError("Cannot find members.");return;}

            Program.LogInfo($"Parsing {pendingTeam.Name} members...");

            foreach (User user in pendingTeam.Members)
            {
                string? linkedHandle = EcacMethods.GetLinkedHandleByType(user.UserId, EcacMethods.HandleTypes.Valorant).Result;

                Program.LogInfo($"Parsing: {user.EcacName} | {linkedHandle}");

                user.SetRiotId(linkedHandle);

                user.TrackerStats = TrackerGg.GetStats(user.RiotId).Result;

                if (!user.TrackerStats.ValidTracker || user.TrackerStats?.KdRatio is null)
                    user.TrackerStats = ValorantTrackerStats.Default();

                //user.TrackerCustomGames = user.TrackerStats.ValidTracker
                //    ? TrackerGgCustomData.GetAndParseData(user.RiotId, DateTime.MinValue).Result
                //    : TrackerGgCustomData.Default();

                user.TrackerCustomGames = TrackerGgCustomData.Default();


                if (user.ValorantCurrentRank?.Rank is null)
                    user.ValorantCurrentRank = user.TrackerStats.CurrentRank;
                if (user.ValorantPeakRank?.Rank is null)
                    user.ValorantPeakRank = user.TrackerStats.PeakRank;

                user.SetValorantCurrentRank(user.ValorantCurrentRank);
                user.SetValorantPeakRank(user.ValorantPeakRank);
            }

            Program.LogInfo($"Successfully parsed {pendingTeam.Name}");
        }
    }
}
