using HowLong.Containers;
using HowLong.Extensions;
using HowLong.Navigation;
using HowLong.Services;
using HowLong.ViewModels;
using HowLong.Views;
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
        }
        protected override void OnSleep() =>
            (Current.MainPage.Navigation.NavigationStack[Current.MainPage.Navigation.NavigationStack.Count - 1]
              .BindingContext as AccountViewModel)?.Unsubscribe();

        protected override void OnResume() => 
            (Current.MainPage.Navigation.NavigationStack[Current.MainPage.Navigation.NavigationStack.Count - 1]
              .BindingContext as AccountViewModel)?.Subscribe();
    }
}

