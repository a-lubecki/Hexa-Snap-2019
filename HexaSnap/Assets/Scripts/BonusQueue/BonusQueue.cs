/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;


public class BonusQueue : InGameModel, GameTimerListener {

	private static BonusQueueListener to(BaseModelListener listener) {
		return (BonusQueueListener) listener;
	}


	private List<TimerRunningBonus> queue = new List<TimerRunningBonus>();


	public BonusQueue(Activity10 activity) : base(activity) {
		
	}

	public int getNbTimers() {
		return queue.Count;
	}

	public TimerRunningBonus getTimer(int pos) {
		return queue[pos];
	}

	public int getTimerPos(TimerRunningBonus timer) {

		if (timer == null) {
			throw new ArgumentException();
		}

		int pos = queue.IndexOf(timer);

		if (pos < 0) {
			throw new InvalidOperationException("Timer not found in queue : nb = " + queue.Count);
		}

		return pos;
	}

	public void enqueueBonus(ItemBonus itemBonus) {

		if (itemBonus == null) {
			throw new ArgumentException();
		}

		TimerRunningBonus timer = new TimerRunningBonus(activity, itemBonus, 20);
		timer.addListener(this);

		queue.Add(timer);

		timer.start();

		notifyListeners(listener => {
			to(listener).onBonusQueueEnqueue(this, timer);
		});
	}

	protected void dequeueBonus(TimerRunningBonus timer) {

		if (timer == null) {
			throw new ArgumentException();
		}

		timer.cancel();

		if (queue.Remove(timer)) {

			notifyListeners(listener => {
				to(listener).onBonusQueueDequeue(this, timer);
			});
		}
	}

	public void clear() {

		foreach (TimerRunningBonus timer in queue) {
			timer.cancel();
		}

		queue.Clear();

		notifyListeners(listener => {
			to(listener).onBonusQueueClear(this);
		});
	}

	public float getEnqueuedScoreMultiplier() {

		float multiplier = 1;

		foreach (TimerRunningBonus timer in queue) {

			BonusType type = timer.itemBonus.bonusType;

			if (type == activity.bonusManager.bonusTypeMultiplier) {
				multiplier *= 2f;
			} else if (type == activity.bonusManager.bonusTypeDivider) {
				multiplier *= 0.5f;
			}
		}

		return multiplier;
	}

	public float getEnqueuedRotationMultiplier() {

		int multiplier = 1;

		foreach (TimerRunningBonus timer in queue) {

			BonusType type = timer.itemBonus.bonusType;

			if (type == activity.bonusManager.bonusTypeInversion) {
				multiplier *= -1;
			}
		}

		return multiplier;
	}

	public float getEnqueuedGenerationFrequency() {
		
		float frequency = 1;

		//use dividers to avoid generating items too fast when there are many enqueued bonuses
		float positiveDivider = 1;
		float negativeDivider = 1;

        var bonusManager = activity.bonusManager;

		foreach (TimerRunningBonus timer in queue) {

			BonusType type = timer.itemBonus.bonusType;

            if (type == bonusManager.bonusTypeShortage || type == bonusManager.bonusTypeSlowDown) {
				
				frequency *= (1 + 0.5f / positiveDivider); // 1.5 => 1
				positiveDivider++;

            } else if (type == bonusManager.bonusTypeProfusion || type == bonusManager.bonusTypeSpeedUp) {
				
				frequency /= (1 + 0.5f / negativeDivider);
				negativeDivider++;
			}
		}

		return frequency;
	}

	public bool hasEnqueuedErosion() {

		foreach (TimerRunningBonus timer in queue) {

			BonusType type = timer.itemBonus.bonusType;
			if (type == activity.bonusManager.bonusTypeErosion) {
				return true;
			}
		}

		return false;
	}

	public float getEnqueuedTimeMultiplier() {

		float multiplier = 1;

		//use dividers to avoid generating items too fast when there are many enqueued bonuses
		float positiveDivider = 1;
		float negativeDivider = 1;

		foreach (TimerRunningBonus timer in queue) {

			BonusType type = timer.itemBonus.bonusType;
			if (type == activity.bonusManager.bonusTypeSpeedUp) {
				
				multiplier *= (1 + 0.2f / positiveDivider); // 1.2 => 1
				positiveDivider++;

			} else if (type == activity.bonusManager.bonusTypeSlowDown) {
				
				multiplier /= (1 + 0.2f / negativeDivider);
				negativeDivider++;
			}
		}

		//cap the value to avoid bugs
		if (multiplier < 0.5) {
			multiplier = 0.5f;
		} else if (multiplier > 1.2) {
			multiplier = 1.2f;
		}

		return multiplier;
	}


	BaseModelBehavior BaseModelListener.getModelBehavior() {
		return null;
	}

	void GameTimerListener.onTimerRunningBonusStart(GameTimer timer) {
		//do nothing
	}

	void GameTimerListener.onTimerRunningBonusProgress(GameTimer timer) {
		//do nothing
	}

	void GameTimerListener.onTimerRunningBonusFinish(GameTimer timer) {
		dequeueBonus((TimerRunningBonus) timer);
	}

	void GameTimerListener.onTimerRunningBonusCancel(GameTimer timer) {
		//do nothing
	}

    void GameTimerListener.onTimerRunningBonusDurationChange(GameTimer timer) {
        //do nothing
    }
}

