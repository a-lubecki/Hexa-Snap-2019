/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using System.Linq;


public class CharacterSituation {

    //store the hide action to compare it
    private static readonly Action<CharacterAnimator> actionHide = 
        animator => {
            animator.enqueueHide();
        };


    private List<Action<CharacterAnimator>> enqueueActions = new List<Action<CharacterAnimator>>();


    public CharacterAnimator fillCharacterAnimator(CharacterAnimator characterAnimator) {

        List<Action<CharacterAnimator>> enqueueActionsCopy = new List<Action<CharacterAnimator>>(enqueueActions);
        Action<CharacterAnimator> lastActionHide = null;

        //get the last elem if hide action
        if (enqueueActionsCopy.Count > 0) {
        
            Action<CharacterAnimator> lastElem = enqueueActionsCopy.Last();
            if (lastElem == actionHide) {
                lastActionHide = lastElem;
                enqueueActionsCopy.RemoveAt(enqueueActionsCopy.Count - 1);
            }
        }

        //call objects enqueue to character animator
        foreach (Action<CharacterAnimator> action in enqueueActionsCopy) {
            action.Invoke(characterAnimator);
        }

        //put the hide action after the unique display to avoid skipping the unique display tag saving
        if (lastActionHide != null) {
            lastActionHide.Invoke(characterAnimator);
        }

        return characterAnimator;
    }

    public CharacterSituation enqueueUniqueDisplay(string tag) {

        enqueueActions.Add(animator => {
            animator.enqueueUniqueDisplay(tag);
        });

        return this;
    }

    /**
     * Enqueue an animation to play with a default duration 
     * Play it if there are nothing in the queue and the character is shown
     */
    public CharacterSituation enqueue(BaseCharacterAnimation animation) {
        
        enqueueActions.Add(animator => {
            animator.enqueue(animation);        
        });

        return this;
    }

    public CharacterSituation enqueueMove(string animName) {
        return enqueue(new CharacterAnimationMove(animName));
    }

    public CharacterSituation enqueueExpression(string animName, float durationSec) {
        return enqueue(new CharacterAnimationExpr(animName, durationSec));
    }

    /**
     * Enqueue speeches with texts to translate with Tr.arr(...)
     * Play it if there are nothing in the queue and the character is shown
     */
    public CharacterSituation enqueueTr(string textTrKey) {

        enqueueActions.Add(animator => {
            animator.enqueueTr(textTrKey);
        });

        return this;
    }

    /**
     * Enqueue speeches with texts to translate with Tr.arr(...) and a random number at the end of the tr key between 1 and max
     * Play it if there are nothing in the queue and the character is shown
     */
    public CharacterSituation enqueueTrRandom(string textTrKeyPrefix, int max) {

        enqueueActions.Add(animator => {
            animator.enqueueTrRandom(textTrKeyPrefix, max);
        });

        return this;
    }

    /**
     * Enqueue a speech with the text in parameters to play with a default duration 
     * Play it if there are nothing in the queue and the character is shown
     */
    public CharacterSituation enqueue(string speechText, params string[] nextSpeechTexts) {

        enqueueActions.Add(animator => {
            animator.enqueue(speechText, nextSpeechTexts);
        });

        return this;
    }

    /**
     * Enqueue a speech with the text in parameters to play with a default duration 
     * Play it if there are nothing in the queue and the character is shown
     */
    public CharacterSituation enqueue(string[] speechTexts) {

        enqueueActions.Add(animator => {
            animator.enqueue(speechTexts);
        });

        return this;
    }

    /**
     * Add a delay for 1 timeline
     */
    public CharacterSituation enqueueDelay(CharacterTimeline timeline, float delaySec) {

        enqueueActions.Add(animator => {
            animator.enqueueDelay(timeline, delaySec);
        });

        return this;
    }

    public CharacterSituation enqueueDelayMove(float delaySec) {
        return enqueueDelay(CharacterTimeline.MOVE, delaySec);
    }

    public CharacterSituation enqueueDelayExpression(float delaySec) {
        return enqueueDelay(CharacterTimeline.EXPRESSION, delaySec);
    }

    public CharacterSituation enqueueDelaySpeech(float delaySec) {
        return enqueueDelay(CharacterTimeline.SPEECH, delaySec);
    }

    /**
     * Wait for the current anims to finish before starting the next anims of the timelines in parameter
     * No timeline in parameters mean all timelines
     */
    public CharacterSituation enqueueJoin() {

        enqueueActions.Add(animator => {
            animator.enqueueJoin();
        });

        return this;
    }

    /**
     * Enqueue an event to play on a timeline
     */
    public CharacterSituation enqueueEvent(bool isImportant, Action action, float durationSec = 0) {

        enqueueActions.Add(animator => {
            animator.enqueueEvent(isImportant, action, durationSec);
        });

        return this;
    }

    public CharacterSituation enqueueHide() {

        enqueueActions.Add(actionHide);

        return this;
    }

}
