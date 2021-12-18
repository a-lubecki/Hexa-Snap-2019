/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public interface GameTimerListener : BaseModelListener {

	void onTimerRunningBonusStart(GameTimer timer);

	void onTimerRunningBonusProgress(GameTimer timer);

	void onTimerRunningBonusFinish(GameTimer timer);

	void onTimerRunningBonusCancel(GameTimer timer);

    void onTimerRunningBonusDurationChange(GameTimer timer);

}

