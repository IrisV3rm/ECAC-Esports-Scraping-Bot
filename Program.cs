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
        public static void Log(params string[] data)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {string.Join(" ", data)}");
        }

        public static async Task Initiate(IniFileParser configFile)
        {
            try
            {
                Log("Creating authentication via ECAC...");
                GlobalProperties.EcacAuthorization = await EcacMethods.SignIn(
                    configFile.GetValue("ECAC", "Username"),
                    configFile.GetValue("ECAC", "Password")
                );
                while (string.IsNullOrEmpty(GlobalProperties.EcacAuthorization)) await Task.Delay(50);

                Log("Successfully authenticated!");

                Log("Gathering home team...");
                Team? currentUserTeam = EcacMethods.GetCurrentUserTeam(GlobalGameData.Games.Valorant).Result;
                Log($"Found team as: {currentUserTeam?.Name}");

                Log("Gathering enemy team...");
                Team? enemyTeam = EcacMethods.GetCurrentOpponent(GlobalGameData.Games.Valorant).Result;
                Log($"Found team as: {currentUserTeam?.Name}");

                currentUserTeam!.ChannelId = ulong.Parse(configFile.GetValue("Discord", "TeamChannelId"));
                enemyTeam!.ChannelId = ulong.Parse(configFile.GetValue("Discord", "EnemyChannelId"));

                Log("Gathering home team stats...");
                TeamHandler.FormatTeam(currentUserTeam);
                Log("Successfully gathered home stats!");
                Log("Gathering enemy team stats...");
                TeamHandler.FormatTeam(enemyTeam);
                Log("Successfully gathered enemy stats!");

                Log("Starting bot initiation...");
                new BotHandlerMain(
                    configFile.GetValue("Discord", "BotToken"),
                    currentUserTeam,
                    enemyTeam
                );

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString()); Console.ForegroundColor = ConsoleColor.Red;
                Log(ex.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }


        public static void Main()
        {
            Log("Loading config parsers...");
            IniFileParser parser = new();
            parser.Load("_config.ini");

            GlobalProperties.EcacAccount = new EcacAccount(
                parser.GetValue("ECAC", "Username") ?? string.Empty,
                parser.GetValue("ECAC", "Password") ?? string.Empty,
                GlobalGameData.Games.Valorant,
                 GlobalApiHandles.GameApiType.TrackerGg
            );

            GlobalProperties.DiscordToken = parser.GetValue("Discord", "BotToken");
            
            Log("Finished parsing config!");

            if (string.IsNullOrWhiteSpace(GlobalProperties.EcacAccount.Password) ||
                string.IsNullOrWhiteSpace(GlobalProperties.EcacAccount.Username) ||
                string.IsNullOrWhiteSpace(GlobalProperties.DiscordToken))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Log($"Missing config settings, please set them: {AppDomain.CurrentDomain.BaseDirectory}_config.ini");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Initiate(parser);
            Task.Delay(-1);
            Console.ReadLine();
        }
    }
}