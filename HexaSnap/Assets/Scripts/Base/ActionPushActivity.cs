/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public class ActionPushActivity : BaseActivityAction {

	public readonly BaseActivity next;
	public readonly bool isRoot;

	public ActionPushActivity(BaseActivity next, bool isRoot) {

		this.next = next;
		this.isRoot = isRoot;
	}

	public void processAction(BaseActivity currentActivity) {
		currentActivity.processPush(next, isRoot);
	}

}

