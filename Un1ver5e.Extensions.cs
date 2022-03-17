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
        public static readonly DiscordEmbed EmbedTemplate = new DiscordEmbedBuilder().WithColor(DiscordColor.Azure).Build();

        /// <summary>
        /// Formats string as a Discord Codeblock
        /// </summary>
        /// <param name="orig">The original string.</param>
        /// <returns></returns>
        public static string AsCodeBlock(this string original) => "```" + original + "```";

        /// <summary>
        /// Cuts the string to the specified length.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CutToLength(this string original, int length) => original.Length > length ? original.Substring(0, length) : original;
    }
}
