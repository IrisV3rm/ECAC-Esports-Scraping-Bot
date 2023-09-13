using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.DataTypes.ECAC;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json.Linq;

namespace Bot.DataTypes
{
    public class TeamViewerHandler
    {
        internal User CurrentUserPage;

        public readonly string TeamIcon;
        public readonly string School;
        public readonly string TeamName;
        public readonly string CurrentGame;
        public readonly string WinCount;
        public readonly string LossCount;
        public readonly string WinPercent;
        public readonly List<User> Users;

        public TeamViewerHandler(string teamIcon, string school, string teamName, string currentGame, string winCount, string lossCount, string winPercent, List<User> users)
        {
            TeamIcon = teamIcon;
            School = school;
            TeamName = teamName;
            CurrentGame = currentGame;
            WinCount = winCount;
            LossCount = lossCount;
            WinPercent = winPercent;
            Users = users;
        }

        public async Task SendMessageWithComponents(DiscordChannel channel)
        {
            CurrentUserPage = Users.First();
            await channel.SendMessageAsync(
                new DiscordMessageBuilder().AddEmbeds(CurrentUserPage.DiscordEmbeds).AddComponents(
                    new DiscordButtonComponent(ButtonStyle.Primary, "PreviousPage", "Previous Page"),
                    new DiscordButtonComponent(ButtonStyle.Primary, "NextPage", "Next Page")
                )
            );
        }

        public async Task DoCommand(string enemyChannelId)
        {
            DiscordChannel enemyChannel = Program.Client.GetChannelAsync(ulong.Parse(enemyChannelId)).Result;
            EmbedCreator.CreateTeamEmbedData(this);
            await SendMessageWithComponents(enemyChannel);
            
            Program.Client.ComponentInteractionCreated += InteractionHandler;

        }

        private async Task InteractionHandler(DiscordClient sender, InteractionCreateEventArgs args)
        {
            Program.Log(args.Interaction.Data.CustomId);
            int nextUserIndex;
            int currentUserIndex = Users.IndexOf(CurrentUserPage);
            switch (args.Interaction.Data.CustomId)
            {
                case "NextPage":
                    nextUserIndex = currentUserIndex + 1 > Users.Count - 1 ? 0 : currentUserIndex + 1;
                    CurrentUserPage = Users[nextUserIndex];
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbeds(CurrentUserPage.DiscordEmbeds).AddComponents(
                        new DiscordButtonComponent(ButtonStyle.Primary, "PreviousPage", "Previous Page"),
                        new DiscordButtonComponent(ButtonStyle.Primary, "NextPage", "Next Page")
                    )));
                    break;
                case "PreviousPage":
                    nextUserIndex = currentUserIndex - 1 < 0 ? 0 : currentUserIndex - 1;
                    CurrentUserPage = Users[nextUserIndex];
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbeds(CurrentUserPage.DiscordEmbeds).AddComponents(
                        new DiscordButtonComponent(ButtonStyle.Primary, "PreviousPage", "Previous Page"),
                        new DiscordButtonComponent(ButtonStyle.Primary, "NextPage", "Next Page")
                    )));
                    break;
            }
        }
    }
}