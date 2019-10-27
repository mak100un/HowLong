using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;

namespace HowLong.Extensions
{
    public class Settings
    {
        private static ISettings AppSettings => CrossSettings.Current;
        public static string Language
        {
            get => AppSettings.GetValueOrDefault(nameof(Language), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Language), value);
        }
        public static bool IsDark
        {
            get => AppSettings.GetValueOrDefault(nameof(IsDark), false);
            set => AppSettings.AddOrUpdateValue(nameof(IsDark), value);
        }
        public static bool IsMonday
        {
            get => AppSettings.GetValueOrDefault(nameof(IsMonday), true);
            set => AppSettings.AddOrUpdateValue(nameof(IsMonday), value);
        }
        public static double MondayStart
        {
            get => AppSettings.GetValueOrDefault(nameof(MondayStart), new TimeSpan(9,0,0).TotalMinutes);
            set => AppSettings.AddOrUpdateValue(nameof(MondayStart), value);
        }
        public static double MondayEnd
        {
            get => AppSettings.GetValueOrDefault(nameof(MondayEnd), new TimeSpan(17, 0, 0).TotalMinutes);
            set => AppSettings.AddOrUpdateValue(nameof(MondayEnd), value);
        }
        public static bool IsTuesday
        {
            get => AppSettings.GetValueOrDefault(nameof(IsTuesday), true);
            set => AppSettings.AddOrUpdateValue(nameof(IsTuesday), value);
        }
        public static double TuesdayStart
        {
            get => AppSettings.GetValueOrDefault(nameof(TuesdayStart), new TimeSpan(9, 0, 0).TotalMinutes);
            set => AppSettings.AddOrUpdateValue(nameof(TuesdayStart), value);
        }
        public static double TuesdayEnd
        {
            get => AppSettings.GetValueOrDefault(nameof(TuesdayEnd), new TimeSpan(17, 0, 0).TotalMinutes);
            set => AppSettings.AddOrUpdateValue(nameof(TuesdayEnd), value);
        }
        public static bool IsWednesday
        {
            get => AppSettings.GetValueOrDefault(nameof(IsWednesday), true);
            set => AppSettings.AddOrUpdateValue(nameof(IsWednesday), value);
        }
        public static double WednesdayStart
        {
            get => AppSettings.GetValueOrDefault(nameof(WednesdayStart), new TimeSpan(9, 0, 0).TotalMinutes);
            set => AppSettings.AddOrUpdateValue(nameof(WednesdayStart), value);
        }
        public static double WednesdayEnd
        {
            get => AppSettings.GetValueOrDefault(nameof(WednesdayEnd), new TimeSpan(17, 0, 0).TotalMinutes);
            set => AppSettings.AddOrUpdateValue(nameof(WednesdayEnd), value);
        }
        public static bool IsThursday
        {
            get => AppSettings.GetValueOrDefault(nameof(IsThursday), true);
            set => AppSettings.AddOrUpdateValue(nameof(IsThursday), value);
        }
        public static double ThursdayStart
        {
            get => AppSettings.GetValueOrDefault(nameof(ThursdayStart), new TimeSpan(9, 0, 0).TotalMinutes);
            set => AppSettings.AddOrUpdateValue(nameof(ThursdayStart), value);
        }
        public static double ThursdayEnd
        {
            get => AppSettings.GetValueOrDefault(nameof(ThursdayEnd), new TimeSpan(17, 0, 0).TotalMinutes);
            set => AppSettings.AddOrUpdateValue(nameof(ThursdayEnd), value);
        }
        public static bool IsFriday
        {
            get => AppSettings.GetValueOrDefault(nameof(IsFriday), true);
            set => AppSettings.AddOrUpdateValue(nameof(IsFriday), value);
        }
        public static double FridayStart
        {
            get => AppSettings.GetValueOrDefault(nameof(FridayStart), new TimeSpan(9, 0, 0).TotalMinutes);
            set => AppSettings.AddOrUpdateValue(nameof(FridayStart), value);
        }
        public static double FridayEnd
        {
            get => AppSettings.GetValueOrDefault(nameof(FridayEnd), new TimeSpan(16, 0, 0).TotalMinutes);
            set => AppSettings.AddOrUpdateValue(nameof(FridayEnd), value);
        }
        public static bool IsSaturday
        {
            get => AppSettings.GetValueOrDefault(nameof(IsSaturday), false);
            set => AppSettings.AddOrUpdateValue(nameof(IsSaturday), value);
        }
        public static double SaturdayStart
        {
            get => AppSettings.GetValueOrDefault(nameof(SaturdayStart), new TimeSpan(9, 0, 0).TotalMinutes);
            set => AppSettings.AddOrUpdateValue(nameof(SaturdayStart), value);
        }
        public static double SaturdayEnd
        {
            get => AppSettings.GetValueOrDefault(nameof(SaturdayEnd), new TimeSpan(16, 0, 0).TotalMinutes);
            set => AppSettings.AddOrUpdateValue(nameof(SaturdayEnd), value);
        }
        public static bool IsSunday
        {
            get => AppSettings.GetValueOrDefault(nameof(IsSunday), false);
            set => AppSettings.AddOrUpdateValue(nameof(IsSunday), value);
        }
        public static double SundayStart
        {
            get => AppSettings.GetValueOrDefault(nameof(SundayStart), new TimeSpan(9, 0, 0).TotalMinutes);
            set => AppSettings.AddOrUpdateValue(nameof(SundayStart), value);
        }
        public static double SundayEnd
        {
            get => AppSettings.GetValueOrDefault(nameof(SundayEnd), new TimeSpan(16, 0, 0).TotalMinutes);
            set => AppSettings.AddOrUpdateValue(nameof(SundayEnd), value);
        }
    }
}
