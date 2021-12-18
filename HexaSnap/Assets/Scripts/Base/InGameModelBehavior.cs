/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public class InGameModelBehavior : BaseModelBehavior {
		

	public InGameModel inGameModel {
		get {
			return (InGameModel) model;
		}
	}

	public Activity10 getActivity() {
		return inGameModel.activity;
	}

}

