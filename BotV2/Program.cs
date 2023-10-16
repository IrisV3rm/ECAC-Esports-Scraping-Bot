using System.Diagnostics;
using System.IO.Pipes;
using BotV2.DataTypes;
using BotV2.DataTypes.ECAC;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable ObjectCreationAsStatement
#pragma warning disable CA1806

namespace BotV2
{
    internal class Program
    {
        public static void Log(params string[] data)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {string.Join(" ", data)}");
        }

        public static DiscordClient Client = null!;
        public static SlashCommandsExtension Slash = null!;
        public static TeamViewerHandler TeamViewer = null!;
        public static EnemyTeamViewerHandler EnemyTeamViewer = null!;
        public static string? TeamChannelId = null!;
        public static string? EnemyChannelId = null!;

        static void Main(string[] args)
        {
            try
            {
                Log("Bot process created, logging parent process...");
                Process process = Process.GetProcessById(int.Parse(args.First()));
                process.EnableRaisingEvents = true;
                process.Exited += delegate { Process.GetCurrentProcess().Kill(); };

                JToken receiveData;

                Log("Opening communication pipe...");
                using (NamedPipeServerStream pipeServer = new("ECAC_BOT_PIPE", PipeDirection.InOut))
                {
                    Console.WriteLine("READY");
                    pipeServer.WaitForConnection();
                    StreamReader reader = new(pipeServer);
                    receiveData = JToken.Parse(reader.ReadToEnd());
                }

                while (true)
                {
                    Log("Waiting for data...");
                    if (receiveData is { HasValues: true }) break;
                    Thread.Sleep(250);
                }

                Log("Extracting JSON var...");
                string? botToken = receiveData.Value<string>("discordToken");
                string? localTeamId = receiveData.Value<string>("localTeamId");
                string? enemyTeamId = receiveData.Value<string>("enemyTeamId");

                dynamic? enemyTeamData = receiveData.Value<JToken>("enemyTeamViewerHandle");
                dynamic? localTeamData = receiveData.Value<JToken>("localTeamViewerHandle");

                JToken? enemyTeamMembers = enemyTeamData?.CurrentTeam.Members;
                JToken? localTeamMembers = localTeamData?.CurrentTeam.Members;

                List<User?>? enemyTeamUsers = enemyTeamMembers?.Select(enemyTeamMember =>
                    JsonConvert.DeserializeObject<User>(enemyTeamMember.ToString())).ToList();
                List<User?>? localTeamUsers = localTeamMembers?.Select(localTeamMember =>
                    JsonConvert.DeserializeObject<User>(localTeamMember.ToString())).ToList();

                Log("Initiating Client...");

                new BotHandlerMain(
                    botToken,
                    new TeamViewerHandler(
                        localTeamData?.CurrentTeam.LogoUrl.ToString(),
                        localTeamData?.CurrentTeam.SchoolName.ToString(),
                        localTeamData?.CurrentTeam.Name.ToString(),
                        localTeamData?.CurrentTeam.Game.ToString(),
                        "N/A",
                        "N/A",
                        "N/A",
                        localTeamUsers),
                    new EnemyTeamViewerHandler(
                        enemyTeamData?.CurrentTeam.LogoUrl.ToString(),
                        enemyTeamData?.CurrentTeam.SchoolName.ToString(),
                        enemyTeamData?.CurrentTeam.Name.ToString(),
                        enemyTeamData?.CurrentTeam.Game.ToString(),
                        "N/A",
                        "N/A",
                        "N/A",
                        enemyTeamUsers),
                    localTeamId,
                    enemyTeamId
                );

                process.WaitForExit();
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
