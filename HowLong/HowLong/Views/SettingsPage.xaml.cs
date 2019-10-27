using HowLong.Converters;
using HowLong.Extensions;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms.Xaml;

namespace HowLong.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage
    {
        private readonly MainPage _mainPage;

        public SettingsPage(MainPage mainPage)
        {
            InitializeComponent();
            _mainPage = mainPage;
            InitHandlers();
        }

        private void InitHandlers()
        {
            Observable.FromEventPattern<EventHandler, EventArgs>(
               h => RuBtn.Clicked += h,
               h => RuBtn.Clicked -= h)
                .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
                .Subscribe(_ => 
                {
                    if (Settings.Language == "ru") return;
                    Settings.Language = "ru";
                    UpdateLanguage();
                    _mainPage.UpdateLanguage();
                });
            Observable.FromEventPattern<EventHandler, EventArgs>(
               h => EnBtn.Clicked += h,
               h => EnBtn.Clicked -= h)
                .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    if (Settings.Language == "en") return;
                    Settings.Language = "en";
                    UpdateLanguage();
                    _mainPage.UpdateLanguage();
                });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.WhenActivated(disposables =>
            {
                this.BindCommand(ViewModel, vm => vm.LightCommand, v => v.LightThemeBtn);
                this.BindCommand(ViewModel, vm => vm.DarkCommand, v => v.DarkThemeBtn);
                this.BindCommand(ViewModel, vm => vm.WorkDaysCommand, v => v.WorkDaysBtn);
                this.BindCommand(ViewModel, vm => vm.ClearCommand, v => v.ClearBtn);
                this.BindCommand(ViewModel, vm => vm.SupportCommand, v => v.SupportBtn);
                this.BindCommand(ViewModel, vm => vm.InstagramCommand, v => v.InstagramGstr);
                this.BindCommand(ViewModel, vm => vm.VkCommand, v => v.VkGstr);
                this.BindCommand(ViewModel, vm => vm.GitCommand, v => v.GitGstr);

                this.OneWayBind(ViewModel, vm => vm.VkBackgroundColor, v => v.VkSpan.BackgroundColor)
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.InstagramBackgroundColor, v => v.InstagramSpan.BackgroundColor)
                .DisposeWith(SubscriptionDisposables);
                this.OneWayBind(ViewModel, vm => vm.GitBackgroundColor, v => v.GitSpan.BackgroundColor)
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

        private void UpdateLanguage()
        {
            HeaderLbl.Text = TranslationCodeExtension.GetTranslation("SettingsText");
            ThemeLbl.Text = TranslationCodeExtension.GetTranslation("ThemeText");
            LightThemeBtn.Text = TranslationCodeExtension.GetTranslation("LightThemeButton");
            DarkThemeBtn.Text = TranslationCodeExtension.GetTranslation("DarkThemeButton");
            LangLbl.Text = TranslationCodeExtension.GetTranslation("LanguageText");
            WorkDaysBtn.Text = TranslationCodeExtension.GetTranslation("WeekdaysButton");
            ClearBtn.Text = TranslationCodeExtension.GetTranslation("ClearHistoryButton");
            SupportBtn.Text = TranslationCodeExtension.GetTranslation("SupportButton");
        }

       
    }
}