using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace Un1ver5e.Bot
{
    public class ScheduleSet
    {
        private static int weekOffset = 1;
        public static ScheduleSet PrE201 => FromFile($"{Statics.DataFolderPath}/Schedules/PrE-201.json");

        [JsonPropertyName("FirstWeek")]
        public ScheduleWeek? First { get; set; } = new();

        [JsonPropertyName("SecondWeek")]
        public ScheduleWeek? Second { get; set; } = new();
        
        public static ScheduleSet FromFile(string path) => JsonSerializer.Deserialize<ScheduleSet>(File.ReadAllText(path, Encoding.Unicode), Statics.JsonSerializerOptions) ?? throw new ArgumentNullException();
        public void SaveAs(string path) => File.WriteAllText(path, JsonSerializer.Serialize(this, Statics.JsonSerializerOptions), Encoding.Unicode);

        public static void SwitchOffset()
        {
            weekOffset = weekOffset == 0 ? 1 : 0;
        }
        public string GetSchedule(DateTime date)
        {
            ScheduleWeek sw = (int)CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date) % 2 == weekOffset ? First! : Second!;

            return date.DayOfWeek switch
            {
                DayOfWeek.Sunday => sw.Sunday!,
                DayOfWeek.Monday => sw.Monday!,
                DayOfWeek.Tuesday => sw.Tuesday!,
                DayOfWeek.Wednesday => sw.Wednesday!,
                DayOfWeek.Thursday => sw.Thursday!,
                DayOfWeek.Friday => sw.Friday!,
                DayOfWeek.Saturday => sw.Saturday!,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public class ScheduleWeek
        {
            [JsonPropertyName("Monday")]
            public string? Monday { get; set; } = string.Empty;

            [JsonPropertyName("Tuesday")]
            public string? Tuesday { get; set; } = string.Empty;

            [JsonPropertyName("Wednesday")]
            public string? Wednesday { get; set; } = string.Empty;

            [JsonPropertyName("Thursday")]
            public string? Thursday { get; set; } = string.Empty;

            [JsonPropertyName("Friday")]
            public string? Friday { get; set; } = string.Empty;

            [JsonPropertyName("Saturday")]
            public string? Saturday { get; set; } = string.Empty;

            [JsonPropertyName("Sunday")]
            public string? Sunday { get; set; } = string.Empty;
        }
    }
}
