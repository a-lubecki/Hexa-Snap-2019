/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public interface ScoreMultiplierListener : BaseModelListener {

    void onMultiplierChanged(ScoreMultiplier scoreMultiplier);

    void onTimerUpdate(ScoreMultiplier scoreMultiplier);

}

