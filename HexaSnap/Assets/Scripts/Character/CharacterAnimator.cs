/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterAnimator : MonoBehaviour, IAnimatorQueueListener {


    private static Texture2D imageFaceDefault;
    private static AnimationClip moveAnimationClip;

    private static String[] DEFAULT_SHORT_EXPRESSIONS = {
        CharacterRes.EXPR_DEFAULT,
        CharacterRes.EXPR_SMILE,
        CharacterRes.EXPR_SURPRISED,
        CharacterRes.EXPR_DETERMINED,
        CharacterRes.EXPR_EYES_CLOSED
    };

    private static String[] DEFAULT_LONG_EXPRESSIONS = {
        CharacterRes.EXPR_DEFAULT,
        CharacterRes.EXPR_DEFAULT_MEH,
        CharacterRes.EXPR_DEFAULT_HAPPY,
        CharacterRes.EXPR_SMILE,
        CharacterRes.EXPR_DETERMINED
    };


    private readonly int test = new System.Random().Next();

    public bool isInsideScreen { get; private set; }
    public bool isShowing { get; private set; }
    public bool isHiding { get; private set; }

    public BaseActivity linkedActivity { get; private set; }
    public Vector3 posOutsideScreen { get; private set; }
    public Vector3 posInsideScreen { get; private set; }
    public CharacterBubblePosition bubblePos { get; private set; }
    public CharacterTimelineEndPolicy timelineEndPolicy { get; private set; }

    protected PositionInterpolator positionInterpolator;

    public Transform trHexagon { get; private set; }
    public Transform trFace { get; private set; }
    public Transform trBubble { get; private set; }

    /**
     * A queue that manage the 3 timelines of the character
     */
    private readonly Dictionary<CharacterTimeline, CharacterAnimatorQueue> queues = new Dictionary<CharacterTimeline, CharacterAnimatorQueue> {
        { CharacterTimeline.MOVE, new CharacterAnimatorQueue() },
        { CharacterTimeline.EXPRESSION, new CharacterAnimatorQueue() },
        { CharacterTimeline.SPEECH, new CharacterAnimatorQueue() }
    };

    private QueueJoinManager joinManager = new QueueJoinManager();

    private bool isPlayingDefaultExpression;

    /**
     * Indicates that the current speech must not be interrupted, it may contains an event to play
     */
    private bool isCurrentSpeechImportant;


    void Awake() {

        positionInterpolator = new PositionInterpolator(transform);

        //assign the join manager to every queue so that they can wait for each other
        foreach (CharacterAnimatorQueue q in queues.Values) {
            q.setListener(this);
            q.setJoinListener(joinManager);
        }

        trHexagon = transform.Find("Hexagon");
        trFace = trHexagon.Find("Face");
        trBubble = transform.Find("Bubble");

        //set default expression
        updateExpressionImage(QueueElementExpr.loadTextureFromExpr(CharacterRes.EXPR_DEFAULT));

        deactivateBubble();
    }

    void OnDisable() {

        positionInterpolator?.cancelInterpolation();
    }

    void Update() {
        
        positionInterpolator?.update();
    }

    public bool isOnSameMarker(BaseActivity otherActivity) {

        if (otherActivity == null) {
            //we consider that when the other marker is not provided, the character can be shown
            return true;
        }

        if (linkedActivity == null) {
            return false;
        }

        return (otherActivity.markerRef == linkedActivity.markerRef);
    }

    public bool isSpeaking() {
        return isInsideScreen && !isHiding && queues[CharacterTimeline.SPEECH].getNbElements() > 0;
    }

    /**
     * Stop anims + clean timelines + change the current position + hide
     */
    public CharacterAnimator reset(BaseActivity linkedActivity, Vector3 posOutsideScreen,
                                   Vector3 posInsideScreen, CharacterBubblePosition bubblePos,
                                   CharacterTimelineEndPolicy timelineEndPolicy) {

        isInsideScreen = false;
        isShowing = false;
        isHiding = false;

        isCurrentSpeechImportant = false;

        this.linkedActivity = linkedActivity;
        this.posOutsideScreen = posOutsideScreen;
        this.posInsideScreen = posInsideScreen;
        this.bubblePos = bubblePos;
        this.timelineEndPolicy = timelineEndPolicy;

        stop();

        positionInterpolator.cancelInterpolation();

        //change pos outside the screen without animation
        transform.position = posOutsideScreen;

        trHexagon.gameObject.SetActive(false);

        return this;
    }

    /*
     * Clean all + stop the current anims
     */
    public CharacterAnimator stop() {

        if (isCurrentSpeechImportant && isInsideScreen) {
            //don't interupt the current speech as it may contains some important events to play
            return this;
        }

        queues[CharacterTimeline.MOVE].clear();
        queues[CharacterTimeline.EXPRESSION].clear();
        queues[CharacterTimeline.SPEECH].clear();

        startDefaultAnimations();

        return this;
    }

    private void tryPlay() {

        if (!isInsideScreen || isShowing || isHiding) {
            //not shown or animating : no queue
            return;
        }

        queues[CharacterTimeline.MOVE].dequeue();
        queues[CharacterTimeline.EXPRESSION].dequeue();
        queues[CharacterTimeline.SPEECH].dequeue();
    }

    /*
     * Animate moving the character inside the scene if outside then play the enqueued things if any
     */
    public CharacterAnimator show(BaseActivity activity, bool animated) {

        if (!isOnSameMarker(activity)) {
            //wrong screen
            return this;
        }

        //don't interrupt the current speech as it may contains some important events to play
        if (isCurrentSpeechImportant && isInsideScreen && !isHiding) {
            //add a join to sync all queues after the current speech for the next speech
            enqueueJoin();
            return this;
        }

        stop();

        bool wasInsideScreen = isInsideScreen;
        bool wasHiding = isHiding;
        isInsideScreen = true;
        isShowing = true;
        isHiding = false;

        isCurrentSpeechImportant = false;

        trHexagon.gameObject.SetActive(true);

        //reset character as it could have stayed in the previous state on the previous screen
        updateExpressionImage(QueueElementExpr.loadTextureFromExpr(CharacterRes.EXPR_DEFAULT));
        deactivateBubble();

        positionInterpolator.cancelInterpolation();

        if (wasInsideScreen && !wasHiding) {
            endShow(activity);
            return this;
        }

        if (!animated) {
            endShow(activity);
            return this;
        }

        //move inside the screen
        positionInterpolator.addNextPosition(
            new PositionInterpolatorBundle(
                posInsideScreen,
                0.5f,
                InterpolatorCurve.EASE_IN_OUT
            ),
            _ => endShow(activity)
        );

        return this;
    }

    private void endShow(BaseActivity activity) {

        if (!isOnSameMarker(activity)) {
            //the linked activity may have changed
            return;
        }

        if (!isShowing) {
            //hidden during showing anim
            return;
        }

        isShowing = false;

        transform.position = posInsideScreen;
                            
        tryPlay();
    }

    /*
     * Clean all + animate moving the character outside of the screen
     */
    public CharacterAnimator hide(BaseActivity activity, bool animated) {

        if (!isOnSameMarker(activity)) {
            //wrong screen
            return this;
        }

        if (!isInsideScreen || isHiding) {
            //not shown
            return this;
        }

        isShowing = false;
        isHiding = true;

        //the other speeches can now be played
        isCurrentSpeechImportant = false;

        stop();

        positionInterpolator.cancelInterpolation();

        if (!animated) {
            endHide(activity);
            return this;
        }

        //move outside of the screen
        positionInterpolator.addNextPosition(
            new PositionInterpolatorBundle(
                posOutsideScreen,
                0.5f,
                InterpolatorCurve.EASE_IN
            ),
            _ => endHide(activity)
        );

        return this;
    }

    private void endHide(BaseActivity activity) {
        
        if (!isOnSameMarker(activity)) {
            //the linked activity may have changed
            return;
        }

        if (!isHiding) {
            //shown during hiding anim
            return;
        }

        isInsideScreen = false;
        isHiding = false;

        transform.position = posOutsideScreen;

        stopDefaultAnimations();

        trHexagon.gameObject.SetActive(false);
    }

    public void interrupt() {

        if (!isInsideScreen || isShowing || isHiding) {
            //not shown
            return;
        }

        var queueSpeech = queues[CharacterTimeline.SPEECH];
        if (!queueSpeech.hasElements()) {
            //not dequeuing
            return;
        }

        float remainingTime = queueSpeech.getRemainingTimeForCurrentElement();

        if (remainingTime <= 0.5f) {
            //can't skip for instant events or when about to skip
            return;
        }

        var startDequeueTime = queueSpeech.startDequeueTime;
        if (startDequeueTime.HasValue && Time.realtimeSinceStartup - startDequeueTime.Value < 0.5f) {
            //can't skip when just shown the speech
            return;
        }

        GameHelper.Instance.getAudioManager().playSound("Button.Clic");

        queues[CharacterTimeline.SPEECH].skipCurrent();

        //skip the other queues as they're not important
        queues[CharacterTimeline.EXPRESSION].skipUntilNextJoinOrEnd();
        queues[CharacterTimeline.MOVE].skipUntilNextJoinOrEnd();
    }

    public CharacterAnimator enqueueUniqueDisplay(string tag) {

        if (string.IsNullOrEmpty(tag)) {
            throw new ArgumentException();
        }
        if (!isInsideScreen || isHiding) {
            //not shown
            return this;
        }

        queues[CharacterTimeline.SPEECH].enqueueUniqueDisplay(tag);

        tryPlay();

        return this;
    }

    /**
     * Enqueue an animation to play with a default duration 
     * Play it if there are nothing in the queue and the character is shown
     */
    public CharacterAnimator enqueue(BaseCharacterAnimation animation) {

        if (animation == null) {
            throw new ArgumentException();
        }
        if (!isInsideScreen || isHiding) {
            //not shown
            return this;
        }

        CharacterAnimatorQueue queue = queues[animation.timeline];
        animation.addToQueue(queue);

        tryPlay();

        return this;
    }

    /**
     * Enqueue speeches with texts to translate with Tr.arr(...)
     * Play it if there are nothing in the queue and the character is shown
     */
    public CharacterAnimator enqueueTr(string textTrKey) {

        if (textTrKey == null) {
            throw new ArgumentException();
        }

        return enqueueSpeeches(new List<string>(Tr.arr(textTrKey)));
    }

    /**
     * Enqueue speeches with texts to translate with Tr.arr(...) and a random number at the end of the tr key between 1 and max
     * Play it if there are nothing in the queue and the character is shown
     */
    public CharacterAnimator enqueueTrRandom(string textTrKeyPrefix, int max) {

        if (textTrKeyPrefix == null) {
            throw new ArgumentException();
        }
        if (max <= 1) {
            throw new ArgumentException();
        }

        return enqueueTr(textTrKeyPrefix + Constants.newRandomInt(1, max));
    }

    /**
     * Enqueue a speech with the text in parameters to play with a default duration 
     * Play it if there are nothing in the queue and the character is shown
     */
    public CharacterAnimator enqueue(string speechText, params string[] nextSpeechTexts) {

        if (speechText == null) {
            throw new ArgumentException();
        }

        List<string> speeches = new List<string>();
        speeches.Add(speechText);

        if (nextSpeechTexts != null) {
            foreach (string text in nextSpeechTexts) {
                speeches.Add(text);
            }
        }

        return enqueueSpeeches(speeches);
    }

    /**
     * Enqueue a speech with the text in parameters to play with a default duration 
     * Play it if there are nothing in the queue and the character is shown
     */
    public CharacterAnimator enqueue(string[] speechTexts) {

        if (speechTexts == null || speechTexts.Length <= 0) {
            throw new ArgumentException();
        }

        return enqueueSpeeches(new List<string>(speechTexts));
    }

    /**
     * Enqueue a speech with the text in parameters to play with a default duration 
     * Play it if there are nothing in the queue and the character is shown
     */
    private CharacterAnimator enqueueSpeeches(List<string> speeches) {

        if (speeches.Count <= 0) {
            throw new ArgumentException();
        }

        return enqueue(new CharacterAnimationSpeech(speeches, bubblePos));
    }

    /**
     * Add a delay for 1 timeline
     */
    public CharacterAnimator enqueueDelay(CharacterTimeline timeline, float delaySec) {

        if (!isInsideScreen || isHiding) {
            //not shown
            return this;
        }

        queues[timeline].enqueueDelay(delaySec);

        tryPlay();

        return this;
    }

    /**
     * Wait for the current anims to finish before starting the next anims of the timelines in parameter
     * No timeline in parameters mean all timelines
     */
    public CharacterAnimator enqueueJoin() {

        if (!isInsideScreen || isHiding) {
            //not shown
            return this;
        }

        HashSet<CharacterTimeline> timelinesSet = new HashSet<CharacterTimeline>();

        //add all timelines
        timelinesSet.Add(CharacterTimeline.MOVE);
        timelinesSet.Add(CharacterTimeline.EXPRESSION);
        timelinesSet.Add(CharacterTimeline.SPEECH);

        foreach (CharacterTimeline t in timelinesSet) {
            queues[t].enqueueJoin();
        }

        tryPlay();

        return this;
    }

    /**
     * Enqueue an event to play on the speech timeline
     */
    public CharacterAnimator enqueueEvent(bool isImportant, Action action, float durationSec = 0) {

        if (durationSec < 0) {
            throw new ArgumentException();
        }
        if (!isInsideScreen || isHiding) {
            //not shown
            return this;
        }

        //set as important to forbid the speech override by other speeches
        if (isImportant) {
            this.isCurrentSpeechImportant = true;
        }

        queues[CharacterTimeline.SPEECH].enqueueEvent(action);

        if (durationSec > 0) {
            queues[CharacterTimeline.SPEECH].enqueueDelay(durationSec);
        }

        tryPlay();

        return this;
    }

    public void enqueueHide() {

        if (!isInsideScreen || isHiding) {
            //not shown
            return;
        }

        enqueueJoin();

        enqueueEvent(false, () => {
            
            if (queues[CharacterTimeline.SPEECH].getNbElements() <= 0) {
                //don't hide if there are other speeches to show, some can be added after
                return;
            }

            hide(null, true);
        });
    }

    /**
     * Enqueue several items in once
     */
    public CharacterAnimator enqueue(CharacterSituation situation) {

        if (situation == null) {
            throw new ArgumentException();
        }
        if (!isInsideScreen || isHiding) {
            //not shown
            return this;
        }

        situation.fillCharacterAnimator(this);

        return this;
    }

    private void startDefaultAnimations() {
        
        setExpression(null);
        deactivateBubble();
        playMove(null);
    }

    private void stopDefaultAnimations() {
        
        setExpression(null);
        deactivateBubble();

        //stop running animation
        trHexagon.GetComponent<Animation>().Stop();
    }

    public void setExpression(Texture2D imageFace) {

        if (!isInsideScreen) {
            return;
        }

        if (imageFace != null) {

            //want to play a defined expression
            isPlayingDefaultExpression = false;

            updateExpressionImage(imageFace);

        } else {

            setDefaultRandomExpression();
        }
    }

    private void setDefaultRandomExpression() {
        
        //want to play a default expression
        if (isPlayingDefaultExpression) {
            //expression already set
            return;
        }

        //load a random expression for some duration
        var isShortExpr = Constants.newRandomBool();
        var exprArray = isShortExpr ? DEFAULT_SHORT_EXPRESSIONS : DEFAULT_LONG_EXPRESSIONS;

        CharacterAnimationExpr expr = new CharacterAnimationExpr(
            exprArray[Constants.newRandomPosInArray(exprArray.Length)],
            isShortExpr ? Constants.newRandomFloat(0.5f, 1.5f) : Constants.newRandomFloat(2, 4)
        );

        //assign the default loaded image
        var imageFace = QueueElementExpr.loadTextureFromExpr(expr.nameImageFace);

        isPlayingDefaultExpression = true;

        //play next default expression if there are no other expressions in queue
        Async.call(expr.durationSec, () => {

            isPlayingDefaultExpression = false;

            if (!isInsideScreen) {
                //stop if not visible
                return;
            }

            if (queues[CharacterTimeline.EXPRESSION].isDequeuing) {
                //already processing enqueued expression
                return;
            }

            //null expression to load a new random default expression
            setExpression(null);
        });

        updateExpressionImage(imageFace);
    }

    private void updateExpressionImage(Texture2D imageFace) {

        //set active in case of the animations deactivate it
        trFace.gameObject.SetActive(true);

        var image = trFace.GetComponent<RawImage>();
        image.texture = imageFace;
        image.SetNativeSize();
    }

    public void playMove(AnimationClip clip) {

        if (clip == null) {

            //init default clip once (default is in loop mode)
            moveAnimationClip = GameHelper.Instance.loadAnimationClipAsset(Constants.PATH_ANIMS + "Character.DEFAULT");

            if (Constants.isAnimationPlaying(trHexagon.GetComponent<Animation>(), moveAnimationClip.name)) {
                //don't interrupt the default anim to play the same again
                return;
            }

            clip = moveAnimationClip;
        }

        Constants.playAnimation(
            trHexagon.GetComponent<Animation>(),
            clip.name,
            false
        );

        trHexagon.localScale = Vector3.one;
    }

    public void activateBubble() {

        trBubble.gameObject.SetActive(true);
    }

    public void deactivateBubble() {

        trBubble.gameObject.SetActive(false);

        //avoid a glitch for the first display
        trBubble.transform.localScale = Vector3.zero;
    }


    void IAnimatorQueueListener.onNeedDefaultAnim(CharacterAnimatorQueue queue) {

        foreach (KeyValuePair<CharacterTimeline, CharacterAnimatorQueue> p in queues) {

            if (p.Value == queue) {
                //found queue

                CharacterTimeline timeline = p.Key;

                switch (timeline) {

                    case CharacterTimeline.EXPRESSION:
                        setExpression(null);
                        return;

                    case CharacterTimeline.MOVE:
                        playMove(null);
                        return;

                    case CharacterTimeline.SPEECH:
                        deactivateBubble();
                        return;
                }

                return;
            }
        }

    }

}
