/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;


public class BonusQueueBehavior : InGameModelBehavior, BonusQueueListener {

	public BonusQueue bonusQueue {
		get {
			return (BonusQueue) model;
		}
	}
    

	BaseModelBehavior BaseModelListener.getModelBehavior() {
		return this;
	}

	void BonusQueueListener.onBonusQueueEnqueue(BonusQueue bonusQueue, TimerRunningBonus timer) {

		foreach (TimerRunningBonusBehavior tb in transform.GetComponentsInChildren<TimerRunningBonusBehavior>()) {
			
			moveTimer(tb, bonusQueue.getTimerPos(tb.timer));
		}

		addTimer(timer);
	}

	void BonusQueueListener.onBonusQueueDequeue(BonusQueue bonusQueue, TimerRunningBonus timer) {

		foreach (TimerRunningBonusBehavior tb in transform.GetComponentsInChildren<TimerRunningBonusBehavior>()) {

			if (timer == tb.timer) {
				
				removeTimer(tb);

			} else {
				
				moveTimer(tb, bonusQueue.getTimerPos(tb.timer));
			}

		}

	}

	void BonusQueueListener.onBonusQueueClear(BonusQueue bonusQueue) {

		float delay = 0.5f;

		foreach (TimerRunningBonusBehavior tb in transform.GetComponentsInChildren<TimerRunningBonusBehavior>()) {

			removeTimer(tb, delay);

			delay += 0.1f;
		}

	}


	private void addTimer(TimerRunningBonus timer) {

		Vector3 endPosition = getTimerEndPosition(bonusQueue.getTimerPos(timer));

		//add a game object for the new timer
		GameObject gameObjectTimer = GameHelper.Instance.getPool().pickTimerRunningBonusGameObject(timer, transform, false, Constants.newVector3(endPosition, -5f, 0, 0));
        
		gameObjectTimer.GetComponent<TimerRunningBonusBehavior>().animatePosition(
			new PositionInterpolatorBundle(
				endPosition,
				Constants.INTERPOLATION_TIMER_QUEUE_IN,
				InterpolatorCurve.EASE_IN
			)
		);

	}

	private void moveTimer(TimerRunningBonusBehavior tb, int index) {

		tb.animatePosition(
			new PositionInterpolatorBundle(
				getTimerEndPosition(index),
				Constants.INTERPOLATION_TIMER_QUEUE_MOVE,
				InterpolatorCurve.EASE_OUT
			)
		);

	}

	private void removeTimer(TimerRunningBonusBehavior tb, float delay = 0) {

		tb.transform.SetParent(null);

		Vector3 foreseenCurrentPos = tb.getLastAnimatedPos();

		if (delay > 0) {
            Async.call(delay, () => {
				animateTimerRemove(tb, foreseenCurrentPos);
			});
		} else {
			animateTimerRemove(tb, foreseenCurrentPos);
		}

	}

	private void animateTimerRemove(TimerRunningBonusBehavior tb, Vector3 foreseenCurrentPos) {

        Vector3 currentPos = tb.transform.position;
        currentPos.z = -5;//put under the other running timers

		tb.animatePosition(
			new PositionInterpolatorBundle(
				new Vector3(foreseenCurrentPos.x - 5f, foreseenCurrentPos.y, currentPos.z),
				Constants.INTERPOLATION_TIMER_QUEUE_OUT,
				InterpolatorCurve.EASE_OUT
			),
            (_) => {

				//remove the finished timer game object
				GameHelper.Instance.getPool().storeTimerRunningBonusGameObject(tb);

			});
	}

	private Vector3 getTimerEndPosition(int index) {
		return new Vector3(transform.position.x - index * 0.75f, transform.position.y - index * 0.75f, -10);
	}
		
}

