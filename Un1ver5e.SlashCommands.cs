using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Globalization;

namespace Un1ver5e.Bot
{
    public class SlashCommands : ApplicationCommandModule
    {
        [SlashCommand("schedule", "Ура автоматическое расписание!")]
        public async Task Schedule(InteractionContext ctx,
            [Choice("сегодня", 0)]
            [Choice("завтра", 1)]
            [Option("day", "На какой день?")]long delay) 
        {
            DateTimeOffset date = DateTimeOffset.Now.AddDays(delay);
            ScheduleSet schset = ScheduleSet.PrE201;
            string schedule = schset.GetSchedule(date.DateTime);

            string timestamp = $"<t:{date.ToUnixTimeSeconds()}:F>";

            DiscordInteractionResponseBuilder dirb = new()
            {
                Content = $"Расписание на {timestamp} ->\n{schedule}",
                IsEphemeral = true
            };

            await ctx.CreateResponseAsync(dirb);
        }

    }

}
