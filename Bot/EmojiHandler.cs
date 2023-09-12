using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.DataTypes.GameAPIHandles.Valorant;
using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.EventArgs;

namespace Bot
{
    internal class EmojiHandler
    {
        public static async Task<Dictionary<string, ulong>> ParseServerEmojis()
        {
            DiscordGuild mainGuild = Program.Client.Guilds.Values.First();
            foreach (DiscordEmoji emoji in mainGuild.Emojis.Values)
            {
            }
            return new Dictionary<string, ulong>();
        }

        public static async Task UploadEmojis()
        {
            Dictionary<string, Stream> emojiStreams = new();

            foreach (string file in Directory.GetFiles(Program.ImageAssetLocation + "\\ValorantAssets", "", SearchOption.AllDirectories))
            {
                Program.Log($"Creating stream for: {Path.GetFileName(file)}");
                string assetName = Path.GetFileNameWithoutExtension(file);
                Stream assetStream = new FileStream(file, FileMode.Open);
                emojiStreams.Add(assetName, assetStream);
            }

            foreach (KeyValuePair<string, Stream> data in emojiStreams)
            {
                Program.Log($"Uploading emoji: {data.Key}");
                await Program.Client.Guilds.Values.First().CreateEmojiAsync(data.Key, data.Value, null,
                    "{ ECAC BOT } Automated emoji building.");
                Program.Log($"Upload complete for emoji: {data.Key}");
            }
        }

        public static Dictionary<string, ValorantRank> ParseUploadedEmojis()
        {
            return new Dictionary<string, ValorantRank>();
        }

        private async Task InteractionHandler(DiscordClient sender, InteractionCreateEventArgs args)
        {
            switch (args.Interaction.Data.CustomId)
            {
                case "YesContext":
                    await UploadEmojis();
                    break;
                case "NoContext":
                    await args.Interaction.DeleteOriginalResponseAsync();
                    break;
            }
        }

        public static async Task UploadEmojiHandler(InteractionContext ctx)
        {
            if ((ctx.Member.Permissions & Permissions.ManageEmojis) == 0)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder(
                        new DiscordFollowupMessageBuilder().WithContent(
                            "You do not have the correct permissions")));
                return;
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder(
                    new DiscordMessageBuilder().AddComponents(
                        new DiscordButtonComponent(ButtonStyle.Success, "YesContext", "UploadEmojis"),
                        new DiscordButtonComponent(ButtonStyle.Danger, "NoContext", "DenyEmojis")
                    ).WithContent("__**Are you sure you want to continue, this requires a MINIMUM of 50 emoji slots**__")
                ));
        }
    }
}
