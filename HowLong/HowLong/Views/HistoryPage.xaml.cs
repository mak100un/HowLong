using HowLong.Converters;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using HowLong.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;

namespace HowLong.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage
    {
        private double _previousY;
        private string _action;

        public HistoryPage()
        {
            InitializeComponent();
            InitHandlers();
            DayPicker.Date = DateTime.Today;
        }

        private void InitHandlers()
        {
            Observable.FromEventPattern<ScrolledEventArgs>(
                h => MainLst.Scrolled += h,
                h => MainLst.Scrolled -= h)
                .Select(x => x.EventArgs)
                 .Subscribe(async _ =>
                        await Task.Run(() =>
                        {
                            if (_previousY < _.ScrollY) Device.BeginInvokeOnMainThread(() => CreateBtn.Hide());
                            else if (_previousY > _.ScrollY) Device.BeginInvokeOnMainThread(() => CreateBtn.Show());
                            _previousY = _.ScrollY;
                        })
                    );
            this.WhenAnyValue(x=>x.DayPicker.SelectedDate)
                .Where(x=> !_action.IsNullOrEmptyOrWhiteSpace()
                && _action != TranslationCodeExtension.GetTranslation("CancelAction")
                && x.HasValue)
                .Select(x => x.Value)
                 .Subscribe(_ =>
                 {
                     DayPicker.Date = DateTime.Today;
                     ViewModel.AddDayExecute(_, _action);
                 });
            CreateBtn.Clicked = async (s, e) => await CreateDayExecute();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.NoElements, v => v.NoElementsLbl.IsVisible)
               .DisposeWith(SubscriptionDisposables);

                this.OneWayBind(ViewModel, vm => vm.NoElements, v => v.OverWorkTitleLbl.IsVisible, OppositeConverter.BooleanConverterFunc)
               .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.NoElements, v => v.TotalOverWorkLbl.IsVisible, OppositeConverter.BooleanConverterFunc)
                .DisposeWith(SubscriptionDisposables);

                this.OneWayBind(ViewModel, vm => vm.TotalOverWork, v => v.OverWorkTitleLbl.Text, t => t >= default(TimeSpan)
                ? TranslationCodeExtension.GetTranslation("OverworkText") + " "
                : TranslationCodeExtension.GetTranslation("WeaknessesText") + " ")
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.TotalOverWork, v => v.TotalOverWorkLbl.TextColor, t => t >= default(TimeSpan)
                ? Color.ForestGreen
                : (Color)Application.Current.Resources["AccentColor"])
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.TotalOverWork, v => v.TotalOverWorkLbl.Text, t => t.ToString(@"dd\.hh\:mm"))
                .DisposeWith(SubscriptionDisposables);

                this.OneWayBind(ViewModel, vm => vm.AllAccounts, v => v.MainLst.ItemsSource)
                    .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.SelectedAccount, v => v.MainLst.SelectedItem)
                    .DisposeWith(SubscriptionDisposables);
                this.WhenAnyValue(x => x.ViewModel.SelectedAccount)
                    .Where(x => x != null)
                    .Subscribe(_ => MainLst.SelectedItem = null)
                    .DisposeWith(SubscriptionDisposables);

                this.OneWayBind(ViewModel, vm => vm.IsEnable, v => v.LoadingIndic.IsRunning, OppositeConverter.BooleanConverterFunc)
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.IsEnable, v => v.LoadingIndic.IsVisible, OppositeConverter.BooleanConverterFunc)
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.IsEnable, v => v.LoadingStck.IsVisible, OppositeConverter.BooleanConverterFunc)
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.IsEnable, v => v.MainRlt.IsEnabled)
                .DisposeWith(SubscriptionDisposables);
            });
        }


        private async Task CreateDayExecute()
        {
            ViewModel.IsEnable = false;
            await Task.Delay(100);
            _action = await Application.Current.MainPage.DisplayActionSheet(
                TranslationCodeExtension.GetTranslation("SelectDayAction"),
                TranslationCodeExtension.GetTranslation("CancelAction"),
                null,
                TranslationCodeExtension.GetTranslation("MissedDayAction"),
                TranslationCodeExtension.GetTranslation("WeekendAction"),
                TranslationCodeExtension.GetTranslation("WeekdayAction"));
            if (!_action.IsNullOrEmptyOrWhiteSpace()
                && _action != TranslationCodeExtension.GetTranslation("CancelAction")) 
                DayPicker.Focus();
            await Task.Delay(50);
            ViewModel.IsEnable = true;
        }
    }
}