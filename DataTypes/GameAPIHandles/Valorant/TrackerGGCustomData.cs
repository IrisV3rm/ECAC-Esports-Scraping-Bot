using static ECAC_eSports_Bot.DataTypes.GameAPIHandles.Valorant.MatchDataType;

namespace ECAC_eSports_Bot.DataTypes.GameAPIHandles.Valorant
{
    public class MatchDataType
    {
        public record Match(MatchData MatchData, MapWinPercentage MatchWinPercentage)
        {
            public MatchData MatchData { get; set; } = MatchData;
            public MapWinPercentage MatchWinPercentage { get; set; } = MatchWinPercentage;
        }

        public record MatchData(string MapName, string Result, string AgentPlayed, int? Kills, int? Deaths, int? Assists)
        {
            public string MapName { get; set; } = MapName;
            public string Result { get; set; } = Result;
            public string AgentPlayed { get; set; } = AgentPlayed;
            public int? Kills { get; set; } = Kills;
            public int? Deaths { get; set; } = Deaths;
            public int? Assists { get; set; } = Assists;
        }

        public record MapWinPercentage(string MapName, double WinPercentage)
        {
            public string MapName { get; set; } = MapName;
            public double WinPercentage { get; set; } = WinPercentage;
        }

    }

    public class TrackerGgCustomData
    {
        public List<Match> Matches { get; }
        public TrackerGgCustomData(List<Match> matches)
        {
            Matches = matches;
        }

        public static TrackerGgCustomData DefaultStats()
        {
            return new TrackerGgCustomData(
                new List<Match>
                {
                    new Match(
                        new MatchData(
                        "N/A",
                        "N/A",
                        "Sage",
                        0,
                        0,
                        0
                    ),
                    new MapWinPercentage(
                        "N/A",
                        0.0
                        )
                    )
                });
        }
    }
}
