namespace ECAC_eSports_Scraper.DataTypes.ECAC
{
    public record TeamStats(double WinCount, double LossCount, double WinPercentage)
    {
        public double WinCount { get; set; } = WinCount;
        public double LossCount { get; set; } = LossCount;
        public double WinPercentage { get; set; } = WinPercentage;
    }
}
