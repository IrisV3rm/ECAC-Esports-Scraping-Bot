namespace Bot.DataTypes.GameAPIHandles.Valorant
{
    public class TrackerScore
    {
        public int Score { get; set; }
        public double WinPercentage { get; set; }
        public double Kast { get; set; }
        public double AverageCombatScore { get; set; }
        public double DdPerRound { get; set; }

        public TrackerScore(int score, double winPercentage, double kAst, double averageCombatScore, double dDPerRound)
        {
            Score = score;
            WinPercentage = winPercentage;
            Kast = kAst;
            AverageCombatScore = averageCombatScore;
            DdPerRound = dDPerRound;
        }
    }
}
