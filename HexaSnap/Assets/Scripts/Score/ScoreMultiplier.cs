/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public class ScoreMultiplier : InGameModel, GameTimerListener {

    private static ScoreMultiplierListener to(BaseModelListener listener) {
        return (ScoreMultiplierListener) listener;
    }


    public float totalMultiplier {
        get {
            return itemsMultiplier * bonusMultiplier;
        }
    }

    public float itemsMultiplier { get; private set; }
    public float bonusMultiplier { get; private set; }

    private GameTimer timerEnd;


    public ScoreMultiplier(Activity10 activity) : base(activity) {
        itemsMultiplier = 1;
        bonusMultiplier = 1;
    }

    public void clearItemsMultiplier() {

        itemsMultiplier = 1;

        notifyListeners(listener => {
            to(listener).onMultiplierChanged(this);
        });

        cancelTimerEnd();
    }

    public void addItemsMultiplier(int multiplier) {

        if (multiplier <= 0) {
            throw new ArgumentException();
        }

        itemsMultiplier += multiplier;

        notifyListeners(listener => {
            to(listener).onMultiplierChanged(this);
        });

        cancelTimerEnd();

        if (itemsMultiplier > 1) {
            startTimerEnd();
        }
    }

    public void reduceItemsMultiplier() {

        if (itemsMultiplier <= 1) {
            return;
        }

        itemsMultiplier = Mathf.Floor(itemsMultiplier / 2f);

        notifyListeners(listener => {
            to(listener).onMultiplierChanged(this);
        });

        cancelTimerEnd();

        if (itemsMultiplier > 1) {
            startTimerEnd();
        }
    }

    public void updateBonusMultiplier() {

        float currentMultiplier = activity.bonusQueue.getEnqueuedScoreMultiplier();

        if (currentMultiplier == bonusMultiplier) {
            //nothing changed
            return;
        }

        bonusMultiplier = currentMultiplier;

        notifyListeners(listener => {
            to(listener).onMultiplierChanged(this);
        });
    }

    private void startTimerEnd() {
        
        float nbSec = 50f / itemsMultiplier;//x1 : -, x2 : 25s, x3 : 16.66s, x4 : 12.5s, x5 : 10s
        
        timerEnd = new GameTimer(activity, false, nbSec);
        timerEnd.addListener(this);

        timerEnd.start();
    }
    
    public void cancelTimerEnd() {

        if (timerEnd == null) {
            return;
        }

        timerEnd.cancel();
        timerEnd = null;
    }

    public bool isTimerEndRunning() {

        if (timerEnd == null) {
            return false;
        }

        return timerEnd.isRunning;
    }

    public float getTimerEndProgressPercentage() {

        if (timerEnd == null) {
            return 0;
        }

        return timerEnd.getProgressPercentage();
    }

    public float getTimerEndRemainingTimeSec() {

        if (timerEnd == null) {
            return 0;
        }

        return timerEnd.remainingTimeSec;
    }

    BaseModelBehavior BaseModelListener.getModelBehavior() {
        return null;
    }

    void GameTimerListener.onTimerRunningBonusStart(GameTimer timer) {
        notifyTimerListener();
    }

    void GameTimerListener.onTimerRunningBonusProgress(GameTimer timer) {
        notifyTimerListener();
    }

    void GameTimerListener.onTimerRunningBonusFinish(GameTimer timer) {
        
        reduceItemsMultiplier();

        notifyTimerListener();
    }

    void GameTimerListener.onTimerRunningBonusCancel(GameTimer timer) {
        notifyTimerListener();
    }

    void GameTimerListener.onTimerRunningBonusDurationChange(GameTimer timer) {
        //do nothing
    }

    private void notifyTimerListener() {

        notifyListeners(listener => {
            to(listener).onTimerUpdate(this);
        });
    }

}

