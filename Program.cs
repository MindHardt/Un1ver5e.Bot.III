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
using System.Reflection;

namespace Un1ver5e.Bot
{
    public static class Program
    {
        /// <summary>
        /// The main <see cref="DSharpPlus.DiscordClient"/> object.
        /// </summary>
        public static readonly DiscordClient DiscordClient =
            new(new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                MinimumLogLevel = LogLevel.Trace,
                LoggerFactory = new LoggerFactory().AddSerilog(Log.Logger),
                Token = TokenReader.GetToken()
            });

        /// <summary>
        /// The async entry point.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            Logging.ConfigureLogs();

            string splash = Database.GetSplash();

            Log.Warning($"Session started >> {splash}");

            CommandsNextExtension cnext = DiscordClient.UseCommandsNext(new()
            {
#if DEBUG
                StringPrefixes = new string[] { "mt " }
#else
                StringPrefixes = new string[] { "mo " }
#endif
            });

            cnext.RegisterCommands<BasicCommands>();

            cnext.CommandErrored += async (s, e) =>
            {
                DiscordEmoji respond =
                    e.Exception is DSharpPlus.CommandsNext.Exceptions.CommandNotFoundException ||
                    e.Exception is DSharpPlus.CommandsNext.Exceptions.InvalidOverloadException ?
                    Statics.QuickResponds.What : Statics.QuickResponds.Error;

                await e.Context.Message.CreateReactionAsync(respond);

                Features.ErrorResponds.AddException(e.Context.User.Id, e.Exception);

                Log.Information($"Command errored >> {e.Exception.Message}");
            };
            cnext.CommandExecuted += async (s, e) =>
            {
                await e.Context.Message.CreateReactionAsync(Statics.QuickResponds.Ok);

                Log.Debug($"Command successfully executed >> {e.Context.Message.Content}");
            };

            Log.Information("CommandsNext set up.");

            DiscordClient.UseInteractivity(new()
            {
                AckPaginationButtons = true,
                PollBehaviour = DSharpPlus.Interactivity.Enums.PollBehaviour.KeepEmojis,
                ResponseBehavior = DSharpPlus.Interactivity.Enums.InteractionResponseBehavior.Ack,
            });

            Log.Information("Interactivity set up.");

            DiscordClient.GuildDownloadCompleted += async (s, e) =>
            {
                Log.Information($"Guild download complete. Loaded {e.Guilds.Count} guilds.");
            };

            await DiscordClient.ConnectAsync(new DiscordActivity(splash, ActivityType.Watching));

            Log.Information($"{DiscordClient.CurrentUser.Username} is here.");

            await Task.Delay(-1);
        }
    }
}
