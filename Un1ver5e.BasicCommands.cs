using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Un1ver5e.Bot
{
    /// <summary>
    /// The basic command module, contains simple commands.
    /// </summary>
    public class BasicCommands : BaseCommandModule
    {
        [Command("Avatar"), Description("Выдает ссылку на аватар нужного юзера."), 
            RequireGuild()]
        public async Task GetAvatar(CommandContext ctx, DiscordMember mem)
        {
            string avatarUrl = mem.GetGuildAvatarUrl(DSharpPlus.ImageFormat.Auto);
            await ctx.RespondAsync(new DiscordEmbedBuilder(Extensions.EmbedTemplate).WithImageUrl(avatarUrl).AddField($"Ссылка на аватар `{mem.DisplayName}`:", avatarUrl));
        }

        [Command("Roll"), Description("Выдает случайное число от 1 до 100.")
            ]
        public async Task Roll(CommandContext ctx)
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder(Extensions.EmbedTemplate).AddField("Результат вашего ролла [1-100]:", "🎲 **" + Extensions.Random.Next(1, 101).ToString() + "**"));
        }

        [Command("Rate"), Description("Оценивает сообщение, на которое вызван ответ."),
            RequireReferencedMessage()]
        public async Task Rate(CommandContext ctx)
        {
            int rate = Extensions.Random.Next(1, 11);

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

            await ctx.Message.ReferencedMessage.RespondAsync(new DiscordEmbedBuilder(Extensions.EmbedTemplate)
                .AddField("Экспертная оценка от бота :sunglasses: ", rateMessage)
                .WithFooter("Все оценки бота случайны, не принимайте близко к сердцу."));
        }




        [Command("Sqlnq"), RequireOwner()]
        public async Task SqlNonQuery(CommandContext ctx, [RemainingText()] string query)
        {
            await ctx.RespondAsync(Database.ExecuteSqlNonQuery(query).AsCodeBlock());
        }

        [Command("Sqls"), RequireOwner()]
        public async Task SqlScalar(CommandContext ctx, [RemainingText()] string query)
        {
            await ctx.RespondAsync(Database.ExecuteSqlScalar(query).AsCodeBlock());
        }

        [Command("givemedb"), RequireOwner(), RequireDirectMessage()]
        public async Task GetDatabaseBackup(CommandContext ctx)
        {
            var dbfs = Database.GetDatabaseBackup();

            await ctx.RespondAsync(new DiscordMessageBuilder().WithFile(
                $"MO_Backup_{DateTime.Now}.db3", dbfs));

            dbfs.Dispose();
        }




        [Command("broadcast"), RequireOwner()]
        public async Task Broadcast(CommandContext ctx, [RemainingText()] string msg)
        {
            await Feeds.SendMessageToFeeds(ctx.Message.Author, msg);
        }

        [Command("enablefeed"), RequireOwner()]
        public async Task Enablefeed(CommandContext ctx)
        {
            Feeds.EnableFeed(ctx.Channel);
            await ctx.RespondAsync(new DiscordEmbedBuilder(Extensions.EmbedTemplate).WithDescription($"Назначил {ctx.Channel.Mention} новостным каналом."));
        }

        [Command("disablefeed"), RequireOwner()]
        public async Task Disablefeed(CommandContext ctx)
        {
            Feeds.DisableFeed(ctx.Channel);
            await ctx.RespondAsync(new DiscordEmbedBuilder(Extensions.EmbedTemplate).WithDescription($"Убрал {ctx.Channel.Mention} из списка новостных каналов."));
        }




        [Command("shutdown"), RequireOwner()]
        public async Task Shutdown(CommandContext ctx, [RemainingText()] string message)
        {
            if (!await CommandExtensions.GetConfirmation(ctx)) return;
            await Feeds.SendMessageToFeeds(Program.MainDiscordClient.CurrentUser, $"Выключаюсь по причине:\n> {message}");
            await Program.MainDiscordClient.DisconnectAsync();
            Environment.Exit(0);
        }




        [Command("getlogs"), RequireOwner()]
        public async Task GetLogs(CommandContext ctx)
        {
            string logs;

            using (var stream = File.Open($"{Extensions.AppFolderPath}/logs/latest.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                logs = Encoding.UTF8.GetString(buffer);
            }

            await Program.MainInteractivityExtension.SendPaginatedMessageAsync(
                ctx.Channel,
                ctx.User,
                Program.MainInteractivityExtension.GeneratePagesInContent(logs));
        }
    }

    public static class CommandExtensions
    {
        public static async Task<bool> GetConfirmation(CommandContext ctx, string question = "Вы уверены что хотите продолжить?")
        {
            var msg = await ctx.RespondAsync(new DiscordMessageBuilder()
                .WithContent(ctx.User.Mention + "\n" + question)
                .AddComponents(new DiscordButtonComponent(DSharpPlus.ButtonStyle.Success, "confirm", "Да")));

            var respond = await Program.MainInteractivityExtension.WaitForButtonAsync(msg, ctx.User);

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
