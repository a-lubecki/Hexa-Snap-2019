/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public class OnboardingHandler {


    private Activity10 activity;
    private UniqueDisplaySpeechesManager uniqueDisplaySpeechesManager;
    private CharacterAnimator characterAnimator;

    public bool isHandlingOnboarding { get; private set; }

    private bool hasShownSpeechSameColors = false;
    private bool hasShownSpeechFailedTooSoon1 = false;
    private bool hasShownSpeech3Items = false;
    private bool hasShownSpeechFirstItemsClick = false;

    private int nbAttachedItems = 0;
    private int nbGeneratedBonus = 0;


    public OnboardingHandler(Activity10 activity) {

        this.activity = activity ?? throw new ArgumentException();
        uniqueDisplaySpeechesManager = GameHelper.Instance.getUniqueDisplaySpeechesManager();
        characterAnimator = GameHelper.Instance.getCharacterAnimator();
    }

    private bool hasPassedOnboarding() {
        return GameHelper.Instance.getGameManager().hasPassedOnboarding;
    }

    private bool canShowSpeechLimitZone() {
        return !hasPassedOnboarding() && activity.axisBehavior.getAngleDegrees() != 0;
    }

    private bool canShowSpeechSameColors() {
        return !hasPassedOnboarding() && !hasShownSpeechSameColors && 
            !hasShownSpeech3Items && nbAttachedItems > 20;
    }

    private bool canShowSpeech3Items(Item lastAttachedItem) {
        return !hasPassedOnboarding() && !hasShownSpeech3Items &&
            activity.axis.getAttachedItemsGroup(lastAttachedItem, true).Count >= Constants.NB_ITEMS_TO_SELECT_GROUP;
    }

    private bool mustShowSpeechForce3ItemsClick(Item lastAttachedItem) {
        return !hasPassedOnboarding() && lastAttachedItem.isSnapped();
    }

    private bool canShowSpeechFirstItemBonus(Item lastAttachedItem) {
        return lastAttachedItem.itemType == ItemType.Bonus && 
            !GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("10.OnboardingFirstItemBonus");
    }

    private bool canShowSpeechFirstItemsClick() {
        return !hasPassedOnboarding() && !hasShownSpeechFirstItemsClick;
    }

    private bool canShowSpeechIncitateStack(ItemBonus itemBonus) {
        //the first bonus click speech (10.OnboardingFirstItemBonus) must be shown before the stack incitating
        return activity.getCurrentLevel() >= 4 && nbGeneratedBonus >= 3 && !itemBonus.bonusType.isMalus &&
            !GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("10.OnboardingIncitateStack") &&
            !GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("10.OnboardingStack") &&
            GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("10.OnboardingFirstItemBonus");
    }

    private bool mustShowSpeechForceStack(ItemBonus item) {
        return item.isEnqueued;
    }

    private bool canShowSpeechFirstStack() {
        //the first stack must occur just after the incitating as the game is blocked
        return GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("10.OnboardingIncitateStack") &&
                         !GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("10.OnboardingStack");
    }


    public void startOnboarding() {

        isHandlingOnboarding = true;

        Async.call(1, () => {

            characterAnimator
                .show(activity, true)
                .enqueueTr("10.OnboardingFirstGame")
                .enqueueJoin()
                .enqueueEvent(true, () => showOnboardingAnimation("OnboardingDragIndicator"))
                .enqueueDelay(CharacterTimeline.MOVE, 2)
                .enqueueDelay(CharacterTimeline.EXPRESSION, 2)
                .enqueue(new CharacterAnimationMove(CharacterRes.MOVE_ROTATE))
                .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_EYES_CLOSED, 1.5f))
                .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_RETARDED, 1))
                .enqueueJoin()
                .enqueueDelay(CharacterTimeline.SPEECH, 2)
                .enqueueEvent(true, showOnboardingSpeechForceAxisRotate);
        });
    }

    private void showOnboardingSpeechForceAxisRotate() {

        if (canShowSpeechLimitZone()) {

            //go to the next speech
            showOnboardingSpeechLimitZone();
            return;
        }

        //if player didn't moved the axis, incitate to move
        characterAnimator
            .show(activity, true)
            .enqueue(Tr.arr("10.OnboardingForceAxisRotate", 0, 1))
            .enqueueEvent(true, () => {

                if (canShowSpeechLimitZone()) {
                
                    //go to the next speech
                    Async.call(2, showOnboardingSpeechLimitZone);
                    return;
                }
                
                //player didn't understand, continue incitating
                characterAnimator
                    .show(activity, true)
                    .enqueue(Tr.arr("10.OnboardingForceAxisRotate", 1, 1))
                    .enqueueEvent(true, () => showOnboardingAnimation("OnboardingDragIndicator"), 6)
                    .enqueueDelay(CharacterTimeline.SPEECH, 1)
                    .enqueueEvent(true, showOnboardingSpeechForceAxisRotate);//retry
            });
    }

    private void showOnboardingSpeechLimitZone() {

        characterAnimator
            .show(activity, true)
            .enqueueEvent(true, () => {

                //spawn the items but don't generate them over time
                activity.itemsGenerator.startGeneration();
                activity.itemsGenerator.pauseGeneration();

            }, 2)
            .enqueue(Tr.arr("10.OnboardingLimitZone", 0, 1))
            .enqueueEvent(true, () => showOnboardingAnimation("OnboardingLimitZone"), 2)
            .enqueueJoin()
            .enqueue(Tr.arr("10.OnboardingLimitZone", 1, 1))
            .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_EMOJI_FEAR, 3))
            .enqueue(new CharacterAnimationMove(CharacterRes.MOVE_BOUNCE))
            .enqueue(new CharacterAnimationMove(CharacterRes.MOVE_SHIVER))
            .enqueueEvent(true, endOnboardingSpeeches);
    }

    public void onFailedTooSoon() {

        var speechText = !hasShownSpeechFailedTooSoon1 ? "10.OnboardingFailedTooSoon1" : "10.OnboardingFailedTooSoon2";

        if (!hasShownSpeechFailedTooSoon1) {
            hasShownSpeechFailedTooSoon1 = true;
        }

        GameHelper.Instance.getGameManager().stopPlaying();

        startOnboardingSpeeches();

        Async.call(activity.animateAxisRelease(null));

        characterAnimator
            .show(activity, true)
            .enqueueTr(speechText)
            .enqueueEvent(true, () => activity.axis.setRotationAngle(0))
            .enqueueEvent(true, activity.generateFirstSnappedItems, 1)
            .enqueueEvent(true, GameHelper.Instance.getGameManager().startPlaying)
            .enqueueEvent(true, endOnboardingSpeeches);
    }

    public void onItemGenerated(Item item) {

        if (isHandlingOnboarding) {
            //already showing another speech
            return;
        }

        if (item is ItemBonus) {

            nbGeneratedBonus++;

            var itemBonus = item as ItemBonus;

            if (canShowSpeechIncitateStack(itemBonus)) {

                startOnboardingSpeeches();

                //display a speech for the first bonus stack after the player clicked on the firs bonus
                characterAnimator
                    .show(activity, true)
                    .enqueueUniqueDisplay("10.OnboardingIncitateStack")//unique display before so that it can be interrupted by the 10.OnboardingFirstStack speech
                    .enqueueTr("10.OnboardingIncitateStack")
                    .enqueueEvent(false, () => showOnboardingSpeechForceStack(itemBonus));
            }
        }
    }

    private void showOnboardingSpeechForceStack(ItemBonus item) {

        if (!mustShowSpeechForceStack(item)) {
            //player understood "10.OnboardingFirstStack" has been called
            return;
        }

        //show an anim over the button
        var itemPos = BaseModelBehavior.findTransform(item).position;
        Async.call(0.5f, () => showOnboardingAnimation("OnboardingButtonStack", activity.buttonStackFirstBonus.transform.position));

        Async.call(5, () => {

            if (!mustShowSpeechForceStack(item)) {
                //player understood "10.OnboardingFirstStack" has been called
                return;
            }

            //the player has still not clicked the stack button, show a speech to incitate
            characterAnimator
                .show(activity, true)
                .enqueueTr("10.OnboardingForceStack")
                .enqueueDelay(CharacterTimeline.SPEECH, 0.5f)
                .enqueueEvent(false, () => showOnboardingSpeechForceStack(item));
        });
    }

    public void onItemBonusStacked(ItemBonus item) {
        
        if (canShowSpeechFirstStack()) {

            startOnboardingSpeeches();

            //the player has clicked on the button to stack the first time
            characterAnimator
                    .show(activity, true)
                    .enqueueTr("10.OnboardingFirstStack")
                    .enqueueUniqueDisplay("10.OnboardingStack")
                    .enqueueEvent(true, () => {

                        if (!item.isStacked) {
                            //item has been clicked
                            endOnboardingSpeeches();
                            return;
                        }

                        characterAnimator
                            .show(activity, true)
                            .enqueueTr("10.OnboardingFirstStackClick")
                            .enqueueEvent(true, () => {

                                var delayEnd = 0.5f;

                                if (item.isStacked) {
                                    showOnboardingAnimation("OnboardingItemsClick", BaseModelBehavior.findTransform(item).position);
                                    delayEnd = 3;
                                }

                                Async.call(delayEnd, endOnboardingSpeeches);
                            });
                    });

        } else {

            //the player has understood, disable speeches about stacking
            var hasChanged = false;

            var udm = GameHelper.Instance.getUniqueDisplaySpeechesManager();
            if (!udm.hasTag("10.OnboardingIncitateStack")) {
                udm.addTag("10.OnboardingIncitateStack");
                hasChanged = true;
            }

            if (!udm.hasTag("10.OnboardingStack")) {
                udm.addTag("10.OnboardingStack");
                hasChanged = true;
            }

            if (hasChanged) {
                SaveManager.Instance.saveCharacter();
            }
        }

    }

    public void onItemAttach(Item lastAttachedItem) {

        if (isHandlingOnboarding) {
            //already showing another speech
            return;
        }

        nbAttachedItems++;

        if (canShowSpeech3Items(lastAttachedItem)) {

            hasShownSpeech3Items = true;

            startOnboardingSpeeches();

            //display a speech for the very first 3 items groupping
            characterAnimator
                .show(activity, true)
                .enqueueTr("10.Onboarding3Items")
                .enqueueEvent(false, () => showOnboardingSpeechForceClic3Items(lastAttachedItem));
        
        } else if (canShowSpeechSameColors()) {

            hasShownSpeechSameColors = true;

            startOnboardingSpeeches();

            //display a speech when the player don't want to group colors after a certain amount of generated items
            characterAnimator
                .show(activity, true)
                .enqueueTr("10.OnboardingSameColors")
                .enqueueEvent(true, endOnboardingSpeeches);
            
        } else if (canShowSpeechFirstItemBonus(lastAttachedItem)) {

            startOnboardingSpeeches();

            characterAnimator
                .show(activity, true)
                .enqueue(Tr.arr("10.OnboardingFirstItemBonus", 0, 1))
                .enqueueUniqueDisplay("10.OnboardingFirstItemBonus")
                .enqueueEvent(true, () => {

                    //show additional touch when item is not destroyed
                    if ((lastAttachedItem as ItemBonus).isSnapped()) {
                        characterAnimator.enqueue(Tr.arr("10.OnboardingFirstItemBonus", 1, 1));
                    }

                    characterAnimator
                        .enqueueEvent(true, () => endShowOnboardingSpeechFirstItemBonus(lastAttachedItem));
                });
        }
    }

    private void showOnboardingSpeechForceClic3Items(Item lastAttachedItem) {

        if (!mustShowSpeechForce3ItemsClick(lastAttachedItem)) {
            //player understood "10.OnboardingFirstItemsClick" has been called
            return;
        }

        //block the rotation to avoid seeing the next anim shift
        activity.axis.isRotationLocked = true;

        //show an anim over the item
        Async.call(0.5f, () => {
            var tr = BaseModelBehavior.findTransform(lastAttachedItem);
            if (tr != null) {
                showOnboardingAnimation("OnboardingItemsClick", tr.position);
            }
        });

        Async.call(2, () => activity.axis.isRotationLocked = false);

        Async.call(5, () => {

            if (!mustShowSpeechForce3ItemsClick(lastAttachedItem)) {
                //player understood "10.OnboardingFirstItemsClick" has been called
                return;
            }

            //the player has still not clicked the items, show a speech to incitate
            characterAnimator
                .show(activity, true)
                .enqueueTr("10.OnboardingForceItemsClick")
                .enqueueDelay(CharacterTimeline.SPEECH, 0.5f)
                .enqueueEvent(false, () => showOnboardingSpeechForceClic3Items(lastAttachedItem));
        });
    }

    private void endShowOnboardingSpeechFirstItemBonus(Item lastAttachedItem) {

        var trItem = BaseModelBehavior.findTransform(lastAttachedItem);
        if (trItem == null) {
            //the player has clicked on the item and it is not available any more
            endOnboardingSpeeches();
            return;
        }

        //show a circle on the item
        activity.axis.isRotationLocked = true;

        Async.call(0.5f, () => showOnboardingAnimation("OnboardingItemsClick", trItem.position));

        Async.call(2.5f, () => {

            activity.axis.isRotationLocked = false;

            endOnboardingSpeeches();
        });
    }

    public void onItemSelect(Item item) {

        if (item.itemType == ItemType.Bonus) {
            var udm = GameHelper.Instance.getUniqueDisplaySpeechesManager();
            if (!udm.hasTag("10.OnboardingFirstItemBonus")) {
                udm.addTag("10.OnboardingFirstItemBonus");
                SaveManager.Instance.saveCharacter();
            }
        }

        if (canShowSpeechFirstItemsClick()) {

            //display a speech for the very first items select
            hasShownSpeechFirstItemsClick = true;

            startOnboardingSpeeches();

            //free rotation in case the last cheracter queued event has been skipped
            activity.axis.isRotationLocked = false;

            characterAnimator
                .show(activity, true)
                .enqueueTr("10.OnboardingFirstItemsClick")
                .enqueueEvent(true, endOnboardingSpeeches);//finish the started onboarding with onItemAttach
            
        } else if (!isHandlingOnboarding && item.itemType == ItemType.Type100 &&
                   !uniqueDisplaySpeechesManager.hasTag("10.OnboardingFirstHexacoins")) {

            startOnboardingSpeeches();

            //display a speech for the first black items select (hexacoins earning), can even occur after the onboarding
            characterAnimator
                .show(activity, true)
                .enqueueTr("10.OnboardingFirstHexacoins")
                .enqueue(new CharacterAnimationMove(CharacterRes.MOVE_JUMP))
                .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_CUTE, 2))
                .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_SMILE, 1))
                .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_BLINK, 0.5f))
                .enqueueUniqueDisplay("10.OnboardingFirstHexacoins")
                .enqueueEvent(true, endOnboardingSpeeches);
        }
    }

    private void showOnboardingAnimation(string animatedObjectName, Vector3? position = null) {
        
        var prefab = GameHelper.Instance.loadPrefabAsset(animatedObjectName);
        var goIndicator = GameObject.Instantiate(prefab, activity.getParentTransform());

        if (position.HasValue) {
            goIndicator.transform.position = position.Value;
        }

        Async.call(prefab.GetComponent<Animation>().clip.length, () => {

            if (goIndicator.activeSelf) {
                GameObject.Destroy(goIndicator);
            }
        });
    }

    private void startOnboardingSpeeches() {

        isHandlingOnboarding = true;

        activity.buttonPause.menuButton.setEnabled(false);
        activity.itemsGenerator.pauseGeneration();

        //disable growing timers to avoid player frustration
        activity.pauseAllMalusTimers();
    }

    private void endOnboardingSpeeches() {

        isHandlingOnboarding = false;

        activity.buttonPause.menuButton.setEnabled(true);
        activity.itemsGenerator.resumeGeneration();

        //enable previously disabled growing timer
        activity.resumeAllMalusTimers();

        //enqueue hide instead of hiding directly to play next speeches if any
        characterAnimator.enqueueHide();
    }

}
