using DSharpPlus;
using DSharpPlus.Entities;

namespace Un1ver5e.Bot
{
    public static class Extensions
    {
        /// <summary>
        /// Shuffles the collection, making it random-ordered using <see cref="Random.Shared"/> as a randomizer. This returns a lazy collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The original collection.</param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection) => collection.OrderBy((e) => Random.Shared.Next());

        /// <summary>
        /// Shuffles the collection, making it random-ordered using <paramref name="random"/> as a randomizer. This returns a lazy collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The original collection.</param>
        /// <param name="random">The used randomizer.</param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection, Random random) => collection.OrderBy((e) => random.Next());

        /// <summary>
        /// Gets random element of a <paramref name="collection"/> using <see cref="Random.Shared"/> as a randomizer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T GetRandomElement<T>(this IList<T> collection) => collection.ElementAt(Random.Shared.Next(0, collection.Count));

        /// <summary>
        /// Gets random element of a <paramref name="collection"/> using <paramref name="random"/> as a randomizer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="random">The used randomizer.</param>
        /// <returns></returns>
        public static T GetRandomElement<T>(this IList<T> collection, Random random) => collection.ElementAt(random.Next(0, collection.Count));

        /// <summary>
        /// Formats string as a Discord Codeblock
        /// </summary>
        /// <param name="orig">The original string.</param>
        /// <returns></returns>
        public static string AsCodeBlock(this string original, string lang = "") => $"```{lang}\n{original}```";

        /// <summary>
        /// Defines whether this <see cref="DiscordUser"/> is an owner of the <paramref name="client"/>.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool IsOwnerOf(this DiscordUser user, DiscordClient client) => client.CurrentApplication.Owners.Contains(user);
    }
}
