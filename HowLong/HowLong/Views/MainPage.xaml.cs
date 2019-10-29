using HowLong.Converters;
using HowLong.DependencyServices;
using HowLong.Extensions;
using HowLong.Services;
using ReactiveUI;
using System;
using System.Globalization;
using System.Reactive.Disposables;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowLong.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            UpdateWorkingDay();
        }

        public void UpdateWorkingDay() => CurrentDateLbl.Text = Settings.Language.IsNullOrEmptyOrWhiteSpace()
               ? $"{DateTime.Now:d}, {DateService.IsWorkDay(DateTime.Today.DayOfWeek)}"
               : DateTime.Now.ToString("d", CultureInfo.GetCultureInfo(Settings.Language))
               + $", {DateService.IsWorkDay(DateTime.Today.DayOfWeek)}";
        public void UpdateLanguage()
        {
            UpdateWorkingDay();
            HeaderLbl.Text = TranslationCodeExtension.GetTranslation("MainHeaderText");
            CurrentBtn.Text = TranslationCodeExtension.GetTranslation("CurrentDayButton");
            HistoryBtn.Text = TranslationCodeExtension.GetTranslation("HistoryButton");
            SettingsBtn.Text = TranslationCodeExtension.GetTranslation("SettingsButton");
            RateTlbrItm.Text = TranslationCodeExtension.GetTranslation("EstimateApp");
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.WhenActivated(disposables =>
            {
                this.BindCommand(ViewModel, vm => vm.RateCommand, v => v.RateTlbrItm);
                this.BindCommand(ViewModel, vm => vm.HistoryCommand, v => v.HistoryBtn);
                this.BindCommand(ViewModel, vm => vm.CurrentCommand, v => v.CurrentBtn);
                this.BindCommand(ViewModel, vm => vm.SettingsCommand, v => v.SettingsBtn);
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

        protected override bool OnBackButtonPressed()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await DisplayAlert(TranslationCodeExtension.GetTranslation("ExitTitle"),
                        TranslationCodeExtension.GetTranslation("ExitText"),
                        TranslationCodeExtension.GetTranslation("YesExitText"),
                        TranslationCodeExtension.GetTranslation("NoExitText"));
                    if (result)
                        DependencyService.Get<ICloseApp>().Close();
                });
            }
            return true;
        }
    }
}