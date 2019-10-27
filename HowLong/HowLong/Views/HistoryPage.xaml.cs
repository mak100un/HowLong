using HowLong.Converters;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using HowLong.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowLong.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage
    {
        public HistoryPage() => InitializeComponent();

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
    }
}