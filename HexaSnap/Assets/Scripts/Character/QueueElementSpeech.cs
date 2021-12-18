/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;
using UnityEngine.UI;


public class QueueElementSpeech : BaseCharacterQueueElement  {


    private static readonly int NB_BUBBLE_VARIANT = 3;

    private static readonly string clipNameShow = "Character.SpeechBubble.Show";
    private static readonly string clipNameWait = "Character.SpeechBubble.Wait";
    private static readonly string clipNameHide = "Character.SpeechBubble.Hide";

    private static Texture2D[][] bubbleTextures;


    private CharacterAnimationSpeech speech;
    private string text;

    private float totalDuration;
    private Action<QueueElementEvent, Action> actionToProcess;


    public QueueElementSpeech(CharacterAnimatorQueue queue, CharacterAnimationSpeech speech, string text) : base(queue) {

        //init textures once
        if (bubbleTextures == null) {

            bubbleTextures = new Texture2D[Enum.GetNames(typeof(CharacterBubbleSize)).Length][];

            for (int s = 0; s < bubbleTextures.Length; s++) {

                bubbleTextures[s] = new Texture2D[NB_BUBBLE_VARIANT];
                string sizeEnumName = Enum.GetNames(typeof(CharacterBubbleSize))[s];

                for (int v = 0; v < NB_BUBBLE_VARIANT; v++) {
                    bubbleTextures[s][v] = GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_CHARACTER + "Bubble." + sizeEnumName + "." + v);
                }
            }
        }

        this.speech = speech ?? throw new ArgumentException();
        this.text = text ?? throw new ArgumentException();
    }

    protected override void processEnqueuedElement() {
        //do nothing
    }

    protected override void processSelectedElement(OneShotDelayedAction completion) {

        completion.anticipateCall(true);

        CharacterAnimator characterAnimator = GameHelper.Instance.getCharacterAnimator();

        Animation animation = characterAnimator.trBubble.GetComponent<Animation>();
        Text textSpeech = characterAnimator.trBubble.Find("Speech").GetComponent<Text>();
        RawImage imageBubble = characterAnimator.trBubble.GetComponent<RawImage>();

        //activate now then deactivated at the end of the speech
        characterAnimator.activateBubble();

        //get the animations durations after setting the object active to not get 0
        float durationShow = animation[clipNameShow].length;
        float durationHide = animation[clipNameHide].length;

        updateBubblePos(characterAnimator.trBubble, speech.bubblePos);

        //animate bubble showing
        callCancelableDelayed(0, () => {

            textSpeech.text = text;

            //pick a bubble with a size adapted to the text length
            imageBubble.texture = pickBubbleTexture(calculateBubbleSize(text));
            imageBubble.SetNativeSize();

            //show bubble
            Constants.playAnimation(
                animation,
                clipNameShow,
                false
            );

            GameHelper.Instance.getAudioManager().playSound("Character.Bubble.Show");
        });

        totalDuration += durationShow;

        // animate bubble waiting
        callCancelableDelayed(totalDuration, () => {

            Constants.playAnimation(
                animation,
                clipNameWait,
                false
            );
        });

        totalDuration += calculateShowingDurationSec(text);

        //animate bubble hiding
        callCancelableDelayed(totalDuration, () => {

            //hide bubble
            Constants.playAnimation(
                animation,
                clipNameHide,
                false
            );

            GameHelper.Instance.getAudioManager().playSound("Character.Bubble.Hide");
        });

        totalDuration += durationHide;

        callCancelableDelayed(totalDuration, () => {

            characterAnimator.deactivateBubble();

            completion.callAction();
        });
    }

    public override void onCancel() {
        base.onCancel();

        GameHelper.Instance.getCharacterAnimator().deactivateBubble();
    }

    private static float calculateShowingDurationSec(string text) {

        return text.Length * 0.05f + 1f;//50ms by character
    }

    private static CharacterBubbleSize calculateBubbleSize(string text) {

        int length = text.Length;

        if (length <= 30) {
            return CharacterBubbleSize.SMALL;
        }

        if (length <= 60) {
            return CharacterBubbleSize.MEDIUM;
        }

        return CharacterBubbleSize.LARGE;
    }

    private static Texture2D pickBubbleTexture(CharacterBubbleSize size) {
        return bubbleTextures[(int)size][Constants.newRandomPosInArray(NB_BUBBLE_VARIANT)];
    }

    private static void updateBubblePos(Transform trBubble, CharacterBubblePosition bubblePos) {

        RectTransform rectTransform = trBubble.GetComponent<RectTransform>();
        Transform trSpeech = trBubble.Find("Speech");
        RawImage imageTailLeft = trBubble.Find("BubbleTail.Left").GetComponent<RawImage>();
        RawImage imageTailBottom = trBubble.Find("BubbleTail.Bottom").GetComponent<RawImage>();

        if (bubblePos == CharacterBubblePosition.TOP || bubblePos == CharacterBubblePosition.TOP_RIGHT) {

            float posY = (bubblePos == CharacterBubblePosition.TOP) ? 200 : 160;
            float pivotX = (bubblePos == CharacterBubblePosition.TOP) ? 0.5f : 0.25f;

            rectTransform.localPosition = new Vector3(0, posY, 0);
            rectTransform.pivot = new Vector2(pivotX, 0);

            trBubble.rotation = Quaternion.identity;
            trSpeech.rotation = Quaternion.identity;

            imageTailLeft.enabled = false;
            imageTailBottom.enabled = true;

        } else {

            if (bubblePos == CharacterBubblePosition.LEFT) {

                rectTransform.localPosition = new Vector3(-200, 0, 0);
                rectTransform.pivot = new Vector2(0, 0.5f);

                trBubble.rotation = Quaternion.Euler(0, 180, 0);
                trSpeech.rotation = Quaternion.identity;//reset the text rotation after setting the bubble rotation

            } else if (bubblePos == CharacterBubblePosition.RIGHT) {

                rectTransform.localPosition = new Vector3(200, 0, 0);
                rectTransform.pivot = new Vector2(0, 0.5f);

                trBubble.rotation = Quaternion.identity;
                trSpeech.rotation = Quaternion.identity;

            } else {
                throw new NotSupportedException();
            }

            imageTailLeft.enabled = true;
            imageTailBottom.enabled = false;
        }
    }

    public override string getTag() {
        return "SPEECH";
    }

    public override float getTotalExecutionTimeSec() {
        return totalDuration;
    }

}
