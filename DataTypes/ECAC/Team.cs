using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using ECAC_eSports_Bot.Classes.SavingLoading;
using ECAC_eSports_Bot.DataTypes.GameTypes;
using ECAC_eSports_Bot.Methods;

namespace ECAC_eSports_Bot.DataTypes.ECAC
{
    public record Team(string? Id, string? LogoUrl, string? Name, List<User> Members, string? SchoolName, GlobalGameData.Games Game, TeamStats Stats)
    {
        internal User CurrentUserPage = null!;

        public string? Id { get; set; } = Id;
        public string? LogoUrl { get; set; } = LogoUrl;
        public string? Name { get; set; } = Name;
        public string? SchoolName { get; set; } = SchoolName;
        public List<User> Members { get; set; } = Members;
        public TeamStats Stats { get; set; } = Stats;
        public ulong ChannelId { get; set; }

/*
        public GlobalGameData.Games Game { get; set; } = Game;
*/

        public async Task SendMessageWithComponents(DiscordChannel channel)
        {
            Console.WriteLine("DOING HOME");
            User homePage = new ("N/A", "N/A", "N/A", "N/A");
            DiscordEmbedBuilder schoolStats = new DiscordEmbedBuilder()
                .WithAuthor($"{SchoolName} | {Name}", "", "https://ecac.leaguespot.gg/static/media/leagues/ecac/ecac-logo.png")
                .AddField("__Win Count__", $"**{Stats.WinCount}**", true)
                .AddField("__Loss Count__", $"**{Stats.LossCount}**", true)
                .AddField("__Win Percent__", $"**{Stats.WinPercentage}%**")
                .AddField("__Coach Riot Id__", $"``{Stats.TeamCoach.RiotId}``", true)
                .AddField("__Coach Discord Tag__", $"``{Stats.TeamCoach.DiscordHandle}``", true)
                .AddField("\u200b", "\u200b")
                .AddField("__Captain Riot Id__", $"``{Stats.TeamCaptain.RiotId}``", true)
                .AddField("__Captain Discord Tag__", $"``{Stats.TeamCaptain.DiscordHandle}``", true)
                .WithThumbnail(LogoUrl)
                .WithFooter("Developed By iri.s", "https://cdn.discordapp.com/avatars/465727038305599500/a_c9dab76a68ced007428738870755b3d4.gif")
                .WithTimestamp(DateTime.Now); // Adjust the timestamp as needed

            homePage.DiscordEmbeds = new List<DiscordEmbed>{schoolStats.Build()};
            Members.Insert(0, homePage);
            CurrentUserPage = Members.First();
            Console.WriteLine("DOING SEND SEND SEND");
            await channel.SendMessageAsync(
                new DiscordMessageBuilder().AddEmbeds(CurrentUserPage.DiscordEmbeds).AddComponents(
                    new DiscordButtonComponent(ButtonStyle.Primary, $"PreviousPage_{Name!.Replace(" ", "")}", "Previous Page"),
                    new DiscordButtonComponent(ButtonStyle.Primary, $"NextPage_{Name!.Replace(" ", "")}", "Next Page")
                )
            );
        }

        public async Task DoCommand()
        {
            Console.WriteLine("DOING COMMAND");
            DiscordChannel channel = GlobalProperties.Client.GetChannelAsync(ChannelId).Result;
            Console.WriteLine("DOING CHANNEL");
            EmbedCreator.CreateTeamEmbedData(this);
            Console.WriteLine("DOING MESSAGE");
            await SendMessageWithComponents(channel);

            GlobalProperties.Client.ComponentInteractionCreated += InteractionHandler;
        }

        private async Task InteractionHandler(DiscordClient sender, InteractionCreateEventArgs args)
        {
            if (args.Interaction.ChannelId != ChannelId) return;
            
            int currentUserIndex = Members.IndexOf(CurrentUserPage);
            int nextUserIndex = args.Interaction.Data.CustomId.Contains("NextPage") ? currentUserIndex + 1 > Members.Count - 1 ? 0 : currentUserIndex + 1 : currentUserIndex - 1 < 0 ? 0 : currentUserIndex - 1;

            CurrentUserPage = Members[nextUserIndex];
            await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbeds(CurrentUserPage.DiscordEmbeds).AddComponents(
                new DiscordButtonComponent(ButtonStyle.Primary, $"PreviousPage_{Name!.Replace(" ", "")}", "Previous Page"),
                new DiscordButtonComponent(ButtonStyle.Primary, $"NextPage_{Name!.Replace(" ", "")}", "Next Page")
            )));
        }
    }
}

