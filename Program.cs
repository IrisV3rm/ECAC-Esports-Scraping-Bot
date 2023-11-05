using ECAC_eSports_Bot.Classes.ECACMethods;
using ECAC_eSports_Bot.Classes.SavingLoading;
using ECAC_eSports_Bot.DataTypes.ECAC;
using ECAC_eSports_Bot.DataTypes.GameAPIHandles;
using ECAC_eSports_Bot.DataTypes.GameTypes;
using ECAC_eSports_Bot.Methods;
// ReSharper disable ObjectCreationAsStatement
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS8604 // Possible null reference argument.

namespace ECAC_eSports_Bot
{
    internal class Program
    {
        private static bool _debugEnabled;
        public static void LogDebug(params string[] data)
        {
            if (!_debugEnabled) return;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [DEBUG] {string.Join(" ", data)}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void LogSuccess(params string[] data)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {string.Join(" ", data)}\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void LogInfo(params string[] data)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Log(data);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void LogError(params string[] data)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Log(data.Prepend("[ERROR] ").ToArray());
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Log(params string[] data)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {string.Join(" ", data)}");
        }

        public static async Task Initiate(IniFileParser configFile)
        {
            try
            {
                LogInfo("Creating authentication via ECAC...");
                GlobalProperties.EcacAuthorization = await EcacMethods.SignIn(
                    configFile.GetValue("ECAC", "Username"),
                    configFile.GetValue("ECAC", "Password")
                );
                while (string.IsNullOrEmpty(GlobalProperties.EcacAuthorization)) await Task.Delay(50);

                LogSuccess("Successfully authenticated!");

                LogInfo("Gathering home team...");
                Team? currentUserTeam = EcacMethods.GetCurrentUserTeam(GlobalGameData.Games.Valorant).Result;
                LogSuccess($"Found team as: {currentUserTeam?.Name}");

                LogInfo("Gathering enemy team...");
                Team? enemyTeam = EcacMethods.GetCurrentOpponent(GlobalGameData.Games.Valorant).Result;
                LogSuccess($"Found team as: {enemyTeam?.Name}");

                currentUserTeam!.ChannelId = ulong.Parse(configFile.GetValue("Discord", "TeamChannelId"));
                enemyTeam!.ChannelId = ulong.Parse(configFile.GetValue("Discord", "EnemyChannelId"));

                LogInfo("Gathering home team stats...");
                TeamHandler.FormatTeam(currentUserTeam);
                LogSuccess("Successfully gathered home stats!");
                LogInfo("Gathering enemy team stats...");
                TeamHandler.FormatTeam(enemyTeam);
                LogSuccess("Successfully gathered enemy stats!");

                LogInfo("Starting bot initiation...");
                new BotHandlerMain(
                    configFile.GetValue("Discord", "BotToken"),
                    currentUserTeam,
                    enemyTeam
                );

            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                Console.ReadLine();
            }
        }


        public static void Main()
        {
            LogInfo("Loading config parsers...");
            IniFileParser parser = new();
            parser.Load("_config.ini");

            GlobalProperties.EcacAccount = new EcacAccount(
                parser.GetValue("ECAC", "Username") ?? string.Empty,
                parser.GetValue("ECAC", "Password") ?? string.Empty,
                GlobalGameData.Games.Valorant,
                 GlobalApiHandles.GameApiType.TrackerGg
            );

            GlobalProperties.DiscordToken = parser.GetValue("Discord", "BotToken");
            
            LogInfo("Finished parsing config!");

            if (string.IsNullOrWhiteSpace(GlobalProperties.EcacAccount.Password) ||
                string.IsNullOrWhiteSpace(GlobalProperties.EcacAccount.Username) ||
                string.IsNullOrWhiteSpace(GlobalProperties.DiscordToken))
            {
                LogError($"Missing config settings, please set them: {AppDomain.CurrentDomain.BaseDirectory}_config.ini");
                Console.ReadLine();
                return;
            }

            _debugEnabled = bool.Parse(parser.GetValue("Debug", "DebugEnabled"));

            Initiate(parser);
            Task.Delay(-1);
            Console.ReadLine();
        }
    }
}