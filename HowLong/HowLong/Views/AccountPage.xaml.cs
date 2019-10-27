using HowLong.Converters;
using HowLong.Extensions;
using HowLong.Services;
using ReactiveUI;
using System;
using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms.Xaml;

namespace HowLong.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountPage
    {
        public AccountPage() => InitializeComponent();

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel.Unsubscribe();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.WhenActivated(disposables =>
            {
                this.BindCommand(ViewModel, vm => vm.StartWorkCommand, v => v.StartWorkBtn);
                this.BindCommand(ViewModel, vm => vm.EndWorkCommand, v => v.EndWorkBtn);
                this.BindCommand(ViewModel, vm => vm.AddBreakCommand, v => v.AddDinnerBtn);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveBtn);
                this.BindCommand(ViewModel, vm => vm.AllCommand, v => v.AllGstr);
                this.BindCommand(ViewModel, vm => vm.CurrentCommand, v => v.CurrentGstr);

                this.Bind(ViewModel, vm => vm.StartWorkTime, v => v.StartWorkTmPck.Time);
                this.Bind(ViewModel, vm => vm.EndWorkTime, v => v.EndWorkTmPck.Time);

                this.OneWayBind(ViewModel, vm => vm.WorkDate, v => v.DateSpn.Text, date => Settings.Language.IsNullOrEmptyOrWhiteSpace()
                ? $"{date:d}"
                : date.ToString("d", CultureInfo.GetCultureInfo(Settings.Language)))
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.WorkDate, v => v.DaySpn.Text, date => DateService.DayShortName(date.DayOfWeek))
                .DisposeWith(SubscriptionDisposables);

                this.OneWayBind(ViewModel, vm => vm.CurrentOverWork, v => v.CurrentOverWorkLbl.Text, t => t > default(TimeSpan)
                ? "-" + t.ToString(@"dd\.hh\:mm\:ss")
                : t.ToString(@"dd\.hh\:mm\:ss"))
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.TotalOverWork, v => v.TotalOverWorkLbl.Text, t => t > default(TimeSpan)
                ? "-" + t.ToString(@"dd\.hh\:mm\:ss")
                : t.ToString(@"dd\.hh\:mm\:ss"))
                .DisposeWith(SubscriptionDisposables);

                this.OneWayBind(ViewModel, vm => vm.IsStarted, v => v.SaveBtn.IsVisible)
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.IsStarted, v => v.AddDinnerBtn.IsEnabled)
               .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.IsStarted, v => v.AddDinnerBtn.Opacity, v => v
                ? 1
                : 0)
              .DisposeWith(SubscriptionDisposables);

                this.OneWayBind(ViewModel, vm => vm.IsEnable, v => v.LoadingIndic.IsRunning, OppositeConverter.BooleanConverterFunc)
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.IsEnable, v => v.LoadingIndic.IsVisible, OppositeConverter.BooleanConverterFunc)
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.IsEnable, v => v.LoadingStck.IsVisible, OppositeConverter.BooleanConverterFunc)
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.IsEnable, v => v.MainRlt.IsEnabled)
                .DisposeWith(SubscriptionDisposables);

                this.OneWayBind(ViewModel, vm => vm.Breaks, v => v.MainLst.ItemsSource)
                    .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.SelectedBreak, v => v.MainLst.SelectedItem)
                    .DisposeWith(SubscriptionDisposables);
                this.WhenAnyValue(x => x.ViewModel.SelectedBreak)
                    .Where(x => x != null)
                    .Subscribe(_ => MainLst.SelectedItem = null)
                    .DisposeWith(SubscriptionDisposables);

                this.OneWayBind(ViewModel, vm => vm.CurrentAccounting.IsClosed, v => v.SaveBtn.Text, v => v
                ? TranslationCodeExtension.GetTranslation("SaveDayButton")
                : TranslationCodeExtension.GetTranslation("EndDayButton"))
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccounting.IsClosed, v => v.DateTitleSpn.Text, v => !v
                ? TranslationCodeExtension.GetTranslation("CurrentDayText") + " "
                : TranslationCodeExtension.GetTranslation("SelectedDayText") + " ")
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccounting.IsClosed, v => v.EndStck.IsVisible)
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.CurrentAccounting.IsClosed, v => v.TimerGrid.IsVisible, OppositeConverter.BooleanConverterFunc)
                .DisposeWith(SubscriptionDisposables);
            });
            ViewModel.Subscribe();
        }
    }
}