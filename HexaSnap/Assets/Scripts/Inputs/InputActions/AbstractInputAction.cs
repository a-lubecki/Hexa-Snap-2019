/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;

public abstract class AbstractInputAction {

	public abstract KeyCode[] getDefaultActionKeys();

	public virtual bool areActionKeysReleased() {
		return false;
	}

	public virtual bool areActionKeysLongPressed() {
		return false;
	}


	public abstract int[] getDefaultActionMouseButtons();

	public virtual bool areActionMouseButtonsReleased() {
		return false;
	}

	public virtual bool areActionMouseButtonsLongPressed() {
		return false;
	}


	public abstract bool execute();


	public virtual bool isActionDone() {

		KeyCode[] keys = getDefaultActionKeys();

		bool keysDone = false;
		if (keys != null) {
			if (areActionKeysReleased()) {
				keysDone = (isAnyKeyReleased(keys));
			} else {
				keysDone = (isAnyKeyPressed(keys, areActionKeysLongPressed()));
			}
		}

		int[] mouseButtons = getDefaultActionMouseButtons();

		bool mouseButtonsDone = false;
		if (mouseButtons != null) {
			if (areActionMouseButtonsReleased()) {
				mouseButtonsDone = (isAnyMouseButtonReleased(mouseButtons));
			} else {
				mouseButtonsDone = (isAnyMouseButtonPressed(mouseButtons, areActionMouseButtonsLongPressed()));
			}
		}

		return (keysDone || mouseButtonsDone);
	}


	protected bool isKeyPressed(KeyCode key, bool longPress) {

		if(longPress) {
			return Input.GetKey(key);
		}
		return Input.GetKeyDown(key);
	}
	
	
	protected bool isAnyKeyPressed(KeyCode[] keys, bool longPress) {
		
		foreach (KeyCode k in keys) {
			if (isKeyPressed(k, longPress)) {
				return true;
			}
		}
		
		return false;
	}
	
	protected bool isAllKeysPressed(KeyCode[] keys, bool longPress) {
		
		foreach (KeyCode k in keys) {
			if (!isKeyPressed(k, longPress)) {
				return false;
			}
		}
		
		return true;
	}

	protected bool isAnyKeyReleased(KeyCode[] keys) {

		foreach (KeyCode k in keys) {
			if (Input.GetKeyUp(k)) {
				return true;
			}
		}

		return false;
	}


	protected bool isMouseButtonPressed(int button, bool longPress) {
        
		if(longPress) {
			return Input.GetMouseButton(button);
		}
		return Input.GetMouseButtonDown(button);
	}


	protected bool isAnyMouseButtonPressed(int[] buttons, bool longPress) {

		foreach (int b in buttons) {
			if(isMouseButtonPressed(b, longPress)) {
				return true;
			}
		}

		return false;
	}

	protected bool isAllMouseButtonPressed(int[] buttons, bool longPress) {

		foreach (int b in buttons) {
			if(!isMouseButtonPressed(b, longPress)) {
				return false;
			}
		}

		return true;
	}

	protected bool isAnyMouseButtonReleased(int[] buttons) {

		foreach (int b in buttons) {
			if (Input.GetMouseButtonUp(b)) {
				return true;
			}
		}

		return false;
	}

}

