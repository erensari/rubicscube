using OneSignalSDK;
using UnityEngine;
using UniRx;
using Com.Moralabs.RubiksCube.Util;

namespace Com.Moralabs.RubiksCube.NotificationSystem {
    public class PushNotifications {
        private OneSignalWrapper oneSignalWrapper;
        private bool notification;

        public bool Notifiacation {
            get {
                return notification;
            }
            set {
                notification = value;
                PlayerPrefs.SetInt(Constants.NOTIFICATION_PREFS, Notifiacation ? 1 : 0);

                if (Notifiacation) {
                    OpenNotification();
                }
                else {
                    CloseNotification();
                }
            }
        }

        public PushNotifications(OneSignalWrapper oneSignalWrapper) {
            this.oneSignalWrapper = oneSignalWrapper;
            Notifiacation = PlayerPrefs.GetInt(Constants.NOTIFICATION_PREFS, 1) == 1;
            oneSignalWrapper.OnNotification.Subscribe(OnNotification);
        }

        private void OnNotification(NotificationOpenedResult result) {
            if (result == null) {
                return;
            }

            if (result.action != null && result.action.actionID == "rate") {
                Rate();
            }
            else if (result.notification != null &&
                    result.notification.additionalData != null &&
                    result.notification.additionalData.ContainsKey("action")) {

                if ((string)result.notification.additionalData["action"] == "rate") {
                    Rate();
                }
            }
        }

        private void Rate() {
#if UNITY_ANDROID
            Application.OpenURL("market://details?id=com.moralabs.rubikscube");
#elif UNITY_IPHONE
            Application.OpenURL("itms-apps://itunes.apple.com/app/com.moralabs.rubikscube");
#endif
        }

        public void OpenNotification() {
            oneSignalWrapper.OpenNotification();
        }

        public void CloseNotification() {
            oneSignalWrapper.CloseNotification();
        }
    }
}

