using ECAC_eSports.DataTypes.GameAPIHandles;
using ECAC_eSports.DataTypes.GameTypes;

namespace ECAC_eSports.DataTypes.ECAC
{
    public record EcacAccount(string Username, string Password, GlobalGameData.Games GameType, GlobalApiHandles.GameApiType ApiType)
    {
        public string Username { get; set; } = Username;
        public string Password { get; set; } = Password;

        public GlobalGameData.Games GameType { get; set; } = GameType;
        public GlobalApiHandles.GameApiType ApiType { get; set; } = ApiType;
    }

}
