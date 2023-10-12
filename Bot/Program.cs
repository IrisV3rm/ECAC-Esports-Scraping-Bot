using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Threading;
using Bot.DataTypes;
using Bot.DataTypes.ECAC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DSharpPlus.SlashCommands;
using DSharpPlus;
// ReSharper disable ObjectCreationAsStatement

namespace Bot
{
    internal class Program
    {
        public static void Log(params string[] data)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {string.Join(" ", data)}");
        }

        public static DiscordClient Client;
        public static SlashCommandsExtension Slash;
        public static TeamViewerHandler TeamViewer;
        public static EnemyTeamViewerHandler EnemyTeamViewer;
        public static string TeamChannelId;
        public static string EnemyChannelId;

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
                string botToken = receiveData.Value<string>("discordToken");
                string localTeamId = receiveData.Value<string>("localTeamId");
                string enemyTeamId = receiveData.Value<string>("enemyTeamId");

                dynamic enemyTeamData = receiveData.Value<JToken>("enemyTeamViewerHandle");
                dynamic localTeamData = receiveData.Value<JToken>("localTeamViewerHandle");

                JToken enemyTeamMembers = enemyTeamData.CurrentTeam.Members;
                JToken localTeamMembers = localTeamData.CurrentTeam.Members;

                List<User> enemyTeamUsers = enemyTeamMembers.Select(enemyTeamMember =>
                    JsonConvert.DeserializeObject<User>(enemyTeamMember.ToString())).ToList();
                List<User> localTeamUsers = localTeamMembers.Select(localTeamMember =>
                    JsonConvert.DeserializeObject<User>(localTeamMember.ToString())).ToList();

                Log("Initiating Client...");
                
                new BotHandlerMain(
                    botToken,
                    new TeamViewerHandler(
                        localTeamData.CurrentTeam.LogoUrl.ToString(),
                        localTeamData.CurrentTeam.SchoolName.ToString(),
                        localTeamData.CurrentTeam.Name.ToString(),
                        localTeamData.CurrentTeam.Game.ToString(),
                        "N/A",
                        "N/A",
                        "N/A",
                        localTeamUsers),
                    new EnemyTeamViewerHandler(
                        enemyTeamData.CurrentTeam.LogoUrl.ToString(),
                        enemyTeamData.CurrentTeam.SchoolName.ToString(),
                        enemyTeamData.CurrentTeam.Name.ToString(),
                        enemyTeamData.CurrentTeam.Game.ToString(),
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
