/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;
using Firebase;
using Firebase.Extensions;


public class FirebaseInitManager {


    public static FirebaseInitManager instance = new FirebaseInitManager();


    private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

    private FirebaseInitManager () {
    }

    public void init(Action completion) {

        if (hasResolvedDependencies()) {
            //already done
            completion?.Invoke();
            return;
        }

        tryFixDependencies(3, completion);
    }

    private void tryFixDependencies(int remainingTries, Action completion) {

        if (remainingTries <= 0) {
            //do nothing
            Debug.LogError("Could not resolve Firebase dependencies END");
            return;
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {

            dependencyStatus = task.Result;

            if (!hasResolvedDependencies()) {

                Debug.LogWarning("Could not resolve Firebase dependencies (" + remainingTries + ") : " + task.Result);

                //failed, try again
                tryFixDependencies(remainingTries - 1, completion);
                return;
            }

            //done
            completion?.Invoke();
        });
    }

    public bool hasResolvedDependencies() {
        return dependencyStatus == DependencyStatus.Available;
    }

}
