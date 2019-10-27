using System;
using HowLong.DependencyServices;
using HowLong.Extensions;
using HowLong.iOS.DependencyServices;
using HowLong.iOS.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(ShowNotify))]
namespace HowLong.iOS.DependencyServices
{
    public class ShowNotify : IShowNotify
    {
        public void SetEnd(double minutes) => new NotificationService().Show(
            TranslationCodeExtension.GetTranslation("DayIsOverTitle"),
            TranslationCodeExtension.GetTranslation("DayIsOverText"),
            1,
            DateTime.Now.AddMinutes(minutes));

        public void SetHalf(double minutes) => new NotificationService().Show(
            TranslationCodeExtension.GetTranslation("HalfWorkTitle"),
            TranslationCodeExtension.GetTranslation("HalfWorkText"),
            2,
            DateTime.Now.AddMinutes(minutes));

        public void CancelAll()
        {
            var notificationService = new NotificationService();
            notificationService.Cancel(1);
            notificationService.Cancel(2);
        }
    }
}