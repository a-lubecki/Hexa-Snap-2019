/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public class DialogBehavior : MonoBehaviour {


	public DialogAnimationListener listener;

	private Animation anim;


	void Start() {

		anim = GetComponent<Animation>();

		foreach (AnimationState s in anim) {

			AnimationClip c = anim.GetClip(s.name);

			if (c.wrapMode != WrapMode.Once) {
				//the dialog animations are only once anims
				continue;
			}

			AnimationEvent eBegin = new AnimationEvent();
			eBegin.functionName = "onAnimationBeginEvent";
			eBegin.time = 0;
			c.AddEvent(eBegin);

			AnimationEvent eEnd = new AnimationEvent();
			eEnd.functionName = "onAnimationEndEvent";
			eEnd.time = c.length;
			c.AddEvent(eEnd);
		}

	}


	private AnimationState getCurrentAnimationState() {

		if (!anim.isPlaying) {
			return null;
		}

		foreach (AnimationState s in anim) {

			if (anim.IsPlaying(s.name)) {
				return s;
			}
		}

		return null;
	}

	public void onAnimationBeginEvent() {

		if (listener == null) {
			return;
		}

		AnimationState s = getCurrentAnimationState();
		if (s == null) {
			return;
		}

		if (s.speed > 0) {
			listener.onDialogAnimationStart(anim, false);
		} else {
			listener.onDialogAnimationFinish(anim, true);
		}

	}

	public void onAnimationEndEvent() {

		if (listener == null) {
			return;
		}

		AnimationState s = getCurrentAnimationState();
		if (s == null) {
			return;
		}

		if (s.speed > 0) {
			listener.onDialogAnimationFinish(anim, false);
		} else {
			listener.onDialogAnimationStart(anim, true);
		}
	}

}

