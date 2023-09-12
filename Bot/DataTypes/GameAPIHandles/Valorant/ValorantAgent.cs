namespace Bot.DataTypes.GameAPIHandles.Valorant
{
    public record ValorantAgent(string Name, ValorantAgent.ValorantRole Role, dynamic Avatar, double HoursPlayed, double WinPercentage, double KdRatio)
    {
        public enum ValorantRole
        {
            Duelist,
            Sentinel,
            Controller,
            Initiator,
        }

        public string Name { get; set; } = Name;
        public ValorantRole Role { get; set; } = Role;
        public dynamic Avatar { get; set; } = Avatar;
        public double HoursPlayed { get; set; } = HoursPlayed;
        public double WinPercentage { get; set; } = WinPercentage;
        public double KdRatio { get; set; } = KdRatio;
    }
}
