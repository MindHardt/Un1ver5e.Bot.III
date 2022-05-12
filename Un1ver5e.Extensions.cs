using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

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
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection) => 
            collection.OrderBy((e) => Random.Shared.Next());

        /// <summary>
        /// Shuffles the collection, making it random-ordered using <paramref name="random"/> as a randomizer. This returns a lazy collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The original collection.</param>
        /// <param name="random">The used randomizer.</param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection, Random random) => 
            collection.OrderBy((e) => random.Next());

        /// <summary>
        /// Gets random element of a <paramref name="collection"/> using <see cref="Random.Shared"/> as a randomizer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T GetRandomElement<T>(this IList<T> collection) =>
            collection.Shuffle().First();

        /// <summary>
        /// Gets random element of a <paramref name="collection"/> using <paramref name="random"/> as a randomizer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="random">The used randomizer.</param>
        /// <returns></returns>
        public static T GetRandomElement<T>(this IList<T> collection, Random random) =>
            collection.Shuffle(random).First();

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

        /// <summary>
        /// Initiates this <see cref="DiscordMessage"/>'s destruction. The period between scheduling and eventually deleting the message is regulated by <see cref="Statics.MessageDestructionTime"/>
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task ScheduleDestructionAsync(this DiscordMessage msg, DiscordUser controller)
        {
            await msg.CreateReactionAsync(Statics.QuickResponds.SelfDestruct);

            InteractivityExtension inter = Program.DiscordClient.GetExtension<InteractivityExtension>();
            DiscordEmoji emoji = Statics.QuickResponds.SelfDestruct;

            await inter.WaitForReactionAsync(r => r.Emoji == emoji && r.User == controller, Statics.MessageDestructionTime);

            await msg.DeleteAsync();
        }
    }
}
