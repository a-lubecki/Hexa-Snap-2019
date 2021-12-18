/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */
 

public abstract class InputActionRotate : InGameInputAction {

	public override bool areActionKeysLongPressed() {
		return true;
	}

	public override int[] getDefaultActionMouseButtons() {
		return null;
	}

    
	public override bool execute() {

		if (!base.execute()) {
			return false;
		}

		Axis axis = getInGameActivity().axis;
		if (axis == null) {
			return false;
		}
		
		rotate(axis);

		return true;
	}
    
	protected abstract void rotate(Axis axis);


}

