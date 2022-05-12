using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Un1ver5e.Bot.TextCommands
{
    /// <summary>
    /// The basic command module, contains simple commands.
    /// </summary>
    public class BasicCommands : BaseCommandModule
    {
        public Random Random { private get; set; } = Random.Shared;

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
            RequireReferencedMessage]
        public async Task RateCommand(CommandContext ctx)
        {
            string[] rateOptions =
            {
                ":thumbsup: Крутяк",
                ":smile: Нормально-нормально",
                ":slight_smile: Покатит",
                ":confused: Ну такое",
                ":thumbsdown: Хрень",
                ":fire: Огонь!",
                ":japanese_ogre: Ы",
                ":scream: Абалдеть!!!",
                ":rage: Кринж",
                ":banana: ок",
                ":zero: 0/10",
                ":one: 1/10",
                ":two: 2/10",
                ":three: 3/10",
                ":four: 4/10",
                ":five: 5/10",
                ":six: 6/10",
                ":seven: 7/10",
                ":eight: 8/10",
                ":nine: 9/10",
                ":ten: 10/10",
                ":knife: РЕЗНЯ",
                $"{DiscordEmoji.FromGuildEmote(ctx.Client, 971147037124984902)} Нет.",
                ":radioactive: Бомба!",
                ":exclamation: !!!"
            };

            string rateMessage = rateOptions.GetRandomElement(new Random((int)ctx.Message.Id));

            await ctx.Message.ReferencedMessage.RespondAsync(new DiscordEmbedBuilder(Statics.EmbedTemplate)
                .AddField("Экспертная оценка от бота :sunglasses: ", rateMessage)
                .WithFooter("Все оценки бота случайны."));
        }

        [Group("encrypt"), Description("Команды для шифрования текста."), Aliases("enc"), RequireOwner]
        public class EncryptionCommands : BaseCommandModule
        {
            //Язык "серого народа"
            [Command("gray"), RequireOwner]
            public async Task GrayEncryptionCommand(CommandContext ctx, [RemainingText] string msg)
            {
                if (string.IsNullOrWhiteSpace(msg))
                {
                    if (ctx.Message.ReferencedMessage == null) throw new ArgumentNullException(nameof(msg), "Не указан текст для шифрования");
                    msg = ctx.Message.ReferencedMessage.Content;
                }
                await ctx.RespondAsync(GrayEncrypt(msg));
            }
            private string GrayEncrypt(string msg)
            {
                return new string(msg
                    .Where(c => char.IsLetter(c))
                    .Select(c => (char)(c ^ ('\u00aa'/*Just a random char that does a nice-looking encryption*/)))
                    .ToArray());
            }

            //Язык Кицуне
            [Command("kit"), RequireOwner]
            public async Task KitEncryptionCommand(CommandContext ctx, [RemainingText] string msg)
            {
                if (string.IsNullOrWhiteSpace(msg))
                {
                    if (ctx.Message.ReferencedMessage == null) throw new ArgumentNullException(nameof(msg), "Не указан текст для шифрования");
                    msg = ctx.Message.ReferencedMessage.Content;
                }
                await ctx.RespondAsync(KitEncrypt(msg));
            }
            private string KitEncrypt(string msg)
            {
                string[] replacements = { "ka", "zu", "ru", "ji", "te", "ku", "su", "z", "ki", "ki", "me", "ta", "rin", "to", "mo", "no", "shi", "ari", "chi", "do", "lu", "ri", "mi", "ke", "hi", "hi", "zuk", "zuk", "zuk", "mei", "fu", "na" };

                return string.Join(string.Empty, msg
                    .ToLower()
                    .Where(c => c == ' ' || c >= 'а' && c <= 'я')
                    .Select(c => c == ' ' ? " " : replacements[c - 'а']));
            }

            //Шифр Base64
            [Command("b64"), RequireOwner]
            public async Task Base64EncryptionCommand(CommandContext ctx, [RemainingText] string msg)
            {
                if (string.IsNullOrWhiteSpace(msg))
                {
                    if (ctx.Message.ReferencedMessage == null) throw new ArgumentNullException(nameof(msg), "Не указан текст для шифрования");
                    msg = ctx.Message.ReferencedMessage.Content;
                }
                await ctx.RespondAsync(Base64Encrypt(msg));
            }
            private string Base64Encrypt(string msg)
            {
                byte[] msgAsBytes = Encoding.Unicode.GetBytes(msg);

                return Convert.ToBase64String(msgAsBytes);
            }
        }

        [Group("generate"), Description("Команды для генерации чего-то."), Aliases("g")]
        public class GenerateCommands : BaseCommandModule
        {
            private async Task SendFileByUrlWithSource(DiscordMessage reference, string url)
            {
                using HttpClient client = new();
                Stream pic = await client.GetStreamAsync(url);
                await reference.RespondAsync(new DiscordMessageBuilder()
                    .WithFile($"{url}.jpg", pic)
                    .WithContent($"||Источник: {url}||")); ;
            }

            [Command("cat"), Description("Случайные несуществующие коты!")
            ]
            public async Task GenerateCatCommand(CommandContext ctx) => await SendFileByUrlWithSource(ctx.Message, "https://thiscatdoesnotexist.com/");

            [Command("horse"), Description("Случайные несуществующие лошади!")
            ]
            public async Task GenerateHorseCommand(CommandContext ctx) => await SendFileByUrlWithSource(ctx.Message, "https://thishorsedoesnotexist.com/");

            [Command("art"), Description("Искусство!")
            ]
            public async Task GenerateArtCommand(CommandContext ctx) => await SendFileByUrlWithSource(ctx.Message, "https://thisartworkdoesnotexist.com/");
        }

        [Group("logs"), Description("Команды для работы с логами.")]
        public class LogsCommands : BaseCommandModule
        {
            [Command("show"), Aliases("get"), RequireOwner]
            public async Task GetLogs(CommandContext ctx)
            {
                using var stream = File.Open($"{Statics.AppPath}/logs/latest.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                await ctx.RespondAsync(new DiscordMessageBuilder().WithFile(stream.Name, stream));
            }

            [Command("drop"), Aliases("clear"), RequireOwner]
            public async Task ClearLogs(CommandContext ctx)
            {
                Logging.ClearLogs();
            }

            [Command("setlevel"), RequireOwner]
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

        [Command("shutdown"), RequireOwner]
        public async Task Shutdown(CommandContext ctx)
        {
            if (!await CommandExtensions.GetConfirmation(ctx)) return;
            await Program.DiscordClient.DisconnectAsync();
            Environment.Exit(0);
        }

        [Command("neofetch"), RequireOwner]
        public async Task Neofetch(CommandContext ctx)
        {
            bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            ProcessStartInfo psi = new()
            {
                FileName = isLinux ? "/bin/bash" : "cmd",
                Arguments = "neofetch",
                RedirectStandardOutput = true
            };

            Process proc = new Process()
            {
                StartInfo = psi
            };

            proc.Start();
            await proc.WaitForExitAsync();

            string respond = proc.StandardOutput.ReadToEnd();

            await ctx.RespondAsync(respond);
        }

        [Command("test"), RequireOwner]
        public async Task Test(CommandContext ctx, [RemainingText()] string message)
        {
            //await Task.Run(async () =>
            //{
            //    Stopwatch sw = new();
            //    sw.Start();

            //    Stream pic = Drawing.CreateLetter(message);

            //    sw.Stop();

            //    pic.Position = 0;

            //    DiscordMessageBuilder dmb = new DiscordMessageBuilder()
            //    .WithFile("foo.jpg", pic)
            //    .WithContent(sw.Elapsed.ToString());

            //    await ctx.RespondAsync(dmb);
            //});
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
}
