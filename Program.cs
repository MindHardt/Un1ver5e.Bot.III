using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Un1ver5e.Bot
{
    public static class Program
    {
        /// <summary>
        /// The main <see cref="DiscordClient"/> object.
        /// </summary>
        public static readonly DiscordClient MainDiscordClient =
            new DiscordClient(new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                LoggerFactory = new LoggerFactory().AddSerilog(Log.Logger),
                Token = Database.GetToken(),
            });

        /// <summary>
        /// The main <see cref="CommandsNextExtension"/> object.
        /// </summary>
        public static readonly CommandsNextExtension MainCommandsNextExtension =
            MainDiscordClient.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { "mo", "мо" }
            });

        /// <summary>
        /// The main <see cref="InteractivityExtension"/> object.
        /// </summary>
        public static readonly InteractivityExtension MainInteractivityExtension =
            MainDiscordClient.UseInteractivity(new InteractivityConfiguration()
            {
                AckPaginationButtons = true,
                PollBehaviour = DSharpPlus.Interactivity.Enums.PollBehaviour.KeepEmojis,
                ResponseBehavior = DSharpPlus.Interactivity.Enums.InteractionResponseBehavior.Ack,
            });

        /// <summary>
        /// The bot's minecraft splash.
        /// </summary>
        public static readonly string Splash = Database.GetSplash();

        /// <summary>
        /// The async Main method.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            ConfigureLogs();

            Log.Warning($"Session started >> {Splash}");

            MainCommandsNextExtension.RegisterCommands<BasicCommands>();

            MainCommandsNextExtension.CommandErrored += async (s, e) =>
            {
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
                await e.Context.RespondAsync(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.DarkRed)
                    .WithDescription("Произошла ошибка при выполнении команды!")
                    .AddField("Команда:", e.Context.Message.Content.CutToLength(1024), true)
                    .AddField("Текст ошибки:", e.Exception.Message.CutToLength(1018).AsCodeBlock())
                    .AddField("Источник:", e.Exception.StackTrace.CutToLength(1018).AsCodeBlock()));
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
            };

            await MainDiscordClient.ConnectAsync(new DiscordActivity(Splash, ActivityType.Watching));

            Log.Information("All set up!");

            await Task.Delay(-1);
        }

        /// <summary>
        /// Configures <see cref="Serilog.Log.Logger"/> for future use.
        /// </summary>
        public static void ConfigureLogs()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .WriteTo.File($"{Extensions.AppFolderPath}/logs/latest.log", shared: true)
                .CreateLogger();

            Log.Information("Serilog on-line!");
        }
    }
}
