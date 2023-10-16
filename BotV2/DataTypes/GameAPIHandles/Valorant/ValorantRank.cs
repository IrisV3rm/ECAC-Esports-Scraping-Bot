namespace BotV2.DataTypes.GameAPIHandles.Valorant
{
    public class RankIcons
    {
        public static Dictionary<string, string> RankIcon = new()
        {
            { "Unranked", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Unranked.png" },
            { "Iron 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Iron 1.png" },
            { "Iron 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Iron 2.png" },
            { "Iron 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Iron 3.png" },
            { "Bronze 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Bronze 1.png" },
            { "Bronze 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Bronze 2.png" },
            { "Bronze 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Bronze 3.png" },
            { "Silver 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Silver 1.png" },
            { "Silver 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Silver 2.png" },
            { "Silver 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Silver 3.png" },
            { "Gold 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Gold 1.png" },
            { "Gold 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Gold 2.png" },
            { "Gold 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Gold 3.png" },
            { "Platinum 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Platinum 1.png" },
            { "Platinum 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Platinum 2.png" },
            { "Platinum 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Platinum 3.png" },
            { "Diamond 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Diamond 1.png" },
            { "Diamond 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Diamond 2.png" },
            { "Diamond 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Diamond 3.png" },
            { "Ascendant 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Ascendant 1.png" },
            { "Ascendant 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Ascendant 2.png" },
            { "Ascendant 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Ascendant 3.png" },
            { "Immortal 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Immortal 1.png" },
            { "Immortal 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Immortal 2.png" },
            { "Immortal 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Immortal 3.png" },
            { "Radiant", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Radiant.png" }
        };
    }

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
