/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;

public class InputActionRotateCounterClockwise : InputActionRotate {
	
	public override KeyCode[] getDefaultActionKeys() {
		return new KeyCode[] { 
			KeyCode.LeftArrow
		};
	}
    
    protected override void rotate(Axis axis) {
        axis.rotateCounterClockwise(Constants.MAX_ROTATION_FORCE);
    }

}

