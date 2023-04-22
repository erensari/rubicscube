using OneSignalSDK;
using UniRx;
using UnityEngine;
using Notification = OneSignalSDK.Notification;

namespace Com.Moralabs.RubiksCube.Util {
    public class OneSignalWrapper {
        private const string APP_ID = "0e241c75-7323-4aa6-9d82-2fb61cc2cfc0";

        private readonly ReactiveProperty<NotificationOpenedResult> onNotification = new ReactiveProperty<NotificationOpenedResult>(null);

        public IReadOnlyReactiveProperty<NotificationOpenedResult> OnNotification => onNotification;

        public OneSignalWrapper() {
            OneSignal.Default.Initialize(APP_ID);
            OneSignal.Default.NotificationOpened += HandleNotificationOpened;
            OneSignal.Default.NotificationWillShow += HandleNotificationWillShow;
            OneSignal.Default.SetLaunchURLsInApp(false);
            OneSignal.Default.SetExternalUserId(SystemInfo.deviceUniqueIdentifier);

            RegisterNotifications();
        }

        private OneSignalSDK.Notification HandleNotificationWillShow(Notification notification) {
            return null;
        }

        // Gets called when the player opens the notification.
        private void HandleNotificationOpened(NotificationOpenedResult result) {
            onNotification.Value = result;
        }

        public void RegisterNotifications() {
            OneSignal.Default.PromptForPushNotificationsWithUserResponse();
        }

        public void OpenNotification() {
            OneSignal.Default.InAppMessagesArePaused = false;
        }

        public void CloseNotification() {
            OneSignal.Default.InAppMessagesArePaused = true;
        }

        public void SetUserId(string identifier) {
            OneSignal.Default.SetExternalUserId(identifier);
        }
    }
}
