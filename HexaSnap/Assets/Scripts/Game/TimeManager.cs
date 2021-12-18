/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;

public class TimeManager : InGameModel {

	private float timeScalePhysics;
	private float timeScalePlay;

	private HashSet<object> pauseHolders = new HashSet<object>();


	public TimeManager(Activity10 activity) : base(activity) {

		timeScalePhysics = 1;
		timeScalePlay = 1;
	}

	public void setTimeScalePhysics(float timeScale) {

		if (timeScale < 0) {
			throw new ArgumentException();
		}

		timeScalePhysics = timeScale;
	}

	public void setTimeScalePlay(float timeScale) {
		
		if (timeScale < 0) {
			throw new ArgumentException();
		}

		timeScalePlay = timeScale;
	}

	public float getTotalTimeScalePhysics() {

		float res = timeScalePhysics * getTotalTimeScalePlay();
		if (res > 0) {
			res *= activity.bonusQueue.getEnqueuedTimeMultiplier();
		}
	
		return res;
	}

	public float getTotalTimeScalePlay() {

		if (isPaused()) {
			return 0;
		}

		return timeScalePlay;
	}


	private bool isPaused() {
		return (pauseHolders.Count > 0);
	}

	/*
	 * Pause the game with a object holding the pause.
	 * To resume the game, the holding object must be previously set or the game will be stuck indefinitely.
	 */
	public void pause(object holder) {

		if (holder == null) {
			throw new ArgumentException();
		}
		if (pauseHolders.Contains(holder)) {
            return;
        }

		pauseHolders.Add(holder);
	}

	/*
	 * Resume the game with a previously set holding object
	 */
	public void resume(object holder) {

		if (holder == null) {
			throw new ArgumentException();
		}
		if (!pauseHolders.Contains(holder)) {
			return;
		}

		pauseHolders.Remove(holder);
	}

}

