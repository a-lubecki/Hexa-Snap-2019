/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.IO;
using UnityEngine;

#if UNITY_IPHONE
using System.Runtime.InteropServices;
#endif


// http://www.theappguruz.com/blog/android-native-popup-using-unity
// http://www.theappguruz.com/blog/ios-native-popup-using-unity

// http://eppz.eu/blog/unity-android-plugin-tutorial-3/

// http://www.theappguruz.com/blog/general-sharing-in-android-ios-in-unity
// https://github.com/ChrisMaire/unity-native-sharing/blob/master/Assets/Plugins/iOS/iOSNativeShare.m


/**
 * How to export the Android lib to Unity :
 * >> go to Android Studio
 * >> open gradle panel
 * >> launch :app/Tasks/build/assembleRelease
 * >> copy app/build/output/aar/app-release.aar
 * >> paste to Unity in HexaSnap/Assets/Plugins/Android/
 * 
 * How to export the iOS lib to Unity :
 * copy .h and .mm files to Assets >> Plugin >> IOS
 */

public class NativeCallsManager {

    #region popup

    #if UNITY_ANDROID
    private static ReturnType callAndroidStaticSynchronous<ReturnType>(AndroidStaticMethod method, params object[] args) {

        try {

            AndroidJavaObject bridge = new AndroidJavaObject(method.className);

            return bridge.CallStatic<ReturnType>(method.methodName, args);

        } catch (System.Exception ex) {
            Debug.LogWarning(ex.Message);
        }

        return default;
    }

    private static void callAndroidStatic(AndroidStaticMethod method, params object[] args) {

        try {

            AndroidJavaObject bridge = new AndroidJavaObject(method.className);

            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                bridge.CallStatic(method.methodName, args);
            }));

        } catch (System.Exception ex) {
            Debug.LogWarning(ex.Message);
        }
    }
    #endif

    #if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void call_showAlertDialog(string title, string message, string negativeButtonText, string positiveButtonText);
    	
	[DllImport("__Internal")]
    private static extern void call_showActionSheetDialog(string title, string message, string negativeButtonText, string[] buttonTexts, int buttonTextsCount);
    #endif

    public static void showAlertDialog(string title, string message, string negativeButtonText, string positiveButtonText) {

        #if UNITY_ANDROID
        callAndroidStatic(
            new AndroidStaticMethod("com.hexasnap.utils.NativePopupManager", "showAlertDialog"),
            title, message, negativeButtonText, positiveButtonText);
        #elif UNITY_IPHONE
		call_showAlertDialog(title, message, negativeButtonText, positiveButtonText);
        #endif
    }

    public static void showActionSheetDialog(string title, string message, string negativeButtonText, string[] buttonTexts) {
        
        #if UNITY_ANDROID
        callAndroidStatic(
            new AndroidStaticMethod("com.hexasnap.utils.NativePopupManager", "showActionSheetDialog"), 
            title, message, negativeButtonText, buttonTexts);
        #elif UNITY_IPHONE
        call_showActionSheetDialog(title, message, negativeButtonText, buttonTexts, buttonTexts.Length);
        #endif
    }

    #endregion


    #region share

    #if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern void call_share(string chooserTitle, string subject, string message, string gameUrl, string imagePath);
    #endif
    
    public static void share(string chooserTitle, string subject, string message, string gameUrl, string imagePath) {
        
        #if UNITY_ANDROID
        callAndroidStatic(new AndroidStaticMethod("com.hexasnap.utils.NativeShareManager", "share"),
            chooserTitle, subject, message, gameUrl, imagePath);
        #elif UNITY_IPHONE
		call_share(chooserTitle, subject, message, gameUrl, imagePath);		
        #endif
    }

    #endregion


    #region app signature

    #if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern System.IntPtr call_getAppOrigin();

    [DllImport("__Internal")]
    private static extern System.IntPtr call_getAppSignatures();
    #endif

    public static string getAppOrigin() {

        #if UNITY_ANDROID
        return callAndroidStaticSynchronous<string>(new AndroidStaticMethod("com.hexasnap.utils.NativeUtils", "getAppOrigin"));
        #elif UNITY_IPHONE
        return marshalString(call_getAppOrigin());
        #else
        return null;
        #endif
    }

    public static string[] getAppSignatures() {

        #if UNITY_ANDROID
        return callAndroidStaticSynchronous<string[]>(new AndroidStaticMethod("com.hexasnap.utils.NativeUtils", "getAppSignatures"));
        #elif UNITY_IPHONE
        string signatures = marshalString(call_getAppSignatures());
        return (signatures != null) ? new string[] { signatures } : new string[0];
        #else
        return null;
        #endif
    }

    #endregion

#if UNITY_IPHONE
    private static string marshalString(System.IntPtr p) {

        if (p == System.IntPtr.Zero) {
            return null;
        }

        return Marshal.PtrToStringAnsi(p);
    }
#endif
}


#if UNITY_ANDROID
public struct AndroidStaticMethod {

    public readonly string className;
    public readonly string methodName;

    public AndroidStaticMethod(string className, string methodName) {
        this.className = className;
        this.methodName = methodName;
    }

}
#endif
