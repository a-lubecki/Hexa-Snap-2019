/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;


public class LevelCounter : InGameModel {

	private static LevelCounterListener to(BaseModelListener listener) {
		return (LevelCounterListener) listener;
	}


	public int currentLevel { get; private set; }
    

    public LevelCounter(Activity10 activity) : base(activity) {

		//init first level
		currentLevel = 1;
        
    }

	public int getCorrectNewLevel(int newLevel) {

		if (newLevel < 1) {
			return 1;
		}

		return newLevel;
	}

	public int getCurrentCappedLevel() {

		if (currentLevel > Constants.MAX_LEVEL_ARCADE) {
			return Constants.MAX_LEVEL_ARCADE;
		}

		return currentLevel;
	}

	public void incrementLevel() {

		setCurrentLevel(currentLevel + 1);
	}

	public void decrementLevel() {

		setCurrentLevel(currentLevel - 1);
	}

	public void setCurrentLevel(int level) {

		int lastLevel = currentLevel;

		currentLevel = getCorrectNewLevel(level);

        if (currentLevel != lastLevel) {

			notifyListeners(listener => {
				to(listener).onLevelCounterLevelChange(this, lastLevel, currentLevel);
			});
		}
	}


}

