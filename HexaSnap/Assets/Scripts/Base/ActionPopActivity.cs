/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public class ActionPopActivity : BaseActivityAction {

	public readonly int popCode;
	public readonly BaseBundle bundlePop;

	public ActionPopActivity(int popCode, BaseBundle bundlePop) {

		this.popCode = popCode;
		this.bundlePop = bundlePop;
	}

	public void processAction(BaseActivity currentActivity) {
		currentActivity.processPop(popCode, bundlePop);
	}

}

