namespace Bot.DataTypes.GameAPIHandles.Valorant
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
}
