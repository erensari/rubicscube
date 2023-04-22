using UniRx;
using UnityEngine;
using Zenject;

namespace Com.Moralabs.RubiksCube.AnalyticsSystem {
    public class FirebaseWrapper {
        private readonly ReactiveProperty<bool> isFirebaseReady = new ReactiveProperty<bool>(false);

        public IReadOnlyReactiveProperty<bool> IsFirebaseReady => isFirebaseReady;

        public FirebaseAnalyticsWrapper FirebaseAnalyticsWrapper {
            get;
            private set;
        }

        public FirebaseCrashlyticsWrapper FirebaseCrashlyticsWrapper {
            get;
            private set;
        }

        public FirebaseWrapper() {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available) {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    // Crashlytics will use the DefaultInstance, as well;
                    // this ensures that Crashlytics is initialized.
                    Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

                    // Set a flag here for indicating that your project is ready to use Firebase.
                    isFirebaseReady.Value = true;
                }
                else {
                    Debug.LogError(string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });

            FirebaseCrashlyticsWrapper = new FirebaseCrashlyticsWrapper(this);
            FirebaseAnalyticsWrapper = new FirebaseAnalyticsWrapper(this);
        }
    }
}
