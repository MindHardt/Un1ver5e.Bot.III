using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Un1ver5e.Bot
{
    /// <summary>
    /// Allows to access and modify feed channels.
    /// </summary>
    public static class Feeds
    {
        public static async Task SendMessageToFeeds(DiscordUser author, string message)
        {
            foreach (ulong id in Database.GetFeedChannels())
            {
                await (await Program.MainDiscordClient.GetChannelAsync(id)).SendMessageAsync(new DiscordEmbedBuilder(Extensions.EmbedTemplate)
                    .WithDescription(message)
                    .WithAuthor(author.Username, null, author.AvatarUrl));
            }
        }

        public static void EnableFeed(DiscordChannel channel)
        {
            Database.AddFeedChannel(channel.Id, channel.GuildId);
        }

        public static void DisableFeed(DiscordChannel channel)
        {
            Database.RemoveFeedChannel(channel.Id);
        }
    }
}
