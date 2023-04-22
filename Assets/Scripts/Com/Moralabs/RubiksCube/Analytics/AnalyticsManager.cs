using System;
using System.Collections.Generic;
using Firebase.Analytics;

namespace Com.Moralabs.RubiksCube.AnalyticsSystem {
    public class AnalyticsManager {
        public enum ProgressionStatus {
            //Undefined progression
            Undefined = 0,
            // User started progression
            Start = 1,
            // User succesfully ended a progression
            Complete = 2,
            // User failed a progression
            Fail = 3
        }

        private FirebaseWrapper firebaseWrapper;

        public AnalyticsManager(FirebaseWrapper firebaseWrapper) {
            this.firebaseWrapper = firebaseWrapper;
        }

        public void LogEvent(string eventName, IDictionary<string, object> customFields = null, float? eventValue = null) {
            try {
                if (customFields != null) {
                    var parameters = new List<Parameter>();
                    foreach (var field in customFields) {
                        Parameter param = null;
                        if (field.Value is string stringValue) {
                            param = new Parameter(field.Key, stringValue);
                        }
                        else if (field.Value is int intValue) {
                            param = new Parameter(field.Key, intValue);
                        }
                        else if (field.Value is long longValue) {
                            param = new Parameter(field.Key, longValue);
                        }
                        else if (field.Value is float floatValue) {
                            param = new Parameter(field.Key, floatValue);
                        }
                        else if (field.Value is double doubleValue) {
                            param = new Parameter(field.Key, doubleValue);
                        }

                        if (param != null) {
                            parameters.Add(param);
                        }
                    }

                    if (eventValue.HasValue) {
                        parameters.Add(new Parameter(AnalyticEvents.EVENT_VALUE, eventValue.Value));
                    }

                    firebaseWrapper.FirebaseAnalyticsWrapper.LogEvent(eventName, parameters.ToArray());

                }
                else if (eventValue.HasValue) {
                    firebaseWrapper.FirebaseAnalyticsWrapper.LogEvent(eventName, new Parameter(AnalyticEvents.EVENT_VALUE, eventValue.Value));
                }
                else {
                    firebaseWrapper.FirebaseAnalyticsWrapper.LogEvent(eventName);
                }
            }
            catch (Exception ex) {
                UnityEngine.Debug.LogException(ex);
            }
        }

        public void LogResourceEvent(bool add, string resourceType, float amount, string itemType, string itemId) {
            try {
                var parameters = new List<Parameter>() {
                new Parameter(AnalyticEvents.FLOW_TYPE, add ? "add" : "spend"),
                new Parameter(AnalyticEvents.RESOURCE_TYPE, resourceType),
                new Parameter(AnalyticEvents.AMOUNT, amount),
                new Parameter(AnalyticEvents.ITEM_TYPE, itemType),
                new Parameter(AnalyticEvents.ITEM_ID, itemId)
            };

                firebaseWrapper.FirebaseAnalyticsWrapper.LogEvent(AnalyticEvents.RESOURCE_EVENT, parameters.ToArray());
            }
            catch (Exception ex) {
                UnityEngine.Debug.LogException(ex);
            }
        }

        public void LogErrorEvent(Exception exception) {
            try {
                firebaseWrapper.FirebaseCrashlyticsWrapper.LogException(exception);
            }
            catch (Exception ex) {
                UnityEngine.Debug.LogException(ex);
            }
        }

        public void LogErrorEvent(string message) {
            try {
                firebaseWrapper.FirebaseCrashlyticsWrapper.Log(message);
            }
            catch (Exception ex) {
                UnityEngine.Debug.LogException(ex);
            }
        }

        public void LogProgressionEvent(ProgressionStatus progressionStatus, float? time = null, params object[] progressions) {
            try {
                if (progressions == null) {
                    return;
                }

                var parameters = new List<Parameter>();
                for (int i = 0; i < progressions.Length; i++) {
                    Parameter param = null;
                    string key = AnalyticEvents.PROGRESSION_ + i;
                    object progression = progressions[i];

                    if (progression is string stringValue) {
                        param = new Parameter(key, stringValue);
                    }
                    else if (progression is int intValue) {
                        param = new Parameter(key, intValue);
                    }
                    else if (progression is long longValue) {
                        param = new Parameter(key, longValue);
                    }
                    else if (progression is float floatValue) {
                        param = new Parameter(key, floatValue);
                    }
                    else if (progression is double doubleValue) {
                        param = new Parameter(key, doubleValue);
                    }

                    if (param != null) {
                        parameters.Add(param);
                    }
                }
                parameters.Add(new Parameter(AnalyticEvents.PROGRESSION_STATUS, progressionStatus.ToString()));

                if (time.HasValue) {
                    parameters.Add(new Parameter(AnalyticEvents.SCORE, time.Value));
                }

                firebaseWrapper.FirebaseAnalyticsWrapper.LogEvent(AnalyticEvents.PROGRESSION_EVENT, parameters.ToArray());
            }
            catch (Exception ex) {
                UnityEngine.Debug.LogException(ex);
            }
        }
    }
}
