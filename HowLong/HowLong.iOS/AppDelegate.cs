using Foundation;
using HowLong.Theme;
using UIKit;
using Xamarin.Forms;

namespace HowLong.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            SQLitePCL.Batteries.Init();
            Forms.Init();
            InitTheme();
            LoadApplication(new App());
            return base.FinishedLaunching(app, options);
        }
        private static void InitTheme()
        {
            var smallScreen = (int)UIScreen.MainScreen.Bounds.Height < 620;

            DeviceSize.Margin = smallScreen
                ? new Thickness(0, 12)
                : new Thickness(0);

            DeviceSize.VerticalOptions = smallScreen
                ? LayoutOptions.Start
                : LayoutOptions.CenterAndExpand;
        }
    }
}