﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Un1ver5e.Bot
{
    /// <summary>
    /// The basic command module, contains simple commands.
    /// </summary>
    public class BasicCommands : BaseCommandModule
    {
        public Random Random { private get; set; } = System.Random.Shared;
        
        [Command("avatar"), Description("Выдает ссылку на аватар нужного юзера."),
            RequireGuild()]
        public async Task GetAvatarCommand(CommandContext ctx, DiscordMember mem)
        {
            string avatarUrl = mem.GetGuildAvatarUrl(DSharpPlus.ImageFormat.Auto);
            await ctx.RespondAsync(new DiscordEmbedBuilder(Statics.EmbedTemplate).WithImageUrl(avatarUrl).AddField($"Ссылка на аватар `{mem.DisplayName}`:", avatarUrl));
        }

        [Command("roll"), Description("Выдает случайное число от 1 до 100.")
            ]
        public async Task RollCommand(CommandContext ctx)
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder(Statics.EmbedTemplate).AddField("Результат вашего ролла [1-100]:", "🎲 **" + Random.Next(1, 101).ToString() + "**"));
        }

        [Command("random"), Aliases("rnd"), Description("Выдает случайное число между данными включительно.")
            ]
        public async Task RandomCommand(CommandContext ctx, int from, int to)
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder(Statics.EmbedTemplate).AddField($"Результат вашего ролла [{from}-{to}]:", "🎲 **" + Random.Next(from, to + 1).ToString() + "**"));
        }

        [Command("rate"), Description("Оценивает сообщение, на которое вызван ответ."),
            RequireReferencedMessage()]
        public async Task RateCommand(CommandContext ctx)
        {
            int rate = Random.Next(1, 11);

            string rateMessage = rate switch
            {
                1 => ":thumbsup: Крутяк",
                2 => ":smile: Нормально-нормально",
                3 => ":slight_smile: Покатит",
                4 => ":confused: Ну такое",
                5 => ":thumbsdown: Хрень",
                6 => ":fire: Огонь!",
                7 => ":japanese_ogre: Ы",
                8 => ":scream: Абалдеть!!!",
                9 => ":rage: Кринж",
                10 => ":banana: ок",
                _ => throw new NotImplementedException()
            };

            await ctx.Message.ReferencedMessage.RespondAsync(new DiscordEmbedBuilder(Statics.EmbedTemplate)
                .AddField("Экспертная оценка от бота :sunglasses: ", rateMessage)
                .WithFooter("Все оценки бота случайны."));
        }

        [Group("encrypt"), Aliases("enc"), RequireOwner()]
        public class EncryptionCommands : BaseCommandModule
        {
            [Command("gray"), RequireOwner()]
            public async Task GrayEncryptionCommand(CommandContext ctx, [RemainingText] string msg)
            {
                await ctx.RespondAsync(GrayEncrypt(msg));
            }
            private string GrayEncrypt(string msg)
            {
                return new string(msg
                    .Where(c => char.IsLetter(c))
                    .Select(c => (char)(c ^ ('\u00aa'/*Just a random char that does a nice-looking encryption*/)))
                    .ToArray());
            }

            [Command("kit"), RequireOwner()]
            public async Task KitEncryptionCommand(CommandContext ctx, [RemainingText] string msg)
            {
                await ctx.RespondAsync(KitEncrypt(msg));
            }
            private string KitEncrypt(string msg)
            {
                string[] replacements = { "ka","zu","ru","ji","te","ku","su","z","ki","ki","me","ta","rin","to","mo","no","shi","ari","chi","do","lu","ri","mi","ke","hi","hi","zuk","zuk","zuk","mei","fu","na" };

                return string.Join(string.Empty, msg
                    .ToLower()
                    .Where(c => c == ' ' || c >= 'а' && c <= 'я')
                    .Select(c => c == ' ' ? " " : replacements[c - 'а']));
            }

        }

        [Group("generate"), Description("Команды для генерации чего-то."), Aliases("g")]
        public class GenerateCommands : BaseCommandModule
        {
            private async Task SendFileByUrlWithSource(DiscordMessage reference, string url)
            {
                using (HttpClient client = new HttpClient())
                {
                    Stream pic = await client.GetStreamAsync(url);
                    await reference.RespondAsync(new DiscordMessageBuilder()
                        .WithFile($"{url}.jpg", pic)
                        .WithContent($"||Источник: {url}||")); ;
                }
            }

            [Command("cat"), Description("Случайные несуществующие коты!")
            ]
            public async Task GenerateCatCommand(CommandContext ctx) => await SendFileByUrlWithSource(ctx.Message, "https://thiscatdoesnotexist.com/");

            [Command("horse"), Description("Случайные несуществующие лошади!")
            ]
            public async Task GenerateHorseCommand(CommandContext ctx) => await SendFileByUrlWithSource(ctx.Message, "https://thishorsedoesnotexist.com/");

            [Command("art"), Description("Искусство!")
            ]
            public async Task GenerateArtCommand(CommandContext ctx)            => await SendFileByUrlWithSource(ctx.Message, "https://thisartworkdoesnotexist.com/");
        }

        [Group("db"), Description("Команды для работы с базой данных.")]
        public class DatabaseCommand : BaseCommandModule
        {
            [GroupCommand()]
            public async Task StatusCommand(CommandContext ctx)
            {
                long dbSizeBytes = Database.GetDatabaseBackup().Length;

                await ctx.RespondAsync($"База данных содержит {dbSizeBytes} байт данных. ({dbSizeBytes / 1_048_576} MBs)");
            }

            [Command("sqlnq"), RequireOwner()]
            public async Task SqlNonQueryCommand(CommandContext ctx, [RemainingText()] string query)
            {
                await ctx.RespondAsync(Database.ExecuteSqlNonQuery(query).AsCodeBlock());
            }

            [Command("sqls"), RequireOwner()]
            public async Task SqlScalarCommand(CommandContext ctx, [RemainingText()] string query)
            {
                await ctx.RespondAsync(Database.ExecuteSqlScalar(query).AsCodeBlock());
            }

            [Command("get"), Aliases("backup"), RequireOwner(), RequireDirectMessage()]
            public async Task GetDatabaseBackupCommand(CommandContext ctx)
            {
                FileStream dbfs = Database.GetDatabaseBackup();

                await ctx.RespondAsync(new DiscordMessageBuilder().WithFile(
                    $"MO_Backup_{DateTime.Now}.db3", dbfs));

                dbfs.Dispose();
            }
        }

        [Group("feed"), Description("Команды для работы с \"лентами\".")]
        public class FeedCommands : BaseCommandModule
        {
            [GroupCommand(), Description("Пишет данное сообщение в ленты."),
                RequireOwner()]
            public async Task Broadcast(CommandContext ctx, [RemainingText(), Description("Сообщение")] string message)
            {
                await Feeds.SendMessageToFeeds(ctx.Message.Author, message);
            }

            [Command("enable"), Description("Назначает данный канал лентой."),
                RequirePermissions(DSharpPlus.Permissions.Administrator)]
            public async Task Enablefeed(CommandContext ctx)
            {
                Feeds.EnableFeed(ctx.Channel);
                await ctx.RespondAsync(new DiscordEmbedBuilder(Statics.EmbedTemplate).WithDescription($"Назначил {ctx.Channel.Mention} лентой."));
            }

            [Command("disable"), Description("Убирает данный канал из списка лент."),
                RequirePermissions(DSharpPlus.Permissions.Administrator)]
            public async Task Disablefeed(CommandContext ctx)
            {
                Feeds.DisableFeed(ctx.Channel);
                await ctx.RespondAsync(new DiscordEmbedBuilder(Statics.EmbedTemplate).WithDescription($"Убрал {ctx.Channel.Mention} из списка лент."));
            }
        }

        [Group("logs"), Description("Команды для работы с логами.")]
        public class LogsCommands : BaseCommandModule
        {
            [Command("show"), Aliases("get"), RequireOwner()]
            public async Task GetLogs(CommandContext ctx)
            {
                using (var stream = File.Open($"{Statics.AppFolderPath}/logs/latest.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    await ctx.RespondAsync(new DiscordMessageBuilder().WithFile(stream.Name, stream));
                }
            }

            [Command("drop"), Aliases("clear"), RequireOwner()]
            public async Task ClearLogs(CommandContext ctx)
            {
                Logging.ClearLogs();
            }

            [Command("setlevel"), RequireOwner()]
            public async Task SetLogLevel(CommandContext ctx, string level)
            {
                Serilog.Events.LogEventLevel actualLevel = level.ToLower() switch
                {
                    "verbose" => Serilog.Events.LogEventLevel.Verbose,
                    "debug" => Serilog.Events.LogEventLevel.Debug,
                    "info" => Serilog.Events.LogEventLevel.Information,
                    "information" => Serilog.Events.LogEventLevel.Information,
                    "warn" => Serilog.Events.LogEventLevel.Warning,
                    "warning" => Serilog.Events.LogEventLevel.Warning,
                    "error" => Serilog.Events.LogEventLevel.Error,
                    _ => throw new ArgumentException("Недопустимый уровень логгирования.")
                };

                Logging.SetLogLevel(actualLevel);
            }
        }

        [Command("status"), Description("Состояние бота.")]
        public async Task Status(CommandContext ctx)
        {
            int memAct = (int)SystemInfo.GetActualMemoryMegaBytes();
            int memVir = (int)SystemInfo.GetVirtualMemoryMegaBytes();

            int memPer = SystemInfo.GetPercentage(memAct, memVir);
            string memBar = SystemInfo.GetPercentageChar(memPer);

            string memoryLine =
                $"{memAct}/{memVir}MBs. ({memPer}%) {memBar}";

            DiscordEmbedBuilder deb = new DiscordEmbedBuilder(Statics.EmbedTemplate)
                .WithDescription($"Состояние бота на момент {DateTime.Now}")
                .AddField("Используемая память:", memoryLine)
                .AddField("Пинг:", $"{SystemInfo.GetPing()}мс.")
                .AddField("Бот запущен уже:", $"{DateTime.Now - Process.GetCurrentProcess().StartTime}.");

            await ctx.RespondAsync(deb);
        }

        [Command("whyerror"), Aliases("why", "details"), Description("Дает подробную информацию о последней ошибке.")]
        public async Task WhyError(CommandContext ctx)
        {
            Exception ex = Features.ErrorResponds.GetException(ctx.User.Id);

            InteractivityExtension interactivity = Program.DiscordClient.GetExtension<InteractivityExtension>();

            await interactivity.SendPaginatedMessageAsync(
                ctx.Channel,
                ctx.User,
                interactivity.GeneratePagesInEmbed($"[{ex.Message}]\n>>{ex.StackTrace}>>", DSharpPlus.Interactivity.Enums.SplitType.Character, new DiscordEmbedBuilder(Statics.EmbedTemplate))
                );
        }


        [Command("shutdown"), RequireOwner()]
        public async Task Shutdown(CommandContext ctx, [RemainingText()] string message)
        {
            if (!await CommandExtensions.GetConfirmation(ctx)) return;
            await Feeds.SendMessageToFeeds(Program.DiscordClient.CurrentUser, $"Выключаюсь по причине:\n> {message}");
            await Program.DiscordClient.DisconnectAsync();
            Environment.Exit(0);
        }

        
    }

    public static class CommandExtensions
    {
        public static async Task<bool> GetConfirmation(CommandContext ctx, string question = "Вы уверены что хотите продолжить?")
        {
            var msg = await ctx.RespondAsync(new DiscordMessageBuilder()
                .WithContent(ctx.User.Mention + "\n" + question)
                .AddComponents(new DiscordButtonComponent(DSharpPlus.ButtonStyle.Success, "confirm", "Да")));

            var respond = await Program.DiscordClient
                .GetExtension<InteractivityExtension>()
                .WaitForButtonAsync(msg, ctx.User);

            await msg.DeleteAsync();

            return !respond.TimedOut;
        }
    }

    /// <summary>
    /// Defines that a command is only usable if the message contains a referenced message.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequireReferencedMessageAttribute : CheckBaseAttribute
    {
        public RequireReferencedMessageAttribute() { }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return Task.FromResult(help || ctx.Message.ReferencedMessage != null);
        }
    }
}
