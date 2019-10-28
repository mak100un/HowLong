using Android.App;
using Android.Content;
using Android.OS;
using HowLong.Extensions;
using Plugin.Vibrate;
using System;

namespace HowLong.Droid.Broadcasts
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class HalfBroadcast : BroadcastReceiver
    {
        [Obsolete]
        public override void OnReceive(Context context, Intent intent)
        {
            using (var builder = new Notification.Builder(Application.Context))
            {
                builder.SetContentTitle(TranslationCodeExtension.GetTranslation("HalfWorkTitle"))
                .SetContentText(TranslationCodeExtension.GetTranslation("HalfWorkText"))
                .SetAutoCancel(true)
                .SetSmallIcon(Resource.Drawable.icon);

                var manager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var channelId = $"{Application.Context.PackageName}.general";
                    var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);

                    manager.CreateNotificationChannel(channel);

                    builder.SetChannelId(channelId);
                }

                var resultIntent = GetLauncherActivity();
                resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(Application.Context);
                stackBuilder.AddNextIntent(resultIntent);
                var resultPendingIntent =
                    stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
                builder.SetContentIntent(resultPendingIntent);

                manager.Notify(2, builder.Build());
                CrossVibrate.Current.Vibration(TimeSpan.FromMilliseconds(150));

                stackBuilder.Dispose();
            }
        }

        public static Intent GetLauncherActivity() =>
            Application.Context.PackageManager.GetLaunchIntentForPackage(Application.Context.PackageName);
    }
}