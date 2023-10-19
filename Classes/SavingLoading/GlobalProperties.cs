using DSharpPlus.SlashCommands;
using DSharpPlus;
using ECAC_eSports_Bot.DataTypes.ECAC;
#pragma warning disable CA2211

namespace ECAC_eSports_Bot.Classes.SavingLoading
{
    public static class GlobalProperties
    {
        // Ini File Data

        public static EcacAccount? EcacAccount { get; set; }
        public static string? DiscordToken { get; set; }

        // End of Ini File Data

        public static DiscordClient Client = null!;
        public static SlashCommandsExtension Slash = null!;

        public static Team? TeamViewer = null!;
        public static Team? EnemyTeamViewer = null!;

        public static HttpClient MainClient = new();
        public static string? EcacAuthorization { get; set; }
    }
}
