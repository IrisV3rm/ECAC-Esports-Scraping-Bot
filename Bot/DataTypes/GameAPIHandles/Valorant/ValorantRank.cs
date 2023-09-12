using System.Collections.Generic;

namespace Bot.DataTypes.GameAPIHandles.Valorant
{
    public record ValorantRank(string Rank, dynamic RankIcon)
    {
        public static Dictionary<string, string> AbbreviatedRanks = new ()
        {
            { "Unranked", "Unr" },
            { "Iron 1", "Ir1" },
            { "Iron 2", "Ir2" },
            { "Iron 3", "Ir3" },
            { "Bronze 1", "B1" },
            { "Bronze 2", "B2" },
            { "Bronze 3", "B3" },
            { "Silver 1", "S1" },
            { "Silver 2", "S2" },
            { "Silver 3", "S3" },
            { "Gold 1", "G1" },
            { "Gold 2", "G2" },
            { "Gold 3", "G3" },
            { "Platinum 1", "P1" },
            { "Platinum 2", "P2" },
            { "Platinum 3", "P3" },
            { "Diamond 1", "D1" },
            { "Diamond 2", "D2" },
            { "Diamond 3", "D3" },
            { "Ascendant 1", "Asc1" },
            { "Ascendant 2", "Asc2" },
            { "Ascendant 3", "Asc3" },
            { "Immortal 1", "Imm1" },
            { "Immortal 2", "Imm2" },
            { "Immortal 3", "Imm3" },
            { "Radiant", "Rad" }
        };
        

        public string Rank { get; set; } = Rank;
        public dynamic RankIcon { get; set; } = RankIcon;
        public string SmallerRank { get; set; } = AbbreviatedRanks[Rank];
    }
}
