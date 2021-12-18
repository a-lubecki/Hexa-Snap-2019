/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public class InputActionRotateDragAroundAxis : BaseInputActionRotateDrag {


    private float? angleFromInitialTouchPos;


    protected override void onInitialPosReset() {
        
        if (initialTouchPos.HasValue) {
            angleFromInitialTouchPos = calculateAngleFromPos(getInGameActivity().axisBehavior.transform.position, initialTouchPos.Value);
        } else {
            angleFromInitialTouchPos = null;
        }
    }

    protected override bool calculateNewAngle() {

        if (!angleFromInitialTouchPos.HasValue) {
            resetInitialPos();
            return false;
        }

        AxisBehavior axisBehavior = getInGameActivity().axisBehavior;

        //calculate angle between axis, initial touch point and current touch point
        Vector2 axisPos = axisBehavior.transform.position;
        Vector2 touchPos = currentTouchPos.Value;

        //disable the drag when the touch is close to the center
        float distance = Vector2.Distance(axisPos, touchPos);
        if (distance < 0.6f) {
            
            resetInitialPos();
            return false;
        }

        float diffAngle = calculateAngleFromPos(axisPos, touchPos) - angleFromInitialTouchPos.Value;
        axisBehavior.axis.setRotationAngle(initialAxisAngle - diffAngle * currentRotationMultiplier);

        return true;
    }

	private float calculateAngleFromPos(Vector2 axisPos, Vector2 touchPos) {

		if (axisPos.Equals(touchPos)) {
			return 0;
		}

		return Constants.vectorToAngle(touchPos.x - axisPos.x, touchPos.y - axisPos.y);
	}
    
}

