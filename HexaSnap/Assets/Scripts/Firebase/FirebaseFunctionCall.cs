/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Functions;
using Firebase.Extensions;


public class FirebaseFunctionCall {


    private static readonly string KEY_ERROR_CODE = "errorCode";


    public bool isPrior;

    public readonly FirebaseFunctionCallMergeStrategy mergeStrategy;

    public readonly string methodName;
    public readonly bool needsToBeLogged;
    private readonly Dictionary<string, object> args;
    private readonly Action<Dictionary<object, object>> endCall;
    public readonly Action onDone;
    public readonly Action<Exception> onError;


    public FirebaseFunctionCall(bool isPrior, FirebaseFunctionCallMergeStrategy mergeStrategy,
                                string methodName, bool needsToBeLogged, Dictionary<string, object> args, 
                                Action<Dictionary<object, object>> endCall, Action onDone, Action<Exception> onError) {

        if (string.IsNullOrEmpty(methodName)) {
            throw new ArgumentException();
        }

        this.isPrior = isPrior;

        this.mergeStrategy = mergeStrategy;

        this.methodName = methodName;
        this.needsToBeLogged = needsToBeLogged;
        this.args = args;
        this.endCall = endCall;
        this.onDone = onDone;
        this.onError = onError;
    }

    public void resolveCallCallback(Exception e = null) {

        if (e == null) {
            onDone?.Invoke(); 
        } else {
            onError?.Invoke(e);   
        }
    }

    public void processCall(Action completion) {

        if (!FirebaseInitManager.instance.hasResolvedDependencies()) {
            //not ready
            completion?.Invoke();
            resolveCallCallback(new InvalidOperationException());
            return;
        }

        if (FirebaseFunctionsManager.isSendingDisabled) {
            //not available, nothing to send
            completion?.Invoke();
            resolveCallCallback(new InvalidOperationException());
            return;
        }

        if (needsToBeLogged && !LoginManager.Instance.isLoggedInFacebook()) {
            //not logged, nothing to send
            completion?.Invoke();
            resolveCallCallback(new InvalidOperationException());
            return;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable) {
            //fix crash of firebase when there is no network
            completion?.Invoke();
            resolveCallCallback(new InvalidOperationException());
            return;
        }

        Debug.Log("Begin call firebase function : " + methodName);

        try {
            
            FirebaseFunctions.DefaultInstance.GetHttpsCallable(methodName)
                             .CallAsync(args)
                             .ContinueWithOnMainThread((task) => {
                
                Debug.Log("End call firebase function : " + methodName);

                var data = task?.Result?.Data;

                onCallProcessed(task, completion);

                if (data != null && data is string) {
                    return (string)data;
                }

                return "";
            });

        } catch (Exception e) {

            Debug.Log("Error on call firebase function : " + methodName);

            completion?.Invoke();
            resolveCallCallback(e);
        }
    }

    protected void onCallProcessed(Task<HttpsCallableResult> task, Action completion) {

        Debug.Log("FirebaseFunctionCall.onCallProcessed : " + methodName);

        if (task == null) {

            Debug.Log("Null task : " + methodName);

            completion?.Invoke();
            resolveCallCallback(new InvalidOperationException());
            return;
        }

        if (task.IsCanceled) {

            Debug.Log("Task canceled : " + methodName);

            completion?.Invoke();
            return;
        }

        if (task.IsFaulted) {

            Debug.LogWarning("Task IsFaulted : " + methodName + " => " + task.Exception);

            completion?.Invoke();
            resolveCallCallback(task.Exception);
            return;
        }

        var data = task.Result?.Data as Dictionary<object, object>;

        if (data == null) {

            Debug.Log("No data : " + methodName);

            completion?.Invoke();
            resolveCallCallback();
            return;
        }

        if (data.ContainsKey(KEY_ERROR_CODE)) {

            var code = -1;
            var rawCode = data[KEY_ERROR_CODE] as long?;
            if (rawCode != null && rawCode.HasValue) {
                code = (int) rawCode.Value;
            }

            if (code == 401) {

                //handle bad token
                LoginManager.Instance.logoutFromFacebook();

                GameHelper.Instance.getGameManager().reactivateAds();

                return;
            }

            if (code == 451) {

                //handle bad app origin/signature
                GameHelper.Instance.getNativePopupManager().show(
                    Tr.get("P6.Title"),
                    Tr.get("P6.Message"),
                    Tr.get("P.Continue"),
                    () => {

                        Application.OpenURL(Constants.URL_SITE_HEXASNAP);

                        Application.Quit();
                    }
                );

                return;
            }

            completion?.Invoke();
            resolveCallCallback(new FirebaseFunctionCallException(code));
            return;
        }

        if (endCall != null) {

            Debug.Log("End call : " + methodName);

            //disable during game objects assigning to avoid sending again the same data to firebase
            FirebaseFunctionsManager.isSendingDisabled = true;

            try {
                endCall(data);

            } catch (Exception e) {
                Debug.LogWarning("End call failed : " + methodName + " => " + e);
            }

            //enable again for next calls
            FirebaseFunctionsManager.isSendingDisabled = false;
        }

        //finished with data
        completion?.Invoke();
        resolveCallCallback();
    }

}

