/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public abstract class Activity24 : BaseDialogActivity {


	private MenuButtonBehavior buttonClose;


	protected override string getDialogAnimationName() {
		return "ActivityDialog.Pause";
	}


	protected override void onCreate() {
		base.onCreate();


		BundlePush24 b = (BundlePush24) bundlePush;


		updateText("TextDialogTitle", Tr.get("Activity24.Title"));

        updateText("TextThanks", string.Format(getTextThanks(b.hadRemovedAds), b.nbPaidHexacoins));

        buttonClose = createButtonGameObject(
            this,
			"PosClose",
			MenuButtonIcon.newButtonDialog(
				Tr.get("Activity24.Button.Close"),
				"MenuButton.Close"
			)
		);

	}

    protected abstract string getTextThanks(bool hadRemovedAds);


	protected override void onButtonClick(MenuButtonBehavior menuButton) {

		if (menuButton == buttonClose) {

			pop();

		} else {

			base.onButtonClick(menuButton);
		}

	}

}


public struct BundlePush24 : BaseBundle {

	public int nbPaidHexacoins;
    public bool hadRemovedAds;
}
