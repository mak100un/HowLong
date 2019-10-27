using HowLong.DependencyServices;
using System;
using Android.App;
using Android.Content;
using HowLong.Droid.Broadcasts;
using HowLong.Droid.DependencyServices;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(ShowNotify))]
namespace HowLong.Droid.DependencyServices
{
    public class ShowNotify : IShowNotify
    {
        public void SetEnd(double minutes) =>
            GetSystemService().
                Set
                (
                    AlarmType.RtcWakeup,
                    GetTimeOffset(minutes),
                    GetPendingIntent
                        (
                            GetTypedIntent(typeof(EndBroadcast))
                        )
                );

        public void SetHalf(double minutes) =>
            GetSystemService().
                Set
                (
                    AlarmType.RtcWakeup,
                    GetTimeOffset(minutes),
                    GetPendingIntent
                        (
                            GetTypedIntent(typeof(HalfBroadcast))
                        )
                 );

        public void CancelAll()
        {
            var alarmManager = GetSystemService();
            alarmManager.Cancel(GetPendingIntent
                (
                    GetTypedIntent
                        (
                        typeof(EndBroadcast)
                        )
                ));
            alarmManager.Cancel(GetPendingIntent
                (
                    GetTypedIntent
                        (
                            typeof(HalfBroadcast)
                        )
                ));
        }

        private static long GetTimeOffset(double minutes) =>
            (long)(DateTime.UtcNow.AddMinutes(minutes) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

        private static Intent GetTypedIntent(Type type) =>
            new Intent(Application.Context, type);

        private static AlarmManager GetSystemService() =>
            (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);

        private static PendingIntent GetPendingIntent(Intent intent) =>
            PendingIntent.GetBroadcast
                (
                    Application.Context,
                    1,
                    intent,
                    PendingIntentFlags.UpdateCurrent
                );
    }
}