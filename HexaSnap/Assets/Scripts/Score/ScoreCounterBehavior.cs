/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */
 
using System.Globalization;
using UnityEngine.UI;


public class ScoreCounterBehavior : InGameModelBehavior, ScoreCounterListener {

	public ScoreCounter scoreCounter {
		get {
			return (ScoreCounter) model;
		}
	}

    
	private Text textScore;

	protected override void onAwake() {
		base.onAwake();
	
		textScore = GetComponent<Text>();
	}
    
    protected override void onInit() {
        base.onInit();

        updateScore();
    }

    BaseModelBehavior BaseModelListener.getModelBehavior() {
		return this;
	}

	void ScoreCounterListener.onScoreChanged(ScoreCounter scoreCounter) {

		updateScore();
	}

	private void updateScore() {
        textScore.text = Constants.getDisplayableScore(scoreCounter.totalScore);
    }

}

