namespace ECAC_eSports_Bot.DataTypes.ECAC
{
    public record TeamStats(double WinCount, double LossCount, double WinPercentage)
    {
        public double WinCount { get; set; } = WinCount;
        public double LossCount { get; set; } = LossCount;
        public double WinPercentage { get; set; } = WinPercentage;
    }
}
