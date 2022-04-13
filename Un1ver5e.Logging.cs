using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Un1ver5e.Bot
{
    public static class Logging
    {
        private static LoggingLevelSwitch logSwitch = new(LogEventLevel.Information);
        /// <summary>
        /// Configures <see cref="Serilog.Log.Logger"/> for future use.
        /// </summary>
        public static void ConfigureLogs()
        {
            Directory.CreateDirectory(Extensions.AppFolderPath + "/logs");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(logSwitch)
                .WriteTo.Console()
                .WriteTo.File(
                    $"{Extensions.AppFolderPath}/logs/latest.log", 
                    shared: true, 
                    fileSizeLimitBytes: 1024)
                .CreateLogger();

            Log.Information("Serilog on-line!");
        }

        /// <summary>
        /// Clears log file.
        /// </summary>
        public static void ClearLogs()
        {
            Log.Warning("Logs being cleared!");
            Log.CloseAndFlush();

            File.WriteAllText($"{Extensions.AppFolderPath}/logs/latest.log", string.Empty);

            ConfigureLogs();
        }

        /// <summary>
        /// Sets minimum log level to specified <see cref="LogEventLevel"/>.
        /// </summary>
        /// <param name="level"></param>
        public static void SetLogLevel(LogEventLevel level)
        {
            logSwitch.MinimumLevel = level;
            Log.Information($"Minimum log level set to {level}");
        }
    }
}
