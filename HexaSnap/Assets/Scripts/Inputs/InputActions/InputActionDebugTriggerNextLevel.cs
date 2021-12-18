/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;

public class InputActionDebugTriggerNextLevel : InGameInputAction {

	public override KeyCode[] getDefaultActionKeys() {
		return new KeyCode[] { 
			KeyCode.KeypadPlus
		};
	}

	public override int[] getDefaultActionMouseButtons() {
		return null;
	}

	public override bool execute() {

		if (!base.execute()) {
			return false;
		}

		BaseActivity activity = getInGameActivity();
		if (activity is Activity10a) {
			(activity as Activity10a).triggerNextLevel();
		}

		return true;
	}

}

