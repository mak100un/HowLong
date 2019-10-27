using Xamarin.Forms;

namespace HowLong.Services
{
    public class ThemeService
    {
        public static void ChangeToDark()
        {
            Application.Current.Resources["PrimaryLightColor"] = Color.FromHex("#2F2F2F");
            Application.Current.Resources["PrimaryDarkColor"] = Color.FromHex("#FFFFFF");
        }
        public static void ChangeToLight()
        {
            Application.Current.Resources["PrimaryLightColor"] = Color.FromHex("#FFFFFF");
            Application.Current.Resources["PrimaryDarkColor"] = Color.FromHex("#2F2F2F");
        }
    }
}
