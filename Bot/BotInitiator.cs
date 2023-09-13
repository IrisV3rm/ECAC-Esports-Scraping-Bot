﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bot.DataTypes;
using Microsoft.Extensions.Logging;

namespace Bot
{
    public class BotHandlerMain
    {
        public BotHandlerMain(string discordToken, TeamViewerHandler teamViewer, EnemyTeamViewerHandler enemyTeamViewer, string teamChannelId, string enemyChannelId)
        {
            try
            {
                Program.Log("Starting client instance...");
                Program.Log($"Token data: {discordToken}");
                Program.Client = new DiscordClient(new DiscordConfiguration
                {
                    Token = discordToken,
                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.AllUnprivileged,
                    MinimumLogLevel = LogLevel.Trace
                });

                Program.Log("Registering slash commands...");
                Program.Slash = Program.Client.UseSlashCommands();
                Program.TeamViewer = teamViewer;
                Program.EnemyTeamViewer = enemyTeamViewer;
                Program.TeamChannelId = teamChannelId;
                Program.EnemyChannelId = enemyChannelId;

                Program.Log("Registering internal slash commands...");
                Program.Slash.RegisterCommands<BotCommands>();
                
                Program.Log("Connecting to discord backend...");

                Program.Client.Ready += async delegate
                {
                    await teamViewer.DoCommand(teamChannelId);
                    await enemyTeamViewer.DoCommand(enemyChannelId);
                };

                Program.Client.ConnectAsync(new DiscordActivity("ECAC eSports", ActivityType.Competing), UserStatus.Online);

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
                await Program.TeamViewer.DoCommand(Program.TeamChannelId);
            }

            [SlashCommand("refreshenemy", "Re-grabs the enemy team data.")]
            public async Task RefreshOpponentTeamCommand(InteractionContext ctx)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
                await Program.EnemyTeamViewer.DoCommand(Program.EnemyChannelId);
            }


        }
    }
}