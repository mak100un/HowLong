using HowLong.Converters;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using Xamarin.Forms.Xaml;

namespace HowLong.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkDaysPage
    {
        private readonly MainPage _mainPage;

        public WorkDaysPage(MainPage mainPage)
        {
            InitializeComponent();
            _mainPage = mainPage;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _mainPage.UpdateWorkingDay();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.WhenActivated(disposables =>
            {
                this.BindCommand(ViewModel, vm => vm.InfoCommand, v => v.InfoBtn);
                this.Bind(ViewModel, vm => vm.IsMonday, v => v.MondayCbx.IsChecked)
                .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.MondayStart, v => v.MondayStartTmPck.Time, TimeSpan.FromMinutes,
                    t => t.TotalMinutes)
                .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.MondayEnd, v => v.MondayEndTmPck.Time, TimeSpan.FromMinutes,
                    t => t.TotalMinutes)
                .DisposeWith(SubscriptionDisposables);

                this.Bind(ViewModel, vm => vm.IsTuesday, v => v.TuesdayCbx.IsChecked)
                .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.TuesdayStart, v => v.TuesdayStartTmPck.Time, TimeSpan.FromMinutes,
                    t => t.TotalMinutes)
                .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.TuesdayEnd, v => v.TuesdayEndTmPck.Time, TimeSpan.FromMinutes,
                    t => t.TotalMinutes)
                .DisposeWith(SubscriptionDisposables);

                this.Bind(ViewModel, vm => vm.IsWednesday, v => v.WednesdayCbx.IsChecked)
                .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.WednesdayStart, v => v.WednesdayStartTmPck.Time, TimeSpan.FromMinutes,
                    t => t.TotalMinutes)
                .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.WednesdayEnd, v => v.WednesdayEndTmPck.Time, TimeSpan.FromMinutes,
                    t => t.TotalMinutes)
                .DisposeWith(SubscriptionDisposables);

                this.Bind(ViewModel, vm => vm.IsThursday, v => v.ThursdayCbx.IsChecked)
                .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.ThursdayStart, v => v.ThursdayStartTmPck.Time, TimeSpan.FromMinutes,
                    t => t.TotalMinutes)
                .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.ThursdayEnd, v => v.ThursdayEndTmPck.Time, TimeSpan.FromMinutes,
                    t => t.TotalMinutes)
                .DisposeWith(SubscriptionDisposables);

                this.Bind(ViewModel, vm => vm.IsFriday, v => v.FridayCbx.IsChecked)
                .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.FridayStart, v => v.FridayStartTmPck.Time, TimeSpan.FromMinutes,
                    t => t.TotalMinutes)
                .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.FridayEnd, v => v.FridayEndTmPck.Time, TimeSpan.FromMinutes,
                    t => t.TotalMinutes)
                .DisposeWith(SubscriptionDisposables);

                this.Bind(ViewModel, vm => vm.IsSaturday, v => v.SaturdayCbx.IsChecked)
                .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.SaturdayStart, v => v.SaturdayStartTmPck.Time, TimeSpan.FromMinutes,
                    t => t.TotalMinutes)
                .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.SaturdayEnd, v => v.SaturdayEndTmPck.Time, TimeSpan.FromMinutes,
                    t => t.TotalMinutes)
                .DisposeWith(SubscriptionDisposables);

                this.Bind(ViewModel, vm => vm.IsSunday, v => v.SundayCbx.IsChecked)
                .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.SundayStart, v => v.SundayStartTmPck.Time, TimeSpan.FromMinutes,
                    t => t.TotalMinutes)
                .DisposeWith(SubscriptionDisposables);
                this.Bind(ViewModel, vm => vm.SundayEnd, v => v.SundayEndTmPck.Time, TimeSpan.FromMinutes,
                    t => t.TotalMinutes)
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
    }
}