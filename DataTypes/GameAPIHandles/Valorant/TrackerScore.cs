﻿// ReSharper disable UnusedMember.Global
namespace ECAC_eSports_Bot.DataTypes.GameAPIHandles.Valorant
{
    public record TrackerScore(int Score, double WinPercentage, double Kast, double AverageCombatScore, double DdPerRound)
    {
        public int Score { get; set; } = Score;
        public double WinPercentage { get; set; } = WinPercentage;
        public double Kast { get; set; } = Kast;
        public double AverageCombatScore { get; set; } = AverageCombatScore;
        public double DdPerRound { get; set; } = DdPerRound;

        public static TrackerScore Default()
        {
            return new TrackerScore(0, 0.0, 0.0, 0.0, 0.0);
        }
    }
}
