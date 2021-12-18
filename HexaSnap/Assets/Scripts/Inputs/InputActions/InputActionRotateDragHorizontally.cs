/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public class InputActionRotateDragHorizontally : BaseInputActionRotateDrag {
	

    protected override void onInitialPosReset() {

        var activity = getInGameActivity();

        activity.onboardingControlsIndicator.deactivateIndicator();

        if (initialTouchPos.HasValue) {
            activity.onboardingControlsIndicator.activateIndicator(initialTouchPos.Value);
        }
    }

    protected override void onCurrentTouchPosEnd() {

        getInGameActivity().onboardingControlsIndicator.deactivateIndicator();
    }

    protected override bool calculateNewAngle() {

        var activity = getInGameActivity();

        float touchPosX = currentTouchPos.Value.x; 
        float distance = touchPosX - initialTouchPos.Value.x;
        float newAngle = initialAxisAngle + 30 * distance * currentRotationMultiplier;

        activity.axis.setRotationAngle(newAngle);

        activity.onboardingControlsIndicator.updateIndicator(touchPosX);

        return true;
    }

}

