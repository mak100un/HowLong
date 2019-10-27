using HowLong.Extensions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HowLong.Navigation
{
    public interface INavigationService
    {
        Task GoToRootAsync(bool isAnimated = true);
        Task GoBackAsync(bool isAnimated = true);
        Task NavigateToAsync<TViewModel>(TViewModel viewModel, bool isAnimated = true)  where TViewModel : ViewModelBase;
        void SetNavigation(INavigation navigation);
    }
}
