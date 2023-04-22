using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Morautils.AutoBuild {
    public class BuildScript : MonoBehaviour {
        #region iOS
        /// <summary>
        /// iOS files path
        /// </summary>
        public static string IOS_FOLDER = "./Builds/iOS/";

        /// <summary>
        /// Get active scene list
        /// </summary>
        public static string[] GetScenes() {
            List<string> scenes = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++) {
                if (EditorBuildSettings.scenes[i].enabled) {
                    scenes.Add(EditorBuildSettings.scenes[i].path);
                }
            }
            return scenes.ToArray();
        }

        /// <summary>
        /// Run iOS development build
        /// </summary>
        public static void iOSDevelopment() {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "DEV");
            PlayerSettings.SplashScreen.showUnityLogo = false;
            PlayerSettings.SplashScreen.show = false;
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            EditorUserBuildSettings.development = true;

            BuildReport report = BuildPipeline.BuildPlayer(GetScenes(), IOS_FOLDER, BuildTarget.iOS, BuildOptions.None);
            int code = (report.summary.result == BuildResult.Succeeded) ? 0 : 1;
            EditorApplication.Exit(code);
        }

        /// <summary>
        /// Run iOS release build
        /// </summary>
        public static void iOSRelease() {

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.SplashScreen.showUnityLogo = false;
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.iOS.appleDeveloperTeamID = "9R3975Z9XK";
            BuildReport report = BuildPipeline.BuildPlayer(GetScenes(), IOS_FOLDER, BuildTarget.iOS, BuildOptions.None);
            int code = (report.summary.result == BuildResult.Succeeded) ? 0 : 1;
            EditorApplication.Exit(code);
        }
        #endregion

        #region Android
        /// <summary>
        /// Android files path
        /// </summary>
        public static string ANDROID_FOLDER = "./Builds/Android/";

        /// <summary>
        /// Run Android development build
        /// </summary>
        public static void AndroidDevelopment() {
            var args = FindArgs();

            PlayerSettings.Android.keystorePass = args.keystorePass;
            PlayerSettings.Android.keyaliasName = args.keyaliasName;
            PlayerSettings.Android.keyaliasPass = args.keyaliasPass;
            PlayerSettings.SplashScreen.showUnityLogo = false;
            PlayerSettings.SplashScreen.show = false;
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

            bool switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            if (switchResult) {
                System.Console.WriteLine("[JenkinsBuild] Successfully changed Build Target to: Android");
            }
            else {
                System.Console.WriteLine("[JenkinsBuild] Unable to change Build Target to: Android Exiting...");
                return;
            }

            BuildReport buildReport = BuildPipeline.BuildPlayer(GetScenes(), ANDROID_FOLDER + "Dev/build.aab", BuildTarget.Android, BuildOptions.None);
            BuildSummary buildSummary = buildReport.summary;
            if (buildSummary.result == BuildResult.Succeeded) {
                System.Console.WriteLine("[JenkinsBuild] Build Success: Time:" + buildSummary.totalTime + " Size:" + buildSummary.totalSize + " bytes");
            }
            else {
                System.Console.WriteLine("[JenkinsBuild] Build Failed: Time:" + buildSummary.totalTime + " Total Errors:" + buildSummary.totalErrors);
            }
        }

        /// <summary>
        /// Run Android production build
        /// </summary>
        public static void AndroidRelease() {
            var args = FindArgs();

            PlayerSettings.Android.keystorePass = args.keystorePass;
            PlayerSettings.Android.keyaliasName = args.keyaliasName;
            PlayerSettings.Android.keyaliasPass = args.keyaliasPass;
            EditorUserBuildSettings.buildAppBundle = true;
            PlayerSettings.SplashScreen.showUnityLogo = false;
            PlayerSettings.SplashScreen.show = false;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

            bool switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            if (switchResult) {
                System.Console.WriteLine("[JenkinsBuild] Successfully changed Build Target to: Android");
            }
            else {
                System.Console.WriteLine("[JenkinsBuild] Unable to change Build Target to: Android Exiting...");
                return;
            }

            BuildReport buildReport = BuildPipeline.BuildPlayer(GetScenes(), ANDROID_FOLDER + "Prod/build.aab", BuildTarget.Android, BuildOptions.None);
            BuildSummary buildSummary = buildReport.summary;
            if (buildSummary.result == BuildResult.Succeeded) {
                System.Console.WriteLine("[JenkinsBuild] Build Success: Time:" + buildSummary.totalTime + " Size:" + buildSummary.totalSize + " bytes");
            }
            else {
                System.Console.WriteLine("[JenkinsBuild] Build Failed: Time:" + buildSummary.totalTime + " Total Errors:" + buildSummary.totalErrors);
            }
        }
        #endregion

        private static Args FindArgs() {
            var returnValue = new Args();

            // find: -executeMethod
            //   +1: JenkinsBuild.BuildMacOS
            //   +2: FindTheGnome
            //   +3: D:\Jenkins\Builds\Find the Gnome\47\output
            string[] args = System.Environment.GetCommandLineArgs();
            var execMethodArgPos = -1;
            bool allArgsFound = false;
            for (int i = 0; i < args.Length; i++) {
                if (args[i] == "-executeMethod") {
                    execMethodArgPos = i;
                }
                var realPos = execMethodArgPos == -1 ? -1 : i - execMethodArgPos - 2;
                if (realPos < 0 || realPos > 2) {
                    continue;
                }
                else if (realPos == 0) {
                    returnValue.keystorePass = args[i];
                }
                else if (realPos == 1) {
                    returnValue.keyaliasName = args[i];
                }
                else if (realPos == 2) {
                    returnValue.keyaliasPass = args[i];
                }
            }

            if (!allArgsFound)
                System.Console.WriteLine("[JenkinsBuild] Incorrect Parameters for -executeMethod Format: -executeMethod <method name> <keystorePass> <keyaliasName> <keyaliasPass>");

            return returnValue;
        }

        private class Args {
            public string keystorePass = "keystorePass";
            public string keyaliasName = "keyaliasName";
            public string keyaliasPass = "keyaliasPass";

            override public string ToString() {
                return "[JenkinsBuild] Args : " + keystorePass + " - " + keyaliasName + " - " + keyaliasPass;
            }
        }

    }
}
