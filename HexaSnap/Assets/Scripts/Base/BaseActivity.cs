/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public abstract class BaseActivity {


    public static void startFirstActivity(BaseActivity first) {

        if (first == null) {
            throw new ArgumentException();
        }
        if (first.previousActivity != null) {
            throw new InvalidOperationException();
        }

        first.isPushed = true;

        first.markerRef = first.getCurrentMarkerForInit(first.gameManager.markerManager);

        first.createActivityContent();

        first.onCreate();
        first.onPreResume();

        //teleport camera
        GameHelper.Instance.getMainCameraBehavior().transform.position = first.markerRef.posCamera;

        first.isResumed = true;
        first.onResume(true);
    }


    protected GameManager gameManager { get; private set; }

    protected BaseActivity previousActivity { get; private set; }
    protected BaseActivity nextActivity { get; private set; }

    protected bool isPushed { get; private set; }
    protected bool isResumed { get; private set; }
    protected bool isAnimatingPushPop { get; private set; }

    protected BaseBundle bundlePush { get; private set; }
    protected BaseBundle bundlePop { get; private set; }
    protected int popCode { get; private set; }

    public MarkerBehavior markerRef { get; private set; }

    private ActionPushActivity nextPushAction;
    private ActionPopActivity nextPopAction;
    private ActionReplaceActivity nextReplaceAction;


    public BaseActivity() {

        //init a reference of the game manager for easier calls
        gameManager = GameHelper.Instance.getGameManager();
    }

    protected abstract MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager);

    public virtual bool isOverlay() {
        //override if necessary
        return false;
    }

    public BaseActivity setBundlePush(BaseBundle bundlePush) {

        if (isPushed) {
            throw new ArgumentException("Can't change the bundle push when created : this=" + GetType() + " / bundlePush=" + bundlePush.GetType());
        }

        this.bundlePush = bundlePush;

        return this;
    }

    public void push(BaseActivity next) {

        registerNextAction(new ActionPushActivity(next, false));

        if (!isAnimatingPushPop) {
            Async.call(0, processNextActions);
        }
    }

    public void pushAsRoot(BaseActivity next) {

        registerNextAction(new ActionPushActivity(next, true));

        if (!isAnimatingPushPop) {
            Async.call(0, processNextActions);
        }
    }

    public void pop() {
        pop(0, null);
    }

    public void pop(int popCode, BaseBundle bundlePop) {

        registerNextAction(new ActionPopActivity(popCode, bundlePop));

        if (!isAnimatingPushPop) {
            Async.call(0, processNextActions);
        }
    }

    public void replaceBy(BaseActivity other) {

        registerNextAction(new ActionReplaceActivity(other));

        if (!isAnimatingPushPop) {
            Async.call(0, processNextActions);
        }
    }

    private void registerNextAction(BaseActivityAction action) {

        if (action == null) {
            throw new ArgumentException();
        }

        if (action is ActionPushActivity) {
            nextPushAction = action as ActionPushActivity;
        } else if (action is ActionPopActivity) {
            nextPopAction = action as ActionPopActivity;
        } else if (action is ActionReplaceActivity) {
            nextReplaceAction = action as ActionReplaceActivity;
        }

    }

    private void processNextActions() {

        if (isAnimatingPushPop) {
            return;
        }

        //process push first then wait for the end of the animation to pop if needed
        if (nextPushAction != null) {

            ActionPushActivity action = nextPushAction;

            nextPushAction = null;
            action.processAction(this);

        } else if (nextPopAction != null) {

            ActionPopActivity action = nextPopAction;

            nextPopAction = null;
            action.processAction(this);

        } else if (nextReplaceAction != null) {

            ActionReplaceActivity action = nextReplaceAction;

            nextReplaceAction = null;
            action.processAction(this);
        }
    }

    public void processPush(BaseActivity next, bool isRoot) {

        if (next == null) {
            throw new ArgumentException();
        }
        if (next == this) {
            Debug.LogError("Can't push an activity over the same activity : this=" + GetType());
            return;
        }
        if (next.isPushed) {
            Debug.LogError("Already pushed, you must create a new activity : this=" + GetType() + " / next=" + next.GetType());
            return;
        }
        if (!isResumed) {
            Debug.LogError("Can't push a new activity over a one that is not resumed : this=" + GetType() + " / next=" + next.GetType());
            return;
        }

        isAnimatingPushPop = true;
        next.isAnimatingPushPop = true;

        next.isPushed = true;

        nextActivity = next;

        if (!isRoot) {
            next.previousActivity = this;
        }

        next.markerRef = next.getCurrentMarkerForInit(gameManager.markerManager);

        gameManager.setUIEventsEnabled(false);

        next.createActivityContent();

        next.onCreate();

        if (isResumed) {

            onPrePause(false);

            if (!next.isResumed) {
                next.onPreResume();
            }
        }

        //animate camera only when the marker is in a different place
        if (markerRef == next.markerRef) {
            endPush(next, isRoot);
            return;
        }

        animatePush(next, () => {
            endPush(next, isRoot);
        });
    }

    protected abstract void animatePush(BaseActivity next, Action completion);

    private void endPush(BaseActivity next, bool isRoot) {

        if (isResumed) {

            onPause();
            isResumed = false;

            if (!next.isResumed) {
                next.isResumed = true;
                next.onResume(true);
            }
        }

        if (isRoot) {

            clearStack();

            processPop(0, null);
        }

        setActivityContentActive(false);

        gameManager.setUIEventsEnabled(true);

        isAnimatingPushPop = false;
        next.isAnimatingPushPop = false;

        processNextActions();
        next.processNextActions();

        trackActivityChange(next, this);
    }

    protected virtual void setActivityContentActive(bool active) {

    }

    public void processPop(int popCode, BaseBundle bundlePop) {

        if (previousActivity == null) {
            if (!(nextActivity != null && nextActivity.isResumed && isAnimatingPushPop && nextActivity.isAnimatingPushPop)) {
                throw new InvalidOperationException("Nothing to pop");
            }
        }

        isAnimatingPushPop = true;

        gameManager.setUIEventsEnabled(false);

        if (previousActivity != null) {

            previousActivity.isAnimatingPushPop = true;

            //pass the bundle to the previous activity
            previousActivity.popCode = popCode;
            previousActivity.bundlePop = bundlePop;
        }

        //release bundles
        this.bundlePush = null;
        this.bundlePop = null;

        if (previousActivity != null) {
            previousActivity.setActivityContentActive(true);
        }

        if (isResumed) {

            onPrePause(true);

            if (previousActivity != null && !previousActivity.isResumed) {
                previousActivity.onPreResume();
            }
        }

        if (previousActivity != null && markerRef == previousActivity.markerRef) {
            endPop();
            return;
        }

        animatePopActivity(endPop);
    }

    protected abstract void animatePopActivity(Action completion);

    private void endPop() {

        if (isResumed) {

            onPause();

            isResumed = false;

            if (previousActivity != null && !previousActivity.isResumed) {
                previousActivity.isResumed = true;
                previousActivity.onResume(false);
            }
        }

        onDestroy();

        destroyActivityContent();

        gameManager.setUIEventsEnabled(true);

        if (previousActivity != null) {
            previousActivity.nextActivity = nextActivity;
            previousActivity.isAnimatingPushPop = false;
        }
        if (nextActivity != null) {
            nextActivity.previousActivity = previousActivity;
        }

        //retain to notify after releasing
        BaseActivity previousActivityRef = previousActivity;

        //release
        previousActivity = null;
        nextActivity = null;

        isAnimatingPushPop = false;

        if (previousActivityRef != null) {

            previousActivityRef.processNextActions();

            trackActivityChange(previousActivityRef, this);
        }
    }

    public void processReplaceActivity(BaseActivity other) {

        if (other == null) {
            throw new ArgumentException();
        }
        if (other == this) {
            Debug.LogError("Can't replace by the same activity : this=" + GetType());
            return;
        }
        if (other.isPushed) {
            Debug.LogError("Already replaced, you must create a new activity : this=" + GetType() + " / other=" + other.GetType());
            return;
        }

        MarkerBehavior otherMarkerRef = other.getCurrentMarkerForInit(gameManager.markerManager);
        if (markerRef != otherMarkerRef) {
            Debug.LogError("Can't replace an activity over the same activity : this=" + GetType() + " / other=" + other.GetType());
            return;
        }

        isAnimatingPushPop = true;
        other.isAnimatingPushPop = true;

        if (previousActivity != null) {
            previousActivity.isAnimatingPushPop = true;
        }
        if (nextActivity != null) {
            nextActivity.isAnimatingPushPop = true;
        }

        other.isPushed = true;

        other.markerRef = otherMarkerRef;

        other.createActivityContent();

        other.onCreate();

        if (isPushed && isResumed) {

            onPrePause(false);

            if (!other.isResumed) {
                other.onPreResume();
            }

            onPause();
            isResumed = false;

            if (!other.isResumed) {
                other.isResumed = true;
                other.onResume(true);
            }
        }

        onDestroy();

        destroyActivityContent();

        if (previousActivity != null) {
            previousActivity.nextActivity = nextActivity;
            previousActivity.isAnimatingPushPop = false;
        }
        if (nextActivity != null) {
            nextActivity.previousActivity = previousActivity;
            nextActivity.isAnimatingPushPop = false;
        }

        //retain to notify after releasing
        other.nextActivity = nextActivity;
        if (nextActivity != null) {
            nextActivity.previousActivity = other;
        }

        other.previousActivity = previousActivity;
        if (previousActivity != null) {
            previousActivity.nextActivity = other;
        }

        BaseActivity previousActivityRef = previousActivity;
        BaseActivity nextActivityRef = nextActivity;

        nextActivity = null;
        previousActivity = null;

        isAnimatingPushPop = false;
        other.isAnimatingPushPop = false;

        if (previousActivityRef != null) {
            previousActivityRef.processNextActions();
        }
        if (nextActivityRef != null) {
            nextActivityRef.processNextActions();
        }

        //release bundles
        this.bundlePush = null;
        this.bundlePop = null;

        other.processNextActions();

        trackActivityChange(other, this);
    }


    public void clearStack() {

        if (previousActivity == null) {
            return;
        }

        previousActivity.clearStack();

        previousActivity.processPop(0, null);
    }


    protected virtual void onCreate() {

    }

    protected virtual void createActivityContent() {

    }

    protected virtual void onPreResume() {

    }

    protected virtual void onResume(bool isFirst) {

    }

    protected virtual void onPrePause(bool isLast) {

    }

    protected virtual void onPause() {

    }

    protected virtual void destroyActivityContent() {

    }

    protected virtual void onDestroy() {

    }

    protected abstract void trackActivityChange(BaseActivity newSeenActivity, BaseActivity lastSeenActivity);

}
