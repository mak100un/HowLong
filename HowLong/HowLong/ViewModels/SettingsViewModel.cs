using HowLong.Data;
using HowLong.Extensions;
using HowLong.Navigation;
using HowLong.Services;
using HowLong.Values;
using Plugin.Messaging;
using Plugin.Toast;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HowLong.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly Func<WorkDaysViewModel> _workDaysFactory;
        private readonly TimeAccountingContext _timeAccountingContext;

        public ReactiveCommand<Unit, Unit> ClearCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> WorkDaysCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> LightCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> DarkCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> SupportCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> VkCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> InstagramCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> GitCommand { get; internal set; }
        public ReactiveCommand<Unit, Unit> DevelopCommand { get; internal set; }
        [Reactive]
        public Color GitBackgroundColor { get; set; } = Color.Transparent;
        [Reactive]
        public Color InstagramBackgroundColor { get; set; } = Color.Transparent;
        [Reactive]
        public Color VkBackgroundColor { get;  set; } = Color.Transparent;

        public SettingsViewModel
        (
            INavigationService navigationService,
            TimeAccountingContext timeAccountingContext,
            Func<WorkDaysViewModel> workDaysFactory
        )
        {
            _workDaysFactory = workDaysFactory;
            _timeAccountingContext = timeAccountingContext;
            _navigationService = navigationService;
            ClearCommand = ReactiveCommand.CreateFromTask(ClearExecuteAsync);
            WorkDaysCommand = ReactiveCommand.CreateFromTask(WorkDaysExecuteAsync);
            DevelopCommand = ReactiveCommand.CreateFromTask(DevelopExecuteAsync);
            LightCommand = ReactiveCommand.Create(() =>
            {
                if (!Settings.IsDark) return;
                Settings.IsDark = false;
                ThemeService.ChangeToLight();
            });
            DarkCommand = ReactiveCommand.Create(() =>
            {
                if (Settings.IsDark) return;
                Settings.IsDark = true;
                ThemeService.ChangeToDark();
            });
            WorkDaysCommand = ReactiveCommand.CreateFromTask(WorkDaysExecuteAsync);
            InstagramCommand = ReactiveCommand.CreateFromTask(async() =>
            {
                InstagramBackgroundColor = Color.FromHex("#D4F4FF");
                await Task.Delay(150);
                await Browser.OpenAsync(new Uri(BaseValue.CreatorInstagram));
                InstagramBackgroundColor = Color.Transparent;
            });
            VkCommand = ReactiveCommand.CreateFromTask(async() => 
            {
                VkBackgroundColor = Color.FromHex("#D4F4FF");
                await Task.Delay(150);
                await Browser.OpenAsync(new Uri(BaseValue.CreatorVk));
                VkBackgroundColor = Color.Transparent;
            });
            GitCommand = ReactiveCommand.CreateFromTask(async() =>
            {
                GitBackgroundColor = Color.FromHex("#D4F4FF");
                await Task.Delay(150);
                await Browser.OpenAsync(new Uri(BaseValue.CreatorGithub));
                GitBackgroundColor = Color.Transparent;
            });
            SupportCommand = ReactiveCommand.Create(()=> 
            {
                var emailMessenger = CrossMessaging.Current.EmailMessenger;
                if (emailMessenger.CanSendEmail) emailMessenger.SendEmail(
                    BaseValue.SupportEmail,
                    TranslationCodeExtension.GetTranslation("MessageToSupport"),
                    string.Empty);
            });
        }

        private static async Task DevelopExecuteAsync()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await Application.Current.MainPage.DisplayAlert(TranslationCodeExtension.GetTranslation("ErrorConnectionTitle"),
                    TranslationCodeExtension.GetTranslation("ErrorConnectionText"),
                    TranslationCodeExtension.GetTranslation("OkText"));
                return;
            }
            if (!CrossInAppBilling.IsSupported)
            {
                await Application.Current.MainPage.DisplayAlert(TranslationCodeExtension.GetTranslation("PaymentNotSupportedTitle"),
                    TranslationCodeExtension.GetTranslation("PaymentNotSupportedText"),
                    TranslationCodeExtension.GetTranslation("OkText"));
                return;
            }
            try
            {
                RePurchase:
                var billing = CrossInAppBilling.Current;

                try
                {
                    var connected = await billing.ConnectAsync();
                    if (!connected)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            TranslationCodeExtension.GetTranslation("StoreServicesConnectProblemTitle"),
                            TranslationCodeExtension.GetTranslation("StoreServicesConnectProblemText"),
                            TranslationCodeExtension.GetTranslation("OkText"));
                        return;
                    }

                    var purchase = await billing.PurchaseAsync
                    (
                        BaseValue.DevelopProductId,
                        ItemType.InAppPurchase,
                        "inAppPayLoad"
                    );
                    if (purchase != null && purchase.State == PurchaseState.Purchased)
                    {
                        await CrossInAppBilling.Current.ConsumePurchaseAsync
                        (
                            purchase.ProductId,
                            purchase.PurchaseToken
                        );
                        CrossToastPopUp.Current.ShowCustomToast
                        (
                            TranslationCodeExtension.GetTranslation("DevelopThanksText"),
                            "#00AB00",
                            "#FFFFFF"
                        );
                    }
                }
                catch (InAppBillingPurchaseException purchaseEx)
                {
                    switch (purchaseEx.PurchaseError)
                    {
                        case PurchaseError.AlreadyOwned:
                            await CrossInAppBilling.Current.ConsumePurchaseAsync
                            (
                                BaseValue.DevelopProductId,
                                ItemType.InAppPurchase,
                                "inAppPayLoad"
                            );
                            await billing.DisconnectAsync();
                            goto RePurchase;
                        case PurchaseError.UserCancelled: break;
                        default:
                            await Application.Current.MainPage.DisplayAlert
                            (
                                TranslationCodeExtension.GetTranslation("StoreServicesUnavailableTitle"),
                                TranslationCodeExtension.GetTranslation("StoreServicesUnavailableText"),
                                TranslationCodeExtension.GetTranslation("OkText")
                            );
                            break;
                    }
                }
                finally
                {
                    await billing.DisconnectAsync();
                }
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert
                (
                    TranslationCodeExtension.GetTranslation("StoreServicesUnavailableTitle"),
                    TranslationCodeExtension.GetTranslation("StoreServicesUnavailableText"),
                    TranslationCodeExtension.GetTranslation("OkText")
                );
            }
        }

        private async Task WorkDaysExecuteAsync()
        {
            IsEnable = false;
            await Task.Delay(100);
            await _navigationService.NavigateToAsync
                   (
                _workDaysFactory()
                   );
            await Task.Delay(150);
            IsEnable = true;
        }

        private async Task ClearExecuteAsync()
        {
            IsEnable = false;
            await Task.Delay(100);
            var result = await Application.Current.MainPage.DisplayAlert(
                TranslationCodeExtension.GetTranslation("ClearHistoryTitle"),
                TranslationCodeExtension.GetTranslation("ClearHistoryText"),
                TranslationCodeExtension.GetTranslation("ClearHistoryButton"),
                TranslationCodeExtension.GetTranslation("NoText")
                );
            if (!result)
            {
                IsEnable = true;
                return;
            }
            await _timeAccountingContext.Database.EnsureDeletedAsync()
                .ConfigureAwait(false);
            await _timeAccountingContext.Database.EnsureCreatedAsync()
                .ConfigureAwait(false);

            CrossToastPopUp.Current.ShowCustomToast
                (
                TranslationCodeExtension.GetTranslation("HistorySuccessClearText"),
                "#00AB00",
                "#FFFFFF"
                );
            await Task.Delay(50);
            IsEnable = true;
        }
    }
}
