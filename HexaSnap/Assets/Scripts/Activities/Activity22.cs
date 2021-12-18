/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public class Activity22 : BaseDialogActivity {


	private MenuButtonBehavior buttonCancel;
	private MenuButtonBehavior buttonShop;


	protected override string[] getPrefabNamesToLoad() {
		return new string[] { "Activity22" };
	}

	protected override Line newPushLine(BaseActivity next) {

		if (next is Activity23) {

			return new Line(
                clickedMenuButton.transform.position,
				next.markerRef.posSafeAreaBottomLeft,
				SegmentThickness.LARGE,
				1,
				true);
		}

		return null;
	}

	protected override string getDialogAnimationName() {
		return "ActivityDialog.Pause";
	}


	protected override void onCreate() {
		base.onCreate();


		updateText("TextDialogTitle", Tr.get("Activity22.Title"));

        buttonCancel = createButtonGameObject(
            this,
			"PosCancel",
			MenuButtonIcon.newButtonDialog(
				Tr.get("Activity22.Button.Cancel"),
				"MenuButton.Close"
			)
		);

        buttonShop = createButtonGameObject(
            this,
			"PosShop",
			MenuButtonIcon.newButtonDialog(
				Tr.get("Activity22.Button.Shop"),
				"MenuButton.Shop"
			)
		);
	}

    protected override void onPreResume() {
        base.onPreResume();

        buttonShop.setBadgeValue(ShopItem.getNbFreeAvailableItems());
    }

    protected override void onResume(bool isFirst) {
        base.onResume(isFirst);

        //close the dialog if coming from the shop
        if (!isFirst) {
            pop();
        }
    }

    protected override void onDialogShown() {
        base.onDialogShown();

        buttonShop.setBadgeValue(ShopItem.getNbFreeAvailableItems());
    }

    protected override void onButtonClick(MenuButtonBehavior menuButton) {

		if (menuButton == buttonCancel) {

			pop();

		} else if (menuButton == buttonShop) {

            push(new Activity23());

		} else {

			base.onButtonClick(menuButton);
		}

	}

}

