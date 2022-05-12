using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un1ver5e.BoardGames.Core;

namespace Un1ver5e.Bot.TextCommands
{
    /// <summary>
    /// The boardgame commands module, contains commands for board games.
    /// </summary>
    public class BoardGamesCommands : BaseCommandModule
    {
        [Command("dice"), Description("Бросает дайсы, заданные текстовым описанием.")]
        public async Task Throw(CommandContext ctx, [Description("Текстовое описание дайса, например 1d6, 3d6+4...")] string dice)
        {
            DiscordEmbedBuilder deb = new DiscordEmbedBuilder(Statics.EmbedTemplate)
                .AddField("Ваш бросок", Dice.ThrowByQuery(dice).ToString());

            await ctx.RespondAsync(deb);
        }

        [RequireGuild]
        [Command("counter"), Description("Решает кто прав самым честным способом из известных.")]
        public async Task Counter(CommandContext ctx, DiscordMember opponent)
        {
            if (ctx.Member == opponent) throw new ArgumentException("Нельзя воевать с собой!");
            if (opponent.IsBot) throw new ArgumentException("Не трогай шайтан-машину!");

            Dice d100 = Dice.FromText("1d100");

            CompleteThrowResult authorResult = d100.Throw();
            CompleteThrowResult opponentResult = d100.Throw();

            string authorNick = ctx.Member!.DisplayName;
            string opponentNick = opponent.DisplayName;

            int resultSign = Math.Sign(authorResult.CompareTo(opponentResult));

            string response = resultSign switch
            {
                -1 => $"Победитель {authorNick}! ({authorResult.GetCompleteSum()}>{opponentResult.GetCompleteSum()})",
                0 => $"Ничья! ({authorResult.GetCompleteSum()}={opponentResult.GetCompleteSum()})",
                1 => $"Победитель {opponentNick}! ({authorResult.GetCompleteSum()}<{opponentResult.GetCompleteSum()})",
                _ => throw new NotImplementedException()
            };

            await ctx.RespondAsync(new DiscordEmbedBuilder().AddField("Результат спора:", response));
        }
    }
}
