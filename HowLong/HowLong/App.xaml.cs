using System.Threading.Tasks;
using HowLong.Containers;
using HowLong.Extensions;
using HowLong.Navigation;
using HowLong.Services;
using HowLong.Values;
using HowLong.ViewModels;
using HowLong.Views;
using Plugin.Connectivity;
using Plugin.LatestVersion;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowLong
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App
    {
        public App()
        {
            InitializeComponent();
            CompositionRoot.Init();
            if (Settings.IsDark) ThemeService.ChangeToDark();
            var mainPage = CompositionRoot.Resolve<MainPage>();
            mainPage.ViewModel = CompositionRoot.Resolve<MainViewModel>();
            var navigationPage = new NavigationPage(mainPage);
            MainPage = navigationPage;
            CompositionRoot.Resolve<INavigationService>().SetNavigation(navigationPage.Navigation);
            CheckVersionAsync();
        }

        private static async void CheckVersionAsync()
        {
            try
            {
                while (true)
                {
                    ReAccess:
                    if (!CrossConnectivity.Current.IsConnected)
                    {
                        await Current.MainPage.DisplayAlert(TranslationCodeExtension.GetTranslation("ErrorConnectionTitle"),
                            TranslationCodeExtension.GetTranslation("ErrorConnectionText"),
                            TranslationCodeExtension.GetTranslation("OkText"));
                        await Task.Delay(3000);
                        goto ReAccess;
                    }

                    var isLatest = await CrossLatestVersion.Current.IsUsingLatestVersion();

                    if (isLatest) return;
                    await Current.MainPage.DisplayAlert
                        (TranslationCodeExtension.GetTranslation("UpdateVersionTitle"), 
                        TranslationCodeExtension.GetTranslation("UpdateVersionText"), 
                        TranslationCodeExtension.GetTranslation("GoToStore"));
                    await CrossLatestVersion.Current.OpenAppInStore(BaseValue.PackageName);
                    await Task.Delay(3000);
                }
            }
            catch
            {
                //
            }
            
        }

        protected override void OnSleep() =>
            (Current.MainPage.Navigation.NavigationStack[Current.MainPage.Navigation.NavigationStack.Count - 1]
              .BindingContext as AccountViewModel)?.Unsubscribe();

        protected override void OnResume() => 
            (Current.MainPage.Navigation.NavigationStack[Current.MainPage.Navigation.NavigationStack.Count - 1]
              .BindingContext as AccountViewModel)?.Subscribe();
    }
}

