﻿using System.Diagnostics.CodeAnalysis;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ECAC_eSports_Bot.Classes.SavingLoading;
using ECAC_eSports_Bot.DataTypes.ECAC;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace ECAC_eSports_Bot.Methods
{
    public class BotHandlerMain
    {
        public BotHandlerMain(string? discordToken, Team? teamViewer, Team? enemyTeamViewer)
        {
            try
            {
                Program.Log("Starting client instance...");
                Program.Log($"Token found: {!string.IsNullOrEmpty(discordToken)}");
                GlobalProperties.Client = new DiscordClient(new DiscordConfiguration
                {
                    Token = discordToken,
                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.AllUnprivileged,
                    MinimumLogLevel = LogLevel.Information,
                    LogTimestampFormat = "yyyy-MM-dd HH:mm:ss"
                });
                
                Program.Log("Registering slash commands...");
                GlobalProperties.Slash = GlobalProperties.Client.UseSlashCommands();
                GlobalProperties.TeamViewer = teamViewer;
                GlobalProperties.EnemyTeamViewer = enemyTeamViewer;
              
                Program.Log("Registering internal slash commands...");
                GlobalProperties.Slash.RegisterCommands<BotCommands>();
                
                Program.Log("Connecting to discord backend...");

                GlobalProperties.Client.Ready += async delegate
                {
                    await teamViewer.DoCommand();
                    await enemyTeamViewer.DoCommand();
                };
                
                GlobalProperties.Client.ConnectAsync(new DiscordActivity("ECAC eSports", ActivityType.Competing), UserStatus.Online);
                Console.ReadLine();
                Task.Delay(-1);
            }
            catch (Exception ex) { Program.Log(ex.ToString()); }

        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public class BotCommands : ApplicationCommandModule
        {
            [SlashCommand("refreshhome", "Re-grabs the home team data.")]
            public async Task RefreshHomeTeamCommand(InteractionContext ctx)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
                await GlobalProperties.TeamViewer.DoCommand();
            }

            [SlashCommand("refreshenemy", "Re-grabs the enemy team data.")]
            public async Task RefreshOpponentTeamCommand(InteractionContext ctx)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
                await GlobalProperties.EnemyTeamViewer.DoCommand();
            }


        }
    }
}