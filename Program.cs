using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Text;
using Un1ver5e.Bot.SlashCommands;
using Un1ver5e.Bot.TextCommands;

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
            //Startup
            Logging.ConfigureLogs();

            string splash = SplashReader.GetSplash();

            Log.Warning($"Session started >> {splash}");

            InteractivityExtension interactivity = DiscordClient.UseInteractivity(new()
            {
                Timeout = TimeSpan.FromMinutes(1),
                AckPaginationButtons = true,
                PollBehaviour = DSharpPlus.Interactivity.Enums.PollBehaviour.KeepEmojis,
                ResponseBehavior = DSharpPlus.Interactivity.Enums.InteractionResponseBehavior.Ack,
            });

            Log.Information("Interactivity set up.");

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

                Log.Debug($"Command errored >> {e.Exception.Message}");

                await Task.Run(async () =>
                {
                    var result = await interactivity.WaitForReactionAsync(r =>
                    r.Message == e.Context.Message &&
                    r.Emoji == respond &&
                    r.User == e.Context.User);

                    if (result.TimedOut == false)
                    {
                        string exceptionMessage = e.Exception.ToString();

                        MemoryStream ms = new(Encoding.Unicode.GetBytes(exceptionMessage));

                        DiscordMessageBuilder dmb = new DiscordMessageBuilder()
                            .WithContent("Текст вашей ошибки:")
                            .WithFile("error.txt", ms);

                        DiscordMessage respond = await e.Context.RespondAsync(dmb);

                        await respond.ScheduleDestructionAsync();
                    }
                });
            };
            cnext.CommandExecuted += async (s, e) =>
            {
                await e.Context.Message.CreateReactionAsync(Statics.QuickResponds.Ok);

                Log.Debug($"Command successfully executed >> {e.Context.Message.Content}");
            };

            Log.Information("CommandsNext set up.");

            DiscordClient.GuildDownloadCompleted += async (s, e) =>
            {
                Log.Information($"Guild download complete. Loaded {e.Guilds.Count} guilds.");
            };

            SlashCommandsExtension slash = DiscordClient.UseSlashCommands();

#if RELEASE
            slash.RegisterCommands<SlashCommands>(956094613536505866);
            slash.RegisterCommands<SlashCommands>(751088089463521322);

            Log.Information("Slashies registered.");
#else
            slash.RegisterCommands<EmptyCommands>(956094613536505866);
            slash.RegisterCommands<EmptyCommands>(751088089463521322);
            Log.Information("Slashies emptied.");
#endif
            await DiscordClient.ConnectAsync(new DiscordActivity(splash, ActivityType.Watching));

            Log.Information($"{DiscordClient.CurrentUser.Username} is here.");

            await Task.Delay(-1);
        }
    }
}
