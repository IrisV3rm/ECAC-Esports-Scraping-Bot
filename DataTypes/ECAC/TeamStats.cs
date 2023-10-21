namespace ECAC_eSports_Bot.DataTypes.ECAC
{
    public record TeamStats(double WinCount, double LossCount, double WinPercentage, User TeamCoach, User TeamCaptain)
    {
        public double WinCount { get; set; } = WinCount;
        public double LossCount { get; set; } = LossCount;
        public double WinPercentage { get; set; } = WinPercentage;
        public User TeamCoach { get; set; } = TeamCoach;
        public User TeamCaptain { get; set; } = TeamCaptain;

        public static TeamStats Default()
        {
            return new TeamStats(
                0.0,
                0.0,
                0.0,
                new User("REDACTED FOR TESTING", "REDACTED", "REDACTED FOR TESTING", "REDACTED FOR TESTING"),
                new User("REDACTED FOR TESTING", "REDACTED", "REDACTED FOR TESTING", "REDACTED FOR TESTING")
            );
        }
    }
}
