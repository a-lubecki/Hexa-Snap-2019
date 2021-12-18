/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */
 
using UnityEngine.UI;


public class LevelCounterBehavior : InGameModelBehavior, LevelCounterListener {

	public LevelCounter levelCounter {
		get {
			return (LevelCounter) model;
		}
	}


	private Text textTitle;


	protected override void onAwake() {
		base.onAwake ();

		textTitle = GetComponent<Text>();
	}
    
    protected override void onInit() {
        base.onInit();

        updateLevelText();
    }

    BaseModelBehavior BaseModelListener.getModelBehavior() {
		return this;
	}

	void LevelCounterListener.onLevelCounterLevelChange(LevelCounter levelCounter, int lastLevel, int level) {

        updateLevelText();
	}

    private void updateLevelText() {

        textTitle.text = string.Format(Tr.get("Activity10a.Text.Level"), levelCounter.currentLevel);
    }

}

