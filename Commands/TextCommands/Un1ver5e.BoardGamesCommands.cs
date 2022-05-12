using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Un1ver5e.Bot.TextCommands
{
    /// <summary>
    /// The basic command module, contains simple commands.
    /// </summary>
    [Group("bg")]
    public class BoardGamesCommands : BaseCommandModule
    {
        [Command("throw")]
        public async Task Throw(CommandContext ctx, [Description("Текстовое описание дайса, например 1d6, 3d6+4...")] string dice)
        {
            DiscordEmbedBuilder deb = new DiscordEmbedBuilder(Statics.EmbedTemplate)
                .AddField("Ваш бросок", BoardGames.Core.Dice.ThrowByQuery(dice).ToString());

            await ctx.RespondAsync(deb);
                
        }
    }
}
