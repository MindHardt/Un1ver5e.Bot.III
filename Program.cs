﻿using DSharpPlus;
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
using System.Reflection;

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
                MinimumLogLevel = LogLevel.Trace,
                LoggerFactory = new LoggerFactory().AddSerilog(Log.Logger),
                Token = Database.Tokens.GetToken(),
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
        /// The time bot launched.
        /// </summary>
        public static DateTime LaunchTime { get; private set; }

        /// <summary>
        /// The async Main method.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            Logging.ConfigureLogs();

            Log.Warning($"Session started >> {Splash}");

            if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            {
                Database.Tokens.AddToken(args[0]);
                Log.Warning($"Added a new token (arg) >> {args[0]}. Log file will be cleared now.");
                Logging.ClearLogs();
            }
            else if (File.Exists("token.txt"))
            {
                string token = File.ReadAllText("token.txt");
                Database.Tokens.AddToken(token);
                Log.Warning($"Added a new token (file) >> {token}. Log file will be cleared now.");
                Logging.ClearLogs();
                File.Delete("token.txt");
            }

            MainCommandsNextExtension.RegisterCommands<BasicCommands>();

            MainCommandsNextExtension.CommandErrored += async (s, e) =>
            {
                DiscordEmoji respond =
                    e.Exception is DSharpPlus.CommandsNext.Exceptions.CommandNotFoundException ||
                    e.Exception is DSharpPlus.CommandsNext.Exceptions.InvalidOverloadException ?
                    Extensions.QuickResponds.What : Extensions.QuickResponds.Error;

                await e.Context.Message.CreateReactionAsync(respond);

                Features.ErrorResponds.AddException(e.Context.User.Id, e.Exception);

                Log.Information($"Command errored >> {e.Exception.Message}");
            };

            MainCommandsNextExtension.CommandExecuted += async (s, e) =>
            {
                await e.Context.Message.CreateReactionAsync(Extensions.QuickResponds.Ok);

                Log.Debug($"Command successfully executed >> {e.Context.Message.Content}");
            };

            MainDiscordClient.GuildDownloadCompleted += async (s, e) =>
            {
                await Task.Delay(0); //Just to calm the IDE so this lambda becomes async
                Log.Information($"Guild download complete. Loaded {e.Guilds.Count} guilds.");
            };

            await MainDiscordClient.ConnectAsync(new DiscordActivity(Splash, ActivityType.Watching));

            Log.Information("All set up!");
            LaunchTime = DateTime.Now;

            await Task.Delay(-1);
        }
    }
}
