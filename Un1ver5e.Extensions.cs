using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Un1ver5e.Bot
{
    /// <summary>
    /// Contains some useful features.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// The main <see cref="System.Random"/> object, all the code is advised to use this one.
        /// </summary>
        public static readonly Random Random = new();

        /// <summary>
        /// The ID of "MO Hub" guild.
        /// </summary>
        public static readonly ulong HubGuildID = 956094613536505866;

        /// <summary>
        /// The root folder for the project. Contains the '/' symbol at the end.
        /// </summary>
        public static readonly string AppFolderPath = Environment.CurrentDirectory + "/";

        /// <summary>
        /// Shuffles the collection, making it random-ordered using <see cref="Extensions.Random"/> as a randomizer. This does not return a lazy collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The original collection.</param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection) => collection.OrderBy((e) => Random.Next()).ToList();

        /// <summary>
        /// Gets random element of a collection using <see cref="Extensions.Random"/> as a randomizer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T GetRandomElement<T>(this IList<T> collection) => collection.ElementAt(Random.Next(0, collection.Count));

        /// <summary>
        /// The default-colored embed, using this is recommended.
        /// </summary>
        public static readonly DiscordEmbed EmbedTemplate = new DiscordEmbedBuilder().WithColor(0x3aebca).Build();

        /// <summary>
        /// Formats string as a Discord Codeblock
        /// </summary>
        /// <param name="orig">The original string.</param>
        /// <returns></returns>
        public static string AsCodeBlock(this string original) => "```" + original + "```";

        /// <summary>
        /// Contains <see cref="DiscordEmoji"/>s for quick responds.
        /// </summary>
        public static class QuickResponds
        {
            public static readonly DiscordEmoji Ok = DiscordEmoji.FromName(Program.MainDiscordClient, ":mo_ok:", true);
            public static readonly DiscordEmoji Error = DiscordEmoji.FromName(Program.MainDiscordClient, ":mo_error:", true);
            public static readonly DiscordEmoji What = DiscordEmoji.FromName(Program.MainDiscordClient, ":mo_what:", true);
            public static readonly DiscordEmoji NotFound = DiscordEmoji.FromName(Program.MainDiscordClient, ":mo_not_found:", true);
        }

    }
}
