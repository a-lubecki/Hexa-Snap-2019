/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;

public class ScoreCounter : InGameModel {

	private static ScoreCounterListener to(BaseModelListener listener) {
		return (ScoreCounterListener) listener;
	}


	public int totalScore { get; private set; }


	public ScoreCounter(Activity10 activity) : base(activity) {

	}

	public void clearScore() {
		
		totalScore = 0;

		notifyListeners(listener => {
			to(listener).onScoreChanged(this);
		});

	}

	public void addScore(int newScore) {
		
		if (newScore <= 0) {
			throw new ArgumentException();
		}

        totalScore += newScore;

		notifyListeners(listener => {
			to(listener).onScoreChanged(this);
		});

	}

}

