/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;

public abstract class InGameInputAction : AbstractInputAction {


	protected Activity10 getInGameActivity() {
		return GameHelper.Instance.getGameManager().getNullableInGameActivity();
	}

	public override bool execute() {

		if (!GameHelper.Instance.getGameManager().isGamePlaying) {
			return false;
		}

		Activity10 activity = getInGameActivity();
		if (activity == null) {
			//not playing
			return false;
		}

		if (activity.timeManager.getTotalTimeScalePlay() <= 0) {
			//can't interact with non UI elements if game is paused
			return false;
		}

		return true;
	}


    protected bool hasStartedClicking() {

        if (Input.touchSupported) {

            if (Input.touchCount <= 0) {
                return false;
            }

            return (Input.GetTouch(0).phase == TouchPhase.Began);
        }

        return isAnyMouseButtonPressed(getDefaultActionMouseButtons(), true);
    }

    protected bool isCurrentlyClicking() {
        
        if (Input.touchSupported) {

            if (Input.touchCount <= 0) {
                return false;
            }

            return (Input.GetTouch(0).phase == TouchPhase.Stationary);
        }

        return isAnyMouseButtonPressed(getDefaultActionMouseButtons(), false);
    }

    protected bool hasFinishedClicking() {

        if (Input.touchSupported) {

            if (Input.touchCount <= 0) {
                return false;
            }

            Touch touch = Input.GetTouch(0);

            return (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled);
        }

        return isAnyMouseButtonReleased(getDefaultActionMouseButtons());
    }

    protected Vector3? getClickPosition() {
        
        if (Input.touchSupported) {

            if (Input.touchCount <= 0) {
                return null;
            }

            return GameHelper.Instance.getMainCameraBehavior().getClickPosInScene(Input.GetTouch(0).position);
        }

        return GameHelper.Instance.getMainCameraBehavior().getClickPosInScene(Input.mousePosition);
    }

}

