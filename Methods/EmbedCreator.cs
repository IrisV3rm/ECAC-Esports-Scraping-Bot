﻿using DSharpPlus.Entities;
using ECAC_eSports_Bot.Classes.GameAPIMethods;
using ECAC_eSports_Bot.DataTypes.ECAC;
using ECAC_eSports_Bot.DataTypes.GameAPIHandles.Valorant;
#pragma warning disable CS8604 // Possible null reference argument.

namespace ECAC_eSports_Bot.Methods
{
    public class EmbedCreator
    {
        internal static readonly DiscordColor SchoolColor = new(92, 125, 138);
        internal static readonly DiscordColor RankColor = new(160, 91, 180);
        internal static readonly DiscordColor PinkColor = new(245, 37, 101);

        internal static string GetTrackerUrl(string? riotName)
        {
            return $"https://tracker.gg/valorant/profile/riot/{riotName?.Replace(" ", "%20").Replace("#", "%23")}";
        }

        internal static void ParseUsers(IEnumerable<User> users, string? teamName, string? teamIcon)
        {
            foreach (User user in users)
            {
                string trackerStatus = user.TrackerStats != null && user.TrackerStats.ValidTracker ? ":white_check_mark:" : ":x:";

                DiscordEmbedBuilder school = new DiscordEmbedBuilder()
                    .WithUrl("https://discord.gg/")
                    .WithAuthor(teamName, null, teamIcon)
                    .WithColor(SchoolColor);

                DiscordEmbedBuilder accountNames = new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.VeryDarkGray)
                    .WithAuthor("Account Names", null, "https://irisapp.ca/api/Dev/ecac-scraping/riot-games.png")
                    .AddField("ECAC Name", $"__**{user.EcacName}**__", true)
                    .AddField("Riot Name", $"__**{user.RiotId}**__", true);

                DiscordEmbedBuilder accountRanks = new DiscordEmbedBuilder()
                    .WithAuthor("Account Rankings", null, "https://irisapp.ca/api/Dev/ecac-scraping/valorant-hexagon.png")
                    .WithColor(RankColor)
                    .AddField("Current Rank", user.ValorantCurrentRank?.Rank, true)
                    .AddField("Peak Rank", user.ValorantPeakRank?.Rank, true)
                    .WithThumbnail(RankIcons.RankIcon[user.ValorantCurrentRank?.Rank], 35, 35);

                DiscordEmbedBuilder trackerStats = new DiscordEmbedBuilder()
                    .WithAuthor("User Tracker Statistics", null, "https://irisapp.ca/api/Dev/ecac-scraping/trackerNet.png")
                    .WithColor(PinkColor)
                    .AddField("Win Percentage", user.TrackerStats?.WinPercentage, true);

                DiscordEmbedBuilder trackerStats2 = new DiscordEmbedBuilder()
                    .WithColor(PinkColor)
                    .AddField("Average KD", user.TrackerStats?.KdRatio, true)
                    .AddField("Headshot Percentage", user.TrackerStats?.HeadshotPercentage, true);

                DiscordEmbedBuilder trackerStats3 = new DiscordEmbedBuilder()
                    .WithColor(PinkColor)
                    .AddField("Top Competitive Agent", user.TrackerStats?.TopAgent?.Name, true)
                    .AddField("Agent Type", user.TrackerStats?.TopAgent?.Role.ToString(), true)
                    .WithFooter("", AgentData.RoleIcon[
                        (AgentData.AgentClass)Enum.Parse(
                            typeof(AgentData.AgentClass),
                            user.TrackerStats?.TopAgent?.Role.ToString()
                        )
                    ])
                    .WithThumbnail(AgentData.AgentHeadshot[
                        (AgentData.Agent)Enum.Parse(
                            typeof(AgentData.Agent),
                            user.TrackerStats.TopAgent.Name
                        )
                    ],35,35);

                //DiscordEmbedBuilder trackerStats4 = new DiscordEmbedBuilder()
                //    .WithColor(PinkColor)
                //    .WithTitle("Custom's Data")
                //    .AddField("Top Agent", user.TrackerCustomGames?.MostUsedAgent.Name, true)
                //    .AddField("Top Agent Class", user.TrackerCustomGames?.MostUsedAgent.Role.ToString(), true)
                //    .WithFooter("", AgentData.RoleIcon[
                //        (AgentData.AgentClass)Enum.Parse(
                //            typeof(AgentData.AgentClass),
                //            user.TrackerCustomGames?.MostUsedAgent.Role.ToString()
                //        )
                //    ])
                //    .WithThumbnail(AgentData.AgentHeadshot[
                //        (AgentData.Agent)Enum.Parse(
                //            typeof(AgentData.Agent),
                //            user.TrackerCustomGames.MostUsedAgent.Name
                //        )
                //    ], 35, 35);

                DiscordEmbedBuilder trackerStats5 = new DiscordEmbedBuilder()
                    .WithColor(PinkColor)
                    .WithDescription($"**Has valid tracker page:**: {trackerStatus}")
                    .WithAuthor("Tracker Profile", GetTrackerUrl(user.RiotId), "https://irisapp.ca/api/Dev/ecac-scraping/valorant-hexagon.png");

                DiscordEmbed schoolEmbed = school.Build();
                DiscordEmbed accountNameEmbed = accountNames.Build();
                DiscordEmbed accountRanksEmbed = accountRanks.Build();
                DiscordEmbed trackerStatsEmbed = trackerStats.Build();
                DiscordEmbed trackerStatsEmbed2 = trackerStats2.Build();
                DiscordEmbed trackerStatsEmbed3 = trackerStats3.Build();
                //DiscordEmbed trackerStatsEmbed4 = trackerStats4.Build();
                DiscordEmbed trackerStatsEmbed5 = trackerStats5.Build();

                user.DiscordEmbeds = new List<DiscordEmbed>
                {
                    schoolEmbed,
                    accountNameEmbed,
                    accountRanksEmbed,
                    trackerStatsEmbed,
                    trackerStatsEmbed2,
                    trackerStatsEmbed3,
                    //trackerStatsEmbed4,
                    trackerStatsEmbed5
                };
            }
        }

        public static void CreateTeamEmbedData(Team team) => ParseUsers(team.Members, team.Name, team.LogoUrl);
    }
}
