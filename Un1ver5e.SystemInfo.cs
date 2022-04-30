using System.Diagnostics;

namespace Un1ver5e.Bot
{
    public static class SystemInfo
    {
        /// <summary>
        /// Gets <see cref="Program.DiscordClient"/>'s WebSocket's latency.
        /// </summary>
        /// <returns></returns>
        public static int GetPing() => Program.DiscordClient.Ping;
        /// <summary>
        /// Gets currently used memory, in Megabytes.
        /// </summary>
        /// <returns></returns>
        public static long GetActualMemoryMegaBytes() => Process.GetCurrentProcess().PrivateMemorySize64 / 1_048_576;
        /// <summary>
        /// Gets the maximum amount of usable memory, in Megabytes.
        /// </summary>
        /// <returns></returns>
        public static long GetVirtualMemoryMegaBytes() => Process.GetCurrentProcess().VirtualMemorySize64 / 1_048_576;

        public static int GetPercentage(int current, int max)
        {
            return Math.Max(current * 100 / max, 0);
        }

        public static string GetPercentageChar(int percentage)
        {
            string[] symbols = { "🟩"/*green*/, "🟨"/*yellow*/, "🟧"/*orange*/, "🟥"/*red*/ };

            int symbolIndex = Math.Min(4, Math.Max(0, percentage / 25));

            return symbols[symbolIndex];
        }
    }
}
