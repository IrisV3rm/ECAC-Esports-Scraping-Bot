using ECAC_eSports_Scraper.DataTypes.GameAPIHandles;
using ECAC_eSports_Scraper.DataTypes.GameTypes;

namespace ECAC_eSports_Scraper.DataTypes.ECAC
{
    public class EcacAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public GlobalGameData.Games GameType { get; set; }
        public GlobalApiHandles.GameApiType ApiType { get; set; }

        public EcacAccount(string username, string password, GlobalGameData.Games gameType, GlobalApiHandles.GameApiType apiType)
        {
            Username = username;
            Password = password;
            GameType = gameType;
            ApiType = apiType;
        }
    }

}
