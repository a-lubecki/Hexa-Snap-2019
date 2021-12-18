/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public class Activity0 : BaseUIActivity {


    private MenuButtonBehavior buttonSkip;


	protected override MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager) {
		return markerManager.marker0;
	}

    protected override string[] getPrefabNamesToLoad() {
        return null;
    }

    protected override Vector3 getCharacterPosInside() {
        return Constants.newVector3(markerRef.posCenter, 0, -3, 0);
    }

    protected override CharacterBubblePosition getCharacterBubblePosition() {
        return CharacterBubblePosition.TOP;
    }

    protected override CharacterSituation getFirstCharacterSituation(bool isFirstResume) {
        
        return new CharacterSituation()
            .enqueueExpression(CharacterRes.EXPR_SMILE, 3)
            .enqueue(Tr.arr("0.Default", 0, 2))
            .enqueueJoin()
            .enqueue(Tr.arr("0.Default", 2, 2))
            .enqueueExpression(CharacterRes.EXPR_DETERMINED_LEFT, 1)
            .enqueueExpression(CharacterRes.EXPR_DETERMINED_RIGHT, 1)
            .enqueueExpression(CharacterRes.EXPR_DETERMINED, 1)
            .enqueueDelayExpression(3.5f)
            .enqueueExpression(CharacterRes.EXPR_EYES_CLOSED, 0.5f)
            .enqueueExpression(CharacterRes.EXPR_HUNGRY, 1.5f)
            .enqueueDelayMove(7)
            .enqueueMove(CharacterRes.MOVE_SHIVER)
            .enqueueJoin()
            .enqueue(Tr.get("0.Default", 4))
            .enqueueMove(CharacterRes.MOVE_TURN)
            .enqueueExpression(CharacterRes.EXPR_SMILE_RIGHT, 0.8f)
            .enqueueExpression(CharacterRes.EXPR_KNOCKED_OUT, 10)
            .enqueueEvent(false, () => {

                buttonSkip.menuButton.setEnabled(false);

                pushAsRoot(new Activity1b());

            });
        
    }

    protected override bool hasAdBanner() {
        return false;
    }

    protected override void onCreate() {
        base.onCreate();

        TrackingManager.instance.trackEvent(T.Event.ONBOARDING_START);

        Async.call(5, () => {

            buttonSkip = createButtonGameObject(
                this,
                markerRef.transform,
                Constants.newVector3(markerRef.posSafeAreaBottomRight, -1, 1, 0),
                MenuButtonIcon.newButtonDefault(
                    Tr.get("Activity0.Button.Skip"),
                    "MenuButton.Play"
                )
            );
        });

        playMusic(null);
    }

    protected override void onDestroy() {
        base.onDestroy();

        //destroy the button because it's not in a root layout
        GameObject.Destroy(buttonSkip);
    }

    protected override void onButtonClick(MenuButtonBehavior menuButton) {

        //prevent the character from dequeueing the activity push event
        GameHelper.Instance.getCharacterAnimator().stop();

        TrackingManager.instance.trackEvent(T.Event.ONBOARDING_SKIP);

        //skip
        pushAsRoot(new Activity1b());
    }

}

