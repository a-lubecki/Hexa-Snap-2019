/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public class Activity24a : Activity24 {


	private MenuButtonBehavior buttonLogin;


	protected override string[] getPrefabNamesToLoad() {
		return new string[] { "Activity24a" };
	}

	protected override Line newPushLine(BaseActivity next) {

		if (next is Activity30) {

			return new Line(
                clickedMenuButton.transform.position,
				next.markerRef.posSafeAreaBottomLeft,
				SegmentThickness.LARGE,
				1,
				true);
		}

		return null;
	}

	protected override void onCreate() {
		base.onCreate();

        buttonLogin = createButtonGameObject(
            this,
			"PosLogin",
			MenuButtonIcon.newButtonDialog(
				Tr.get("Activity24a.Button.Login"),
				"MenuButton.Options"
			)
		);

	}

	protected override void onResume(bool isFirst) {
		base.onResume(isFirst);

		if (!isFirst && LoginManager.Instance.isLoggedInFacebook()) {
			//let the popup close when the user has logged in
			pop();
		}
	}

    protected override string getTextThanks(bool hadRemovedAds) {
		return Tr.get("Activity24a.Text.Thanks");
	}

	protected override void onButtonClick(MenuButtonBehavior menuButton) {

		if (menuButton == buttonLogin) {

            BundlePush30 b = new BundlePush30 {
                originActivityName = getActivityName()
            };

            push(new Activity30().setBundlePush(b));

		} else {

			base.onButtonClick(menuButton);
		}
	}

}

