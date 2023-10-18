using ECAC_eSports_Bot.DataTypes.GameAPIHandles;
using ECAC_eSports_Bot.DataTypes.GameTypes;

namespace ECAC_eSports_Bot.DataTypes.ECAC
{
    public record EcacAccount(string Username, string Password, GlobalGameData.Games GameType, GlobalApiHandles.GameApiType ApiType)
    {
        public string Username { get; set; } = Username;
        public string Password { get; set; } = Password;

        public GlobalGameData.Games GameType { get; set; } = GameType;
        public GlobalApiHandles.GameApiType ApiType { get; set; } = ApiType;
    }

}
