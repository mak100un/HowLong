using Android.App;
using Android.Content.PM;
using Android.OS;
using HowLong.Theme;
using Xamarin.Forms;

namespace HowLong.Droid
{
    [Activity(Label = "HowLong",
        Icon = "@drawable/icon",
        Theme = "@style/MainTheme",
        MainLauncher = false,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            Forms.Init(this, savedInstanceState);
            InitTheme();
            LoadApplication(new App());
        }

        private void InitTheme()
        {
            var smallScreen = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density) < 620;

            DeviceSize.Margin = smallScreen
                ? new Thickness(0, 12)
                : new Thickness(0);

            DeviceSize.VerticalOptions = smallScreen
                ? LayoutOptions.Start
                : LayoutOptions.CenterAndExpand;
        }
    }
}