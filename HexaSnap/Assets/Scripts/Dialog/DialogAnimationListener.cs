/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public interface DialogAnimationListener {
	
	void onDialogAnimationStart(Animation anim, bool isReversed);

	void onDialogAnimationFinish(Animation anim, bool isReversed);

}

