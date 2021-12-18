/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public class ActionReplaceActivity : BaseActivityAction {

	public readonly BaseActivity other;

	public ActionReplaceActivity(BaseActivity other) {

		this.other = other;
	}

	public void processAction(BaseActivity currentActivity) {
		currentActivity.processReplaceActivity(other);
	}

}
