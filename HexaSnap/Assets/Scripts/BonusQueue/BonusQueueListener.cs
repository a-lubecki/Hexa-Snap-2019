/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public interface BonusQueueListener : BaseModelListener {

	void onBonusQueueEnqueue(BonusQueue bonusQueue, TimerRunningBonus timer);

	void onBonusQueueDequeue(BonusQueue bonusQueue, TimerRunningBonus timer);

	void onBonusQueueClear(BonusQueue bonusQueue);

}

