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
            foreach (ulong id in Database.Feeds.GetFeedChannels())
            {
                await (await Program.DiscordClient.GetChannelAsync(id)).SendMessageAsync(new DiscordEmbedBuilder(Statics.EmbedTemplate)
                    .WithDescription(message)
                    .WithAuthor(author.Username, null, author.AvatarUrl));
            }
        }

        public static void EnableFeed(DiscordChannel channel)
        {
            Database.Feeds.AddFeedChannel(channel.Id);
        }

        public static void DisableFeed(DiscordChannel channel)
        {
            Database.Feeds.RemoveFeedChannel(channel.Id);
        }
    }
}
