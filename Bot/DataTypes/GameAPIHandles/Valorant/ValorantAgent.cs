namespace Bot.DataTypes.GameAPIHandles.Valorant
{
    public class ValorantAgent
    {
        public enum ValorantRole
        {
            Duelist,
            Sentinel,
            Controller,
            Initiator,
        }

        public string Name { get; set; }
        public ValorantRole Role { get; set; }
        public dynamic Avatar { get; set; }
        public double HoursPlayed { get; set; }
        public double WinPercentage { get; set; }
        public double KdRatio { get; set; }

        public ValorantAgent(string name, ValorantRole role, dynamic avatar, double hoursPlayed, double winPercentage, double kdRatio)
        {
            Name = name;
            Role = role;
            Avatar = avatar;
            HoursPlayed = hoursPlayed;
            WinPercentage = winPercentage;
            KdRatio = kdRatio;
        }
    }
}
