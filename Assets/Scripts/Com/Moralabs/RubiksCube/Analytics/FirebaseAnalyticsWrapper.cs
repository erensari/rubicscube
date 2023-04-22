using System.Collections;
using System.Collections.Generic;
using Firebase.Analytics;
using UnityEngine;

namespace Com.Moralabs.RubiksCube.AnalyticsSystem {
    public class FirebaseAnalyticsWrapper {
        private FirebaseWrapper firebaseWrapper;

        public FirebaseAnalyticsWrapper(FirebaseWrapper firebaseWrapper) {
            this.firebaseWrapper = firebaseWrapper;
        }

        public void LogEvent(string name, params Parameter[] eventParameters) {
            if (firebaseWrapper.IsFirebaseReady.Value) {
                FirebaseAnalytics.LogEvent(name, eventParameters);
            }
        }

        public void SetUserId(string identifier) {
            if (firebaseWrapper.IsFirebaseReady.Value) {
                FirebaseAnalytics.SetUserId(identifier);
            }
        }

        public void SetUserProperty(string name, string property) {
            if (firebaseWrapper.IsFirebaseReady.Value) {
                FirebaseAnalytics.SetUserProperty(name, property);
            }
        }
    }
}
