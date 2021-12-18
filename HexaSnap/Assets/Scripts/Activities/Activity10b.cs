/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */
 
using System;
using UnityEngine;
using UnityEngine.UI;


public class Activity10b : Activity10, GameTimerListener {

    private static readonly int NB_STARS = 5;


    private Text textTimer;
    private int lastTimerTimeSec = -1;
    private DateTime lastSpeechTime;

    private MaskableGraphic[] starsIcon = new MaskableGraphic[NB_STARS];
    private int reachedStarPos = -1;

    //the main time attack timer
    public GameTimer gameTimer { get; private set; }


    protected override string getHUDGameObjectName() {
        return "UIHUD.TimeAttack";
    }
    
    protected override GraphPercentagesHolder getGraphPercentagesHolder() {
        return GameHelper.Instance.getUpgradesManager().graphTimeAttack.graphPercentagesHolder;
    }

    public override int getMaxLevel() {
        return Constants.MAX_LEVEL_TIME_ATTACK;
    }

    private int getLevelFromTime() {
        return 1 + Mathf.FloorToInt(gameTimer.durationSec / (Constants.TARGET_TIME_ATTACK_TIME_S / (float)Constants.MAX_LEVEL_TIME_ATTACK));
    }

    public override int getCurrentLevel() {
        //use capped level to avoid generating items too quickly in items generator
        return getCurrentCappedLevel();
    }

    public override int getCurrentCappedLevel() {

        int res = getLevelFromTime();
        if (res > Constants.MAX_LEVEL_TIME_ATTACK) {
            return Constants.MAX_LEVEL_TIME_ATTACK;
        }

        return res;
    }


    protected override void onCreate() {
		base.onCreate();

        textTimer = findChildTransform("TextTimer").GetComponent<Text>();

        Transform trAdvance = findChildTransform("Advance");

        for (int i = 0 ; i < NB_STARS ; i++) {
            starsIcon[i] = trAdvance.Find("Star" + i).GetComponent<MaskableGraphic>();
        }

        gameTimer = new GameTimer(this, true, Constants.INITIAL_TIME_ATTACK_TIME_S);
        gameTimer.addListener(this);
        
        updateTime();

        Async.call(2.5f, gameTimer.start);

        TrackingManager.instance.setUserProperty(
            T.Property.T_NB_PLAY,
            Prop.nbPlayTimeAttack.increment()
        );

        TrackingManager.instance.trackEvent(T.Event.T_START);
    }

    protected override void onResume(bool isFirst) {
        base.onResume(isFirst);

        playMusic(Constants.MUSIC_INFO_GAME_TIMEATTACK);
    }

    protected override void onDestroy() {
        base.onDestroy();

        gameTimer.cancel();
    }

    protected override void onGameStopped(string trackingTagReason) {

        gameTimer.cancel();

        TrackingManager.instance.prepareEvent(T.Event.T_GAMEOVER)
                       .add(T.Param.TIME_SEC, (int)gameTimer.durationSec)
                       .add(T.Param.SCORE, scoreCounter.totalScore)
                       .add(T.Param.REASON, trackingTagReason)
                       .track();
    }

    protected override void pushGameOverActivity() {

        //get values before saving in the game manager
        float lastTimeSec = gameManager.maxTimeAttackTimeSec;
        float totalTimeSec = gameTimer.durationSec;
        int lastScore = gameManager.maxArcadeScore;

        gameManager.saveFinishedTimeAttackGame(scoreCounter.totalScore, totalTimeSec);
        
        if (getLevelFromTime() < Constants.MAX_LEVEL_TIME_ATTACK) {
            
            //game over
            BundlePush12b b = new BundlePush12b {
                lastTimeSec = lastTimeSec,
                timeSec = totalTimeSec,
                lastScore = lastScore,
                score = scoreCounter.totalScore
            };

            push(new Activity12b().setBundlePush(b));

        } else {

            //win
            BundlePush13c b = new BundlePush13c {
                timeSec = totalTimeSec,
                score = scoreCounter.totalScore
            };

            push(new Activity13c().setBundlePush(b));
        }
        
        pop();
    }

    protected override void trackTotalTime() {

        TrackingManager.instance.setUserProperty(
            T.Property.T_TOTAL_TIME,
            Prop.totalTimeSecInTimeAttack.add((int)totalDuration)
        );
    }

    public void addSeconds(int seconds) {
        
        gameTimer.addSeconds(seconds);
    }

    void GameTimerListener.onTimerRunningBonusStart(GameTimer timer) {

        updateTime();
    }

    void GameTimerListener.onTimerRunningBonusProgress(GameTimer timer) {

        updateTime();

        //show a message if there are less than 20sec in the timer and if the last message was displayed more than 0 sec agao
        float remainingTimeSec = timer.remainingTimeSec;

        if (remainingTimeSec >= 15 && remainingTimeSec <= 20 && lastSpeechTime < DateTime.Now.AddSeconds(-10)) {

            lastSpeechTime = DateTime.Now;

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTrRandom("10b.Chrono", 8)
                      .enqueueHide();
        }

    }

    void GameTimerListener.onTimerRunningBonusFinish(GameTimer timer) {

        updateTime();

        stopGame(pushGameOverActivity, T.Value.GAME_OVER_REASON_END);
    }

    void GameTimerListener.onTimerRunningBonusCancel(GameTimer timer) {

        updateTime();
    }

    void GameTimerListener.onTimerRunningBonusDurationChange(GameTimer timer) {

        updateTime();
    }

    private void updateTime() {

        float timeSecFloat = gameTimer.remainingTimeSec;
        int timeSec = Mathf.FloorToInt(timeSecFloat);

        if (timeSec == lastTimerTimeSec) {
            //optimize the display by not updating if the time is the same
            return;
        }

        lastTimerTimeSec = timeSec;

        textTimer.text = Constants.getDisplayableTimeSec(timeSecFloat);

        updateStars();
    }

    public void updateStars() {

        //update stars
        float percentage = gameTimer.durationSec / Constants.TARGET_TIME_ATTACK_TIME_S;
        if (percentage > 1) {
            percentage = 1;
        }

        int pos = Mathf.FloorToInt(percentage * NB_STARS);
        if (pos == reachedStarPos) {
            //optimized to avoid updating stars too much
            return;
        }

        reachedStarPos = pos;
        
        for (int i = 0 ; i < NB_STARS ; i++) {

            MaskableGraphic star = starsIcon[i];

            Color c = star.color;
            c.a = (i < reachedStarPos) ? 1 : 0.3f;
            star.color = c;
        }
        
    }

}

