using System;
using Firebase.Crashlytics;

namespace Com.Moralabs.RubiksCube.AnalyticsSystem {
    public class FirebaseCrashlyticsWrapper {
        private FirebaseWrapper firebaseWrapper;

        public FirebaseCrashlyticsWrapper(FirebaseWrapper firebaseWrapper) {
            this.firebaseWrapper = firebaseWrapper;
        }

        public void Log(string message) {
            if (firebaseWrapper.IsFirebaseReady.Value) {
                Crashlytics.Log(message);
            }

#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(message);
#endif
        }

        public void LogException(Exception ex) {
            if (firebaseWrapper.IsFirebaseReady.Value) {
                Crashlytics.LogException(ex);
            }

#if UNITY_EDITOR
            UnityEngine.Debug.LogException(ex);
#endif
        }

        public void SetUserId(string identifier) {
            if (firebaseWrapper.IsFirebaseReady.Value) {
                Crashlytics.SetUserId(identifier);
            }
        }
    }
}