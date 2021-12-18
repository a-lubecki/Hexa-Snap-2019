/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;

public class InputActionRotateClockwise : InputActionRotate {
	
	public override KeyCode[] getDefaultActionKeys() {
		return new KeyCode[] { 
			KeyCode.RightArrow
		};
	}

    
	protected override void rotate(Axis axis) {
		axis.rotateClockwise(Constants.MAX_ROTATION_FORCE);
	}

}

