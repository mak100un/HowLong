using HowLong.Containers;
using HowLong.Navigation;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using Xamarin.Forms.Xaml;

namespace HowLong.Extensions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseTitleView
    {
        private readonly INavigationService _navigationService;
        public BaseTitleView()
        {
            InitializeComponent();
            _navigationService = CompositionRoot.Resolve<INavigationService>();
            InitHandlers();
        }

        private void InitHandlers() => Observable.FromEventPattern(
                ev => LogoImgBtn.Clicked += ev,
                ev => LogoImgBtn.Clicked -= ev
            )
            .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
            .Subscribe(async _ => await _navigationService.GoToRootAsync());
    }
}