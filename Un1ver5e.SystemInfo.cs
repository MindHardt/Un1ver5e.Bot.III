using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Un1ver5e.Bot
{
    public static class SystemInfo
    {
        /// <summary>
        /// Gets <see cref="Program.MainDiscordClient"/>'s WebSocket's latency.
        /// </summary>
        /// <returns></returns>
        public static int GetPing() => Program.MainDiscordClient.Ping;
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

        public static string GetPercentageBar(int percentage, int barCount = 10)
        {
            if (percentage == 0) return new string('□', barCount);
            if (percentage >= 100) return new string('■', barCount);

            int filled = (percentage / barCount) - 1;

            return string.Join(string.Empty,
                "■",
                new string('■', filled),
                new string('□', barCount - filled),
                "□"
                );
        }
    }
}
