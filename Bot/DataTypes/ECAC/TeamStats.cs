namespace Bot.DataTypes.ECAC
{
    public class TeamStats
    {
        public double WinCount { get; set; }
        public double LossCount { get; set; }
        public double WinPercentage { get; set; }

        public TeamStats(double winCount, double lossCount, double winPercentage)
        {
            WinCount = winCount;
            LossCount = lossCount;
            WinPercentage = winPercentage;
        }
    }
}
