/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using UnityEngine;


public class Activity10a : Activity10, LevelCounterListener, GoalCompletionListener {


    public LevelCounterBehavior levelCounterBehavior { get; private set; }
    public LevelCounter levelCounter { 
        get {
            return levelCounterBehavior.levelCounter;
        }
    }

    public GoalCompletionBehavior goalCompletionBehavior { get; private set; }
    public GoalCompletion goalCompletion {
        get {
            return goalCompletionBehavior.goalCompletion;
        }
    }

    private int levelAtStart;
    private float totalLevelDuration;


    protected override string getHUDGameObjectName() {
        return "UIHUD.Arcade";
    }

    protected override GraphPercentagesHolder getGraphPercentagesHolder() {
        return GameHelper.Instance.getUpgradesManager().graphArcade.graphPercentagesHolder;
    }

    public override int getMaxLevel() {
        return Constants.MAX_LEVEL_ARCADE;
    }

    public override int getCurrentLevel() {
        return levelCounter.currentLevel;
    }

    public override int getCurrentCappedLevel() {
        return levelCounter.getCurrentCappedLevel();
    }


    protected override void onCreate() {
		base.onCreate();

		BundlePushE10a b = (BundlePushE10a) bundlePush;

        levelAtStart = b.level;

        var savedScore = b.savedScore;
        if (savedScore.HasValue) {
            var score = savedScore.Value;
            if (score > 0) {
                scoreCounter.clearScore();
                scoreCounter.addScore(savedScore.Value);
            }
        }

        levelCounterBehavior = findChildTransform("TextLevel").GetComponent<LevelCounterBehavior>();
        levelCounterBehavior.init(new LevelCounter(this));
        levelCounter.addListener(this);

        goalCompletionBehavior = findChildTransform("Goal").GetComponent<GoalCompletionBehavior>();
        goalCompletionBehavior.init(new GoalCompletion(levelAtStart));
        goalCompletionBehavior.mustAnimateChanges = true;
        goalCompletion.addListener(this);

        //init level after all init, this will update the level in the items generator
        levelCounter.setCurrentLevel(levelAtStart);
        updateStackEnabling();

        TrackingManager.instance.setUserProperty(
            T.Property.A_NB_PLAY,
            Prop.nbPlayArcade.increment()
        );

        TrackingManager.instance.prepareEvent(T.Event.A_START)
                       .add(T.Param.LEVEL_START, levelAtStart)
                       .track();
    }

    protected override void onResume(bool isFirst) {
        base.onResume(isFirst);

        levelCounterBehavior.gameObject.SetActive(true);
        goalCompletionBehavior.gameObject.SetActive(true);

        if (popCode == Activity4a.POP_CODE_CHANGE_LEVEL) {

            //level change
            levelCounter.setCurrentLevel(((BundlePop4a) bundlePop).nextLevel);
            updateStackEnabling();

            itemsGenerator.startGeneration();

            //start time for a new level
            resetTotalLevelDuration();
        }

        //start or update music for current level
        MusicInfo musicForLevel = null;

        //play a different music (3 musics) every 3 levels
        var root = Mathf.Floor((levelCounter.currentLevel - 1) / 3f) % 3;

        switch (root) {

            case 0:
                musicForLevel = Constants.MUSIC_INFO_GAME_ARCADE_LEVEL1TO5;
                break;

            case 1:
                musicForLevel = Constants.MUSIC_INFO_GAME_ARCADE_LEVEL16TO20;
                break;

            default: //case 2
                musicForLevel = Constants.MUSIC_INFO_GAME_ARCADE_LEVEL21TO30;
                break;
        }

        playMusic(musicForLevel);
    }

    protected override void onPrePause(bool isLast) {
        base.onPrePause(isLast);

        levelCounterBehavior.gameObject.SetActive(false);
        goalCompletionBehavior.gameObject.SetActive(false);
    }

    protected override void onDestroy() {
        base.onDestroy();
        
        //free the goals list
        LevelGoalsManager.Instance.resetGeneratedGoals();
    }

    protected override CharacterSituation newRandomSituation() {

        //show a secret speech once from between level 5 and 10 (shake gesture only available on mobile)
        if (SpecificDeviceManager.Instance.isMobile() &&
            levelCounter.currentLevel >= 5 && 
            levelCounter.currentLevel <= 10 &&
            !GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("10a.Secret")) {

            //show once for 1000
            if (Constants.newRandomInt(0, 1000) == 42) {
                return new CharacterSituation()
                    .enqueueTr("10a.Secret")
                    .enqueueMove(CharacterRes.MOVE_SPIRAL)
                    .enqueueExpression(CharacterRes.EXPR_SUNGLASSES, 30)
                    .enqueueUniqueDisplay("10a.Secret");
            }
        }

        return base.newRandomSituation();
    }

    protected override void onGameStopped(string trackingTagReason) {

        TrackingManager.instance.prepareEvent(T.Event.A_GAMEOVER)
                       .add(T.Param.LEVEL, levelCounter.currentLevel)
                       .add(T.Param.SCORE, scoreCounter.totalScore)
                       .add(T.Param.TIME_SEC, (int)totalDuration)
                       .add(T.Param.LEVEL_START, levelAtStart)
                       .add(T.Param.REASON, trackingTagReason)
                       .track();
    }

    protected override void pushGameOverActivity() {

        //get values before saving in the game manager
        int lastScore = gameManager.maxArcadeScore;
        int lastLevel = gameManager.maxArcadeLevel;

        gameManager.saveFinishedArcadeGame(scoreCounter.totalScore, levelCounter.currentLevel);

        BundlePush12a b = new BundlePush12a {
            lastLevel = lastLevel,
            level = levelCounter.currentLevel,
            lastScore = lastScore,
            score = scoreCounter.totalScore
        };

		push(new Activity12a().setBundlePush(b));
		pop();
    }

    protected override void trackTotalTime() {

        TrackingManager.instance.setUserProperty(
            T.Property.A_TOTAL_TIME,
            Prop.totalTimeSecInArcade.add((int)totalDuration)
        );
    }

	public void triggerNextLevel() {
		triggerLevel(true);
	}

	public void triggerPreviousLevel() {
		triggerLevel(false);
	}

    private void triggerLevel(bool isNext) {

        int currentLevel = levelCounter.currentLevel;

        int newLevel;
        if (isNext) {
            newLevel = levelCounter.getCorrectNewLevel(currentLevel + 1);
        } else {
            newLevel = levelCounter.getCorrectNewLevel(currentLevel - 1);
        }

        Debug.Log("Changed level : " + currentLevel + " => " + newLevel);

        itemsGenerator.stopGeneration();
        bonusStack.unstackAllItems();
        bonusQueue.clear();

        buttonPause.menuButton.setVisible(false);
        buttonStackFirstBonus.menuButton.setVisible(false);

        //remove all free items to avoid snapping
        foreach (Item item in getFreeItems()) {
            item.destroy(ItemDestroyCause.System);
        }

        //reset timer of malus to avoid triggering some directly after the next level start
        foreach (Item item in items) {

            if (item is ItemBonus) {

                ItemBonus itemBonus = item as ItemBonus;
                if (itemBonus.bonusType.isMalus && 
                    (itemBonus.isSnapped() || itemBonus.isStacked)) {
                    itemBonus.cancelSnappedMalusTimer();
                }
            }
        }

        //get values before saving in the game manager
        bool hasJustWin1 = gameManager.maxArcadeLevel <= 1 && newLevel > 1;
        bool hasJustWin20 = (!gameManager.isArcadeHarcoreModeUnlocked() && newLevel > 20);
        bool hasJustWin100 = (!gameManager.isArcadeHarcoreModeBeaten() && newLevel > 100);

        gameManager.saveFinishedArcadeGame(scoreCounter.totalScore, newLevel);

        if (!hasJustWin20 && !hasJustWin100) {
            //don't pause game if the player has just finished the game (let the axis release the items)
            timeManager.pause(this);
        }

        if (isNext) {
            GameHelper.Instance.getAudioManager().playSound("Level.End");
        }

        //remove most distant items to reward the player, only when the game continues (4a)
        if (isNext && !hasJustWin20 && !hasJustWin100) {

            Async.call(0.5f, destroyMostDistantLayerItems);
            Async.call(1.5f, () => endTriggerLevel(currentLevel, newLevel, hasJustWin1, hasJustWin20, hasJustWin100));

            return;
        }

        endTriggerLevel(currentLevel, newLevel, hasJustWin1, hasJustWin20, hasJustWin100);
    }

    private void endTriggerLevel(int currentLevel, int newLevel, bool hasJustWin1, bool hasJustWin20, bool hasJustWin100) {

        //show a speech from the character then trigger the next level
        showCongratulationForLevel(newLevel, () => {

            if (hasJustWin20) {

                stopGame(() => {
                    
                    BundlePush13a b = new BundlePush13a {
                        level = currentLevel,
                        score = scoreCounter.totalScore
                    };

                    push(new Activity13a().setBundlePush(b));
                    pop();
                }, T.Value.GAME_OVER_REASON_END);

            } else if (hasJustWin100) {

                stopGame(() => {

                    BundlePush13b b = new BundlePush13b {
                        level = currentLevel,
                        score = scoreCounter.totalScore
                    };

                    push(new Activity13b().setBundlePush(b));
                    pop();
                }, T.Value.GAME_OVER_REASON_END);

            } else {

                //default level change
                BundlePush4a b = new BundlePush4a {
                    hasCustomTitleLine = true,
                    mustOverlayGame = true,
                    previousLevel = currentLevel,
                    nextLevel = newLevel
                };

                push(new Activity4a().setBundlePush(b));
            }
        });

        if (!gameManager.hasPassedOnboarding && newLevel > 1) {
            //after level 1, the onboarding is complete
            gameManager.setOnboardingAsPassed(true);
        }

        //update duration before getting the totalDuration for tracking
        updateElapsedDuration();

        TrackingManager.instance.prepareEvent(T.Event.A_LEVEL_END)
                       .add(T.Param.LEVEL, currentLevel)
                       .add(T.Param.LEVEL_NEXT, newLevel)
                       .track();
        
        if (hasJustWin1) {

            TrackingManager.instance.prepareEvent(T.Event.A_END_1)
                           .add(T.Param.SCORE, scoreCounter.totalScore)
                           .add(T.Param.TIME_SEC, (int)totalDuration)
                           .track();

        } else if (hasJustWin20) {

            TrackingManager.instance.prepareEvent(T.Event.A_END_20)
                           .add(T.Param.SCORE, scoreCounter.totalScore)
                           .add(T.Param.TIME_SEC, (int)totalDuration)
                           .add(T.Param.LEVEL_START, levelAtStart)
                           .track();

        } else if (hasJustWin100) {

            TrackingManager.instance.prepareEvent(T.Event.A_END_100)
                           .add(T.Param.SCORE, scoreCounter.totalScore)
                           .add(T.Param.TIME_SEC, (int)totalDuration)
                           .add(T.Param.LEVEL_START, levelAtStart)
                           .track();
        }
    }

    protected override void updateElapsedDuration() {

        if (startTime.HasValue) {
            totalLevelDuration += Time.realtimeSinceStartup - startTime.Value;
        }

        //call base method after because the start time is needed before and will be reset
        base.updateElapsedDuration();
    }

    private void resetTotalLevelDuration() {
        totalLevelDuration = 0;
    }


    private void destroyMostDistantLayerItems() {
        
        int level = 3;

        //check if there are some items to destroy
        var itemsToDestroy = axis.getItemsOnLayerLevel(level);
        if (itemsToDestroy.Count <= 0) {
            //play no anim
            return;
        }

        //play 6 FX, one for each hexagon point
        foreach (ItemSnapDirection d in Constants.getAllDirections()) {
            
            Constants.playFX(
                "FX.Axis.LayerWipeout", 
                axisBehavior.calculateGameObjectPositionInGrid(new ItemSnapPosition(d, level, 0), false),
                axisBehavior.getAngleDegrees() - Constants.getAngle(d) - 30
            );
        }

        GameHelper.Instance.getAudioManager().playSound("Level.Wipe");

        //destroy after delay
        Async.call(0.2f, () => {
            
            axis.destroySnappedItems(
                itemsToDestroy,
                true,
                ItemDestroyCause.System
            );

            //remove all free remaining items
            foreach (Item item in getFreeItems()) {
                item.destroy(ItemDestroyCause.System);
            }
        });
    }

    private void showCongratulationForLevel(int nextLevel, Action completion) {
        
        //speeches go from 2 to 20 then 30, 40, 50 ... 100
        if (nextLevel < 2 || nextLevel > 100 || (nextLevel > 20 && nextLevel % 10 != 0)) {
            //no speech to show
            completion();
            return;
        }

        if (nextLevel < gameManager.maxArcadeLevel) {
            //speech has been shown yet (can be by connecting with an advanced account then playing)
            completion();
            return;
        }

        string speechTag = "10a.Level" + nextLevel;

        if (GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag(speechTag)) {
            //speech was already displayed
            completion();
            return;
        }

        //pause the game during the speech
        var tag = "congrat_speech";
        timeManager.pause(tag);

        GameHelper.Instance.getCharacterAnimator()
                  .show(this, true)
                  .enqueue(getCongratulationSituation(nextLevel, speechTag))
                  .enqueueEvent(true, () => {

                      GameHelper.Instance.getCharacterAnimator()
                                .hide(this, true);

                      //take the time to hide the character before showing the next level
                      Async.call(1, () => {
                          
                          timeManager.resume(tag);

                          completion();
                      });
                  });
    }

    private CharacterSituation getCongratulationSituation(int nextLevel, string speechTag) {
        
        CharacterSituation res = new CharacterSituation()
            .enqueueTr(speechTag);

        //add specific expressions / moves
        switch (nextLevel) {

            case 2:
                res.enqueueMove(CharacterRes.MOVE_BOUNCE)
                   .enqueueExpression(CharacterRes.EXPR_EMOJI_FEAR, 3);
                break;

            case 5:
                res.enqueueDelayMove(10)
                   .enqueueDelayExpression(10)
                   .enqueueMove(CharacterRes.MOVE_SHIVER)
                   .enqueueExpression(CharacterRes.EXPR_EMOJI_DEVIL, 4);
                break;

            case 8:
                res.enqueueDelayExpression(4)
                   .enqueueExpression(CharacterRes.EXPR_AMAZED, 4);
                break;

            case 10:
                res.enqueueMove(CharacterRes.MOVE_BOUNCE)
                   .enqueueExpression(CharacterRes.EXPR_EMOJI_FEAR, 3.2f)
                   .enqueueExpression(CharacterRes.EXPR_MEME_POKERFACE, 4);
                break;

            case 11:
                res.enqueueExpression(CharacterRes.EXPR_DEFAULT_MEH, 10);
                break;

            case 12:
                res.enqueueExpression(CharacterRes.EXPR_DEFAULT_MEH, 9)
                   .enqueueExpression(CharacterRes.EXPR_EMOJI_POOP, 5);
                break;

            case 14:
                res.enqueueExpression(CharacterRes.EXPR_MEME_NOTICE_ME_SENPAI, 6)
                   .enqueueDelayMove(6)
                   .enqueueMove(CharacterRes.MOVE_BOUNCE)
                   .enqueueExpression(CharacterRes.EXPR_HUNGRY, 2)
                   .enqueueExpression(CharacterRes.EXPR_SAD, 6);
                break;

            case 15:
                res.enqueueExpression(CharacterRes.EXPR_SAD, 10)
                   .enqueueDelayMove(4)
                   .enqueueMove(CharacterRes.MOVE_STRETCH);
                break;

            case 17:
                res.enqueueMove(CharacterRes.MOVE_BOUNCE)
                   .enqueueExpression(CharacterRes.EXPR_EMOJI_FEAR, 4)
                   .enqueueExpression(CharacterRes.EXPR_SAD, 5);
                break;

            case 18:
                res.enqueueMove(CharacterRes.MOVE_BOUNCE)
                   .enqueueDelayMove(5)
                   .enqueueMove(CharacterRes.MOVE_JUMP)
                   .enqueueExpression(CharacterRes.EXPR_EMOJI_FEAR, 3)
                   .enqueueExpression(CharacterRes.EXPR_KNOCKED_OUT, 3)
                   .enqueueExpression(CharacterRes.EXPR_UNHAPPY, 5);
                break;

            case 19:
                res.enqueueExpression(CharacterRes.EXPR_MEME_POKERFACE, 3)
                   .enqueueExpression(CharacterRes.EXPR_SAD, 4);
                break;
                    
            case 20:
                res.enqueueExpression(CharacterRes.EXPR_UNHAPPY, 4)
                   .enqueueExpression(CharacterRes.EXPR_DEFAULT_MEH, 4)
                   .enqueueExpression(CharacterRes.EXPR_SAD, 2)
                   .enqueueExpression(CharacterRes.EXPR_EYES_CLOSED, 4);
                break;

            case 30:
                res.enqueueExpression(CharacterRes.EXPR_MEME_ME_GUSTA, 4);
                break;

            case 40:
                res.enqueueMove(CharacterRes.MOVE_JUMP)
                   .enqueueExpression(CharacterRes.EXPR_HUNGRY, 3)
                   .enqueueExpression(CharacterRes.EXPR_SUNGLASSES, 5)
                   .enqueueExpression(CharacterRes.EXPR_SMILE, 5);
                break;

            case 80:
                res.enqueueDelayMove(3)
                   .enqueueMove(CharacterRes.MOVE_STRETCH)
                   .enqueueExpression(CharacterRes.EXPR_DEFAULT_MEH, 3)
                   .enqueueExpression(CharacterRes.EXPR_SAD, 4)
                   .enqueueExpression(CharacterRes.EXPR_MEME_POKERFACE, 4);
                break;

            case 90:
                res.enqueueMove(CharacterRes.MOVE_JUMP)
                   .enqueueDelayMove(1)
                   .enqueueMove(CharacterRes.MOVE_JUMP)
                   .enqueueDelayMove(1)
                   .enqueueMove(CharacterRes.MOVE_JUMP)
                   .enqueueDelayMove(1)
                   .enqueueMove(CharacterRes.MOVE_JUMP);
                break;

            case 100:
                res.enqueueMove(CharacterRes.MOVE_SPIRAL)
                   .enqueueDelayMove(10)
                   .enqueueMove(CharacterRes.MOVE_JUMP)
                   .enqueueExpression(CharacterRes.EXPR_AMAZED, 6)
                   .enqueueExpression(CharacterRes.EXPR_MEME_TROLLFACE, 5.5f)
                   .enqueueExpression(CharacterRes.EXPR_DETERMINED, 5);
                break;
        }

        res.enqueueUniqueDisplay(speechTag);
        
        return res;
    }


    protected override void registerSelectedItems(Item memberItem, HashSet<Item> itemsGroup) {

        if (memberItem == null) {
            throw new ArgumentException();
        }

        //fallback if group not provided
        if (itemsGroup == null) {
            itemsGroup = new HashSet<Item>();
            itemsGroup.Add(memberItem);
        }

        //before changing the goal, check if need to show a goal adding anim
        var levelType = goalCompletion.getNullableAvailableType(memberItem);
        if (!levelType.HasValue || goalCompletion.getRemainingNbItemsToFinish(levelType.Value) <= 0) {
            //no need to change anything
            return;
        }

        var pool = GameHelper.Instance.getPool();
        float maxDelay = 0f;

        //play one anim by item updating the goal
        foreach (var item in itemsGroup) {

            var delay = Constants.newRandomFloat(0, 0.2f);

            Vector3 itemPos;

            if (item.isSnapped()) {

                itemPos = axisBehavior.calculateGameObjectPositionInGrid(item.snapPosition, false);

            } else if (item.wasStacked && item is ItemBonus) {

                var trItem = BaseModelBehavior.findTransform(item);
                if (trItem == null) {
                    //unknown item position, don't display ghost item
                    continue;
                }
                itemPos = trItem.position;

            } else {
                //unknown item position, don't display ghost item
                continue;
            }

            //create the ghost item now as the item behavior will be destroyed at the next frame
            var goItemGhost = pool.pickGoalItemGhostGameObject(item, itemPos);

            //play anim with a random delay for a nicer effect
            Async.call(
                delay,
                () => playItemToGoalAnim(item, levelType, goItemGhost.GetComponent<GoalItemGhostBehavior>())
            );

            //calculate max setting total goal later
            if (delay > maxDelay) {
                maxDelay = delay;
            }
        }

        //save the value now as there will be delays
        int nbSelectedItems = itemsGroup.Count;

        //don't take too long for showing goal changes
        if (maxDelay > 0.3f) {
            maxDelay = 0.3f;
        }

        //take the time of the max delay + the anim time to complete the goals : to avoid generating new items before the goal anim ends
        string tag = "item_ghost_anim_" + memberItem.id;
        timeManager.pause(tag);

        Async.call(maxDelay * 0.9f + Constants.ITEM_TO_GOAL_ANIM_TIME_SEC, () => {

            timeManager.resume(tag);

            //finally update the new goal to reach
            goalCompletion.addSelectedItems(memberItem, nbSelectedItems);
        });
	}

    private void playItemToGoalAnim(Item item, LevelItemType? levelType, GoalItemGhostBehavior itemGhostBehavior) {

        if (item == null) {
            throw new ArgumentException();
        }
        if (itemGhostBehavior == null) {
            throw new ArgumentException();
        }

        //calculate the destination point in the goals
        Vector3 nextPos = goalCompletionBehavior.transform.position;
        if (levelType.HasValue) {
            
            var goSubGoal = goalCompletionBehavior.getNullableSubGoal(levelType.Value);
            if (goSubGoal != null) {
                nextPos = goSubGoal.transform.position;
            }
        }

        itemGhostBehavior.animatePosition(
            new PositionInterpolatorBundle(
                nextPos,
                Constants.ITEM_TO_GOAL_ANIM_TIME_SEC,
                InterpolatorCurve.EASE_IN
            ),
            _ => GameHelper.Instance.getPool().storeGoalItemGhostGameObject(itemGhostBehavior.gameObject)
        );
    }

	#region LevelCounterListener
	void LevelCounterListener.onLevelCounterLevelChange(LevelCounter levelCounter, int lastLevel, int level) {

        goalCompletion.resetCompletion(level);

        //enable void bonus items generation to not block the game when a bonus/malus is required to finish the level and the player has deactivated bonus
        bool hasVoidBonus = false;
        bool hasVoidMalus = false;
        
        foreach (LevelItemType type in goalCompletion.goal.getAvailableTypes()) {

            if (type == LevelItemType.Bonus) {
                hasVoidBonus = true;
            } else if (type == LevelItemType.Malus) {
                hasVoidMalus = true;
            }
        }

        itemsGenerator.updateVoidBonusGeneration(hasVoidBonus, hasVoidMalus);

    }
    #endregion

    #region GoalCompletionListener
    void GoalCompletionListener.onGoalChange(GoalCompletion g) {
        //do nothing
    }

    void GoalCompletionListener.onGoalCompletionChange(GoalCompletion goalCompletion) {

		if (goalCompletion.getCompletionPercentage() >= 1) {
			triggerNextLevel();
		}

	}
    #endregion
    
}

public struct BundlePushE10a : BaseBundle {

    public int level;
    public int? savedScore;

}
