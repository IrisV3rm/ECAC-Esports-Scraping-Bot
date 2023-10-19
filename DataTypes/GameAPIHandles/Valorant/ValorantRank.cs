#pragma warning disable CA2211
namespace ECAC_eSports_Bot.DataTypes.GameAPIHandles.Valorant;

public class RankIcons
{
    public static Dictionary<string, string> RankIcon = new()
    {
        { "Unranked", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Unranked.png" },
        { "Iron 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Iron%201.png" },
        { "Iron 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Iron%202.png" },
        { "Iron 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Iron%203.png" },
        { "Bronze 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Bronze%201.png" },
        { "Bronze 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Bronze%202.png" },
        { "Bronze 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Bronze%203.png" },
        { "Silver 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Silver%201.png" },
        { "Silver 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Silver%202.png" },
        { "Silver 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Silver%203.png" },
        { "Gold 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Gold%201.png" },
        { "Gold 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Gold%202.png" },
        { "Gold 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Gold%203.png" },
        { "Platinum 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Platinum%201.png" },
        { "Platinum 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Platinum%202.png" },
        { "Platinum 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Platinum%203.png" },
        { "Diamond 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Diamond%201.png" },
        { "Diamond 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Diamond%202.png" },
        { "Diamond 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Diamond%203.png" },
        { "Ascendant 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Ascendant%201.png" },
        { "Ascendant 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Ascendant%202.png" },
        { "Ascendant 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Ascendant%203.png" },
        { "Immortal 1", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Immortal%201.png" },
        { "Immortal 2", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Immortal%202.png" },
        { "Immortal 3", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Immortal%203.png" },
        { "Radiant", "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Ranks/Radiant.png" }
    };
}

public record ValorantRank(string? Rank, dynamic? RankIcon)
{
    public string? Rank { get; set; } = Rank;

    public static ValorantRank Default()
    {
        return new ValorantRank("Unranked", "https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png");
    }
}