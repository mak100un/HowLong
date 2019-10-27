using HowLong.Extensions;
using HowLong.ViewModels;
using HowLong.Views;
using HowLong.Containers;
using ReactiveUI;
using Splat;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HowLong.Navigation
{
    public class NavigationService : INavigationService
    {
        public static void Init()
        {
            Locator.CurrentMutable.Register(() => new HistoryPage(), typeof(IViewFor<HistoryViewModel>));
            Locator.CurrentMutable.Register(() => new AccountPage(), typeof(IViewFor<AccountViewModel>));
            Locator.CurrentMutable.Register(() => new MainPage(), typeof(IViewFor<MainViewModel>));
            Locator.CurrentMutable.Register(() => new SettingsPage
            (
                CompositionRoot.Resolve<MainPage>()
            ), typeof(IViewFor<SettingsViewModel>));
            Locator.CurrentMutable.Register(() => new WorkDaysPage
            (
                CompositionRoot.Resolve<MainPage>()
            ), typeof(IViewFor<WorkDaysViewModel>));
        }
        private INavigation _navigation;

        public void SetNavigation(INavigation navigation) => _navigation = navigation;
        public async Task GoToRootAsync(bool isAnimated = true) => await _navigation.PopToRootAsync(isAnimated);
        public async Task GoBackAsync(bool isAnimated = true) => await _navigation.PopAsync(isAnimated);
        public async Task NavigateToAsync<TViewModel>(TViewModel viewModel, bool isAnimated = true) where TViewModel : ViewModelBase
        {
            var page = Locator.Current.GetService<IViewFor<TViewModel>>();
            page.ViewModel = viewModel;
            await _navigation.PushAsync(page as Page, isAnimated);
        }
    }
}
