/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


using UnityEngine;
using UnityEngine.CrashReportHandler;
using Firebase.Crashlytics;


public class CrashHandler : MonoBehaviour {
    

    void Awake() {

        //disable crash sending when testing
        if (Debug.isDebugBuild) {
            CrashReportHandler.enableCaptureExceptions = false;
        }
    }
        

    void OnEnable() {
        Application.logMessageReceived += onMessageReceived;
    }

    void OnDisable() {
        Application.logMessageReceived -= onMessageReceived;
    }

    void onMessageReceived(string logString, string stackTrace, LogType type) {

        if (type == LogType.Exception) {

            Debug.LogWarning(logString);
            Debug.LogWarning(stackTrace);

            if (Application.isEditor) {
                //only break in editor to allow examination of the current scene state.
                Debug.Break();

            } else {    
                //force a crash to not allow player seeing a broken game
                Application.Quit();             //TODO crash with crashlytics

                //Utils.ForceCrash(ForcedCrashCategory.Abort);
            }

        } else if (type == LogType.Error) {

            //send a non fatal to crashlytics
            if (FirebaseInitManager.instance.hasResolvedDependencies()) {
                Crashlytics.LogException(new System.Exception("ERROR : " + logString + "\n" + stackTrace));
            }

        } else if (type == LogType.Warning) {

            //add a line in the next crash logs
            if (FirebaseInitManager.instance.hasResolvedDependencies()) {
                Crashlytics.Log("LOG :\n" + logString + "\n" + stackTrace);
            }
        }
    }

}
