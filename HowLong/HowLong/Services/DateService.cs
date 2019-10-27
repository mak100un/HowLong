using HowLong.Extensions;
using System;

namespace HowLong.Services
{
    public class DateService
    {
        public static bool IsWorking(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return Settings.IsMonday;
                case DayOfWeek.Tuesday:
                    return Settings.IsTuesday;
                case DayOfWeek.Wednesday:
                    return Settings.IsWednesday;
                case DayOfWeek.Thursday:
                    return Settings.IsThursday;
                case DayOfWeek.Friday:
                    return Settings.IsFriday;
                case DayOfWeek.Saturday:
                    return Settings.IsSaturday;
                default:
                    return Settings.IsSunday;
            }
        }
        public static TimeSpan StartWorkTime(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return TimeSpan.FromMinutes(Settings.MondayStart);
                case DayOfWeek.Tuesday:
                    return TimeSpan.FromMinutes(Settings.TuesdayStart);
                case DayOfWeek.Wednesday:
                    return TimeSpan.FromMinutes(Settings.WednesdayStart);
                case DayOfWeek.Thursday:
                    return TimeSpan.FromMinutes(Settings.ThursdayStart);
                case DayOfWeek.Friday:
                    return TimeSpan.FromMinutes(Settings.FridayStart);
                case DayOfWeek.Saturday:
                    return TimeSpan.FromMinutes(Settings.SaturdayStart);
                default:
                    return TimeSpan.FromMinutes(Settings.SundayStart);
            }
        }
        public static double WorkingTime(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return Settings.MondayEnd - Settings.MondayStart;
                case DayOfWeek.Tuesday:
                    return Settings.TuesdayEnd - Settings.TuesdayStart;
                case DayOfWeek.Wednesday:
                    return Settings.WednesdayEnd - Settings.WednesdayStart;
                case DayOfWeek.Thursday:
                    return Settings.ThursdayEnd - Settings.ThursdayStart;
                case DayOfWeek.Friday:
                    return Settings.FridayEnd - Settings.FridayStart;
                case DayOfWeek.Saturday:
                    return Settings.SaturdayEnd - Settings.SaturdayStart;
                default:
                    return Settings.SundayEnd - Settings.SundayStart;
            }
        }
        public static string DayShortName(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return TranslationCodeExtension.GetTranslation("MondayShortText");
                case DayOfWeek.Tuesday:
                    return TranslationCodeExtension.GetTranslation("TuesdayShortText");
                case DayOfWeek.Wednesday:
                    return TranslationCodeExtension.GetTranslation("WednesdayShortText");
                case DayOfWeek.Thursday:
                    return TranslationCodeExtension.GetTranslation("ThursdayShortText");
                case DayOfWeek.Friday:
                    return TranslationCodeExtension.GetTranslation("FridayShortText");
                case DayOfWeek.Saturday:
                    return TranslationCodeExtension.GetTranslation("SaturdayShortText");
                default:
                    return TranslationCodeExtension.GetTranslation("SundayShortText");
            }
        }
        public static string IsWorkDay(DayOfWeek dayOfWeek) => DayShortName(dayOfWeek)
                + (IsWorking(dayOfWeek)
                ? ", " + TranslationCodeExtension.GetTranslation("WeekdayText")
                : ", " + TranslationCodeExtension.GetTranslation("WeekendText"));
    }
}
