/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections;
using UnityEngine;


public class GameTimer : InGameModel {
    
    private static GameTimerListener to(BaseModelListener listener) {
        return (GameTimerListener) listener;
    }

    public bool hasTimeScalePhysics { get; private set; }

    public float totalDurationSec { get; private set; }
    public float durationSec { get; private set; }

    public float remainingTimeSec {
        get {
            float res = totalDurationSec - durationSec;
            if (res < 0) {
                return 0;
            }
            return res;
        }
    }
    
    public bool isRunning { get; private set; }
    public bool isCancelled { get; private set; }


    public GameTimer(Activity10 activity, bool hasTimeScalePhysics, float totalDurationSec) : base(activity) {
        
        if (totalDurationSec <= 0) {
            throw new ArgumentException();
        }

        this.hasTimeScalePhysics = hasTimeScalePhysics;
        this.totalDurationSec = totalDurationSec;
    }

    public float getProgressPercentage() {

        float res = durationSec / totalDurationSec;

        if (res < 0) {
            return 0;
        }
        if (res > totalDurationSec) {
            return totalDurationSec;
        }

        return res;
    }

    public void start() {

        if (isRunning) {
            Debug.LogWarning("Can't start timer : already started");
            return;
        }

        notifyListeners(listener => {
            to(listener).onTimerRunningBonusStart(this);
        });

        Async.call(processTimer());
    }

    public void cancel() {

        if (!isRunning) {
            Debug.LogWarning("Can't cancel timer : not started");
            return;
        }

        isCancelled = true;
        isRunning = false;

        notifyListeners(listener => {
            to(listener).onTimerRunningBonusCancel(this);
        });
    }

    private IEnumerator processTimer() {

        isRunning = true;

        while (isRunning) {

            float timeScale;
            if (hasTimeScalePhysics) {
                timeScale = activity.timeManager.getTotalTimeScalePlay();
            } else {
                timeScale = activity.timeManager.getTotalTimeScalePhysics();
            }

            durationSec += Time.deltaTime * timeScale;

            if (durationSec >= totalDurationSec) {

                durationSec = totalDurationSec;

                //finish processing
                isRunning = false;
                break;
            }

            notifyListeners(listener => {
                to(listener).onTimerRunningBonusProgress(this);
            });

            yield return new WaitForSeconds(Constants.COROUTINE_FIXED_UPDATE_S);

        }

        if (!isCancelled) {

            notifyListeners(listener => {
                to(listener).onTimerRunningBonusFinish(this);
            });
        }

    }

    public void addSeconds(int seconds) {

        if (seconds == 0) {
            //no changes
            return;
        }

        totalDurationSec += seconds;

        if (isRunning && seconds < 0 && totalDurationSec < durationSec) {
            totalDurationSec = durationSec;
        }

        notifyListeners(listener => {
            to(listener).onTimerRunningBonusDurationChange(this);
        });
    }

}

