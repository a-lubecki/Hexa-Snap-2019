/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Firebase.Auth;
using Firebase.Extensions;


public class LoginManager {


	//singleton
	public static readonly LoginManager Instance = new LoginManager();


    private FirebaseAuth firebaseAuth;

	public bool isLoggingInToFacebook { get; private set; }
	public bool isLoggingInToSpecific { get; private set; }

    public bool hasSyncDataOnce { get; private set; }
    public bool wasSignUp { get; private set; }


    private LoginManager() {
        firebaseAuth = FirebaseAuth.DefaultInstance;
    }

	public bool isLoggedInFacebook() {
        return firebaseAuth.CurrentUser != null;
	}

    public FirebaseUser getFirebaseUser() {
        return firebaseAuth.CurrentUser;
    }

    public string getFirebaseUserId() {

        var userId = getFirebaseUser()?.UserId;

        if (userId == null || userId.Length <= 0) {
            return null;
        }

        return userId;
    }

    public void logoutFromFacebook() {
        logoutFromFacebook(false);
    }

    private void logoutFromFacebook(bool isPreLogin) {
        
        isLoggingInToFacebook = false;

        FB.LogOut();
        firebaseAuth.SignOut();

        hasSyncDataOnce = false;
        wasSignUp = false;

        var gameManager = GameHelper.Instance.getGameManager();

        gameManager.updateUserId();

        if (!isPreLogin) {
            
            //delete the pending purchases if any, to not attribute one purchase to a another account
            gameManager.purchasesManager.clearPendingPurchases();
            SaveManager.Instance.saveShopItems();

            //reactivate ads as the user couldn't buy ads
            gameManager.reactivateAds();
        }
    }

    public void cancelFacebookLogin() {

        isLoggingInToFacebook = false;
    }

    public void logInToFacebook(string originActivityName, Action onDone, Action onError) {

        if (isLoggingInToFacebook) {
            //already logging in
            return;
        }

        if (isLoggedInFacebook()) {
            //already logged, sync directly
            syncDataOnce(onDone, onError, originActivityName, T.Value.LOGIN_FACEBOOK);
            return;
        }

        //logout from FB or/and firebase before login
        logoutFromFacebook(true);

        isLoggingInToFacebook = true;

        //log in with facebook
        FB.LogInWithReadPermissions(
            new List<string> { "public_profile" },
            (result) => {

                //call on main thread to avoid bugs
                DispatchQueue.Instance.Invoke(() => {
                    onFacebookLoginFinished(originActivityName, result, onDone, onError);
                });
            }
        );

    }

    private void onFacebookLoginFinished(string originActivityName, ILoginResult result, Action onDone, Action onError) {

        if (!isLoggingInToFacebook) {
            //has been cancelled
            return;
        }

        isLoggingInToFacebook = false;

        if (result == null || result.Cancelled) {
            //cancelled
            onDone?.Invoke();
            return;
        }

        string tokenToSend = null;

        var accessToken = result.AccessToken;
        if (accessToken != null) {
            tokenToSend = accessToken.TokenString;
        }

        if (tokenToSend == null) {

            if (result.Error != null) {
                Debug.LogWarning("Err login facebook : " + result.Error);
            }

            //failed
            onError?.Invoke();
            return;
        }

        isLoggingInToFacebook = true;

        var credential = FacebookAuthProvider.GetCredential(tokenToSend);
        firebaseAuth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task => {
            
            onFirebaseLoginFinished(originActivityName, task, onDone, onError);
        });
    }

    private void onFirebaseLoginFinished(string originActivityName, Task<SignInResult> task, Action onDone, Action onError) {

        if (!isLoggingInToFacebook) {
            //has been cancelled
            logoutFromFacebook(true);
            return;
        }

        isLoggingInToFacebook = false;

        if (!isLoggedInFacebook()) {
            
            if (task != null && task.IsFaulted) {
                Debug.LogWarning("Err login firebase : " + task.Exception);
            }

            //error
            onError?.Invoke();
            return;
        }

        GameHelper.Instance.getGameManager().updateUserId();
    
        //finished and logged successfully
        syncDataOnce(onDone, onError, originActivityName, T.Value.LOGIN_FACEBOOK);
	}

    private void syncDataOnce(Action onDone, Action onError, string originActivityName, string loginType) {

        if (!isLoggedInFacebook()) {
            //must be logged to sync data
            onError?.Invoke();
            return;
        }

        if (hasSyncDataOnce) {
            //already done
            onDone?.Invoke();
            return;
        }

        //sent current save to the server then retrieve the new save
        FirebaseFunctionsManager.instance.synchronize(hasBeenCreated => {

            hasSyncDataOnce = true;

            if (hasBeenCreated) {
                trackSignUp(originActivityName, loginType);
            } else {
                trackSignIn(originActivityName, loginType);
            }
            
            //save last sync date to know when to sync again in the future
            Prop.lastSync.putNow();

            //done finish
            onDone?.Invoke();

        }, (exception) => {

            Debug.LogWarning("Err sync firebase : " + exception);

            //done with error
            onError?.Invoke();
        });
    }

    private void trackSignIn(string originActivityName, string loginType) {

        TrackingManager.instance.prepareEvent(T.Event.SIGN_IN)
                       .add(T.Param.ORIGIN, originActivityName)
                       .add(T.Param.TYPE, loginType)
                       .track();
    }

    private void trackSignUp(string originActivityName, string loginType) {

        TrackingManager.instance.prepareEvent(T.Event.SIGN_UP)
                       .add(T.Param.ORIGIN, originActivityName)
                       .add(T.Param.TYPE, loginType)
                       .track();
    }

}

