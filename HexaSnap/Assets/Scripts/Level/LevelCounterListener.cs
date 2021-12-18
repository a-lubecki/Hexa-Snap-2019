/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public interface LevelCounterListener : BaseModelListener {

	void onLevelCounterLevelChange(LevelCounter levelCounter, int lastLevel, int level);
    
}

