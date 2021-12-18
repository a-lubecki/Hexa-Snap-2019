/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;

public class InputActionDebugSwapGeneration : InGameInputAction {

	public override KeyCode[] getDefaultActionKeys() {
		return new KeyCode[] { 
			KeyCode.Space
		};
	}

	public override int[] getDefaultActionMouseButtons() {
		return null;
	}

	public override bool execute() {

		if (!base.execute()) {
			return false;
		}

		ItemsGenerator generator = getInGameActivity().itemsGenerator;
		if (generator.isGeneratingItems) {
			generator.stopGeneration();
			Debug.Log("STOPPED GEN");
            
			Debug.Log(getInGameActivity().axis.ToString());

		} else {
			generator.startGeneration();
			Debug.Log("STARTED GEN");
		}

		return true;
	}

}

