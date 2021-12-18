/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;

public abstract class InGameModel : BaseModel {
		

	public Activity10 activity { get; private set; }


	protected InGameModel(Activity10 activity) : base() {

		if (activity == null) {
			throw new ArgumentException();
		}

		this.activity = activity;
	}


}

