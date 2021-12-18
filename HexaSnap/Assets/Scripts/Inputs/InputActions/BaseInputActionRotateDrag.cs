/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;

public abstract class BaseInputActionRotateDrag : InGameInputAction {

    public override KeyCode[] getDefaultActionKeys() {
        return null;
    }

    public override int[] getDefaultActionMouseButtons() {
        return new int[] {
            0
        };
    }


    protected Vector2? initialTouchPos { get; private set; }
    protected Vector2? currentTouchPos { get; private set; }

    protected float currentRotationMultiplier { get; private set; }
    protected float lastRotationMultiplier { get; private set; }

    protected float initialAxisAngle { get; private set; }


    public override bool isActionDone() {

        if (!base.execute()) {
            
            initialTouchPos = null;
            currentTouchPos = null;

            return false;
        }

        if (hasFinishedClicking()) {

            initialTouchPos = null;
            currentTouchPos = null;

            onCurrentTouchPosEnd();

            return false;
        }

        currentTouchPos = getClickPosition();
        if (!currentTouchPos.HasValue) {
            return false;
        }

        //assign initial pos
        if (!initialTouchPos.HasValue) {

            initialTouchPos = currentTouchPos;

            resetInitialPos();
        }

        //reset the touch position when the multiplier changes
        currentRotationMultiplier = getInGameActivity().bonusQueue.getEnqueuedRotationMultiplier();
        if (lastRotationMultiplier != currentRotationMultiplier) {

            lastRotationMultiplier = currentRotationMultiplier;

            initialTouchPos = currentTouchPos;

            resetInitialPos();
        }

        if (!initialTouchPos.HasValue) {
            //invalid if no initial point
            return false;
        }

        return true;
    }

    public override bool execute() {

        if (!base.execute()) {
            return false;
        }

        if (!initialTouchPos.HasValue) {
            return false;
        }

        if (!currentTouchPos.HasValue) {
            return false;
        }

        if (currentTouchPos.Value.x == initialTouchPos.Value.x) {
            //same point
            return false;
        }

        return calculateNewAngle();
    }

    protected void resetInitialPos() {

        initialTouchPos = currentTouchPos;
        currentTouchPos = null;

        initialAxisAngle = getInGameActivity().axisBehavior.getAngleDegrees();

        onInitialPosReset();
    }

    protected virtual void onInitialPosReset() {
        //do nothing
    }

    protected virtual void onCurrentTouchPosEnd() {
        //do nothing
    }

    protected abstract bool calculateNewAngle();

}
