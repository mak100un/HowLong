using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace HowLong.Droid
{
    [Activity(MainLauncher = true,
        Label = "HowLong",
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Splash",
        NoHistory = true,
        ScreenOrientation = ScreenOrientation.Portrait)]

    public class SplashScreenActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var mainIntent = new Intent(Application.Context, typeof(MainActivity));

            mainIntent.SetFlags(ActivityFlags.SingleTop);

            StartActivity(mainIntent);
            Finish();
            OverridePendingTransition(0, 0);
        }
    }
}