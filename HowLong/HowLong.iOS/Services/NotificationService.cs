using Foundation;
using Plugin.Vibrate;
using System;
using System.Linq;
using UIKit;
using UserNotifications;

namespace HowLong.iOS.Services
{
    public class NotificationService
    {
        private const string NotificationKey = "NotificationKey";

        public void Show(string title, string body, int id, DateTime notifyTime)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                using(var trigger = UNCalendarNotificationTrigger.CreateTrigger(GetNsDateComponentsFromDateTime(notifyTime), false))
                    ShowUserNotification(title, body, id, trigger);
            else
            {
                var notification = new UILocalNotification
                {
                    FireDate = (NSDate)notifyTime,
                    AlertTitle = title,
                    AlertBody = body,
                    UserInfo = NSDictionary.FromObjectAndKey(NSObject.FromObject(id), NSObject.FromObject(NotificationKey))
                };

                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            }
        }

        public void Cancel(int id)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.RemovePendingNotificationRequests(new[] { id.ToString() });
                UNUserNotificationCenter.Current.RemoveDeliveredNotifications(new[] { id.ToString() });
            }
            else
            {
                var notifications = UIApplication.SharedApplication.ScheduledLocalNotifications;
                var notification = notifications.Where(n => n.UserInfo.ContainsKey(NSObject.FromObject(NotificationKey)))
                    .FirstOrDefault(n => n.UserInfo[NotificationKey].Equals(NSObject.FromObject(id)));

                if (notification != null)
                    UIApplication.SharedApplication.CancelLocalNotification(notification);
            }
        }

        private static void ShowUserNotification(string title, string body, int id, UNNotificationTrigger trigger)
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(10, 0)) return;
            
            var content = new UNMutableNotificationContent()
            {
                Title = title,
                Body = body
            };

            var request = UNNotificationRequest.FromIdentifier(id.ToString(), content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request,
                error => CrossVibrate.Current.Vibration(TimeSpan.FromMilliseconds(150)));
        }

        private static NSDateComponents GetNsDateComponentsFromDateTime(DateTime dateTime) => 
            new NSDateComponents
            {
                Month = dateTime.Month,
                Day = dateTime.Day,
                Year = dateTime.Year,
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                Second = dateTime.Second
            };
    }
}