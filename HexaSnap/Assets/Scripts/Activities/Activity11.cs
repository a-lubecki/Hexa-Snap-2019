/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public class Activity11 : BaseDialogActivity {


	public static readonly int POP_CODE_GIVE_UP = 1278;


	private MenuButtonBehavior buttonGiveUp;
	private MenuButtonBehavior buttonResume;
	private MenuButtonBehavior buttonOptions;


	protected override string[] getPrefabNamesToLoad() {
		return new string[] { "Activity11" };
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

	protected override string getDialogAnimationName() {
		return "ActivityDialog.Pause";
	}


	protected override void onCreate() {
		base.onCreate();

		updateText("TextDialogTitle", Tr.get("Activity11.Title"));

		buttonGiveUp = createButtonGameObject(
            this,
			"PosGiveUp",
            new MenuButtonIcon(
                Tr.get("Activity11.Button.GiveUp"),
                Constants.PATH_DESIGNS_MENUS + "MenuButton.Background.Destructive",
                Constants.COLOR_MENU_BUTTON_HIGHLIGHT,
                Constants.PATH_DESIGNS_MENUS + "MenuButton.GiveUp"
			)
		);

        buttonResume = createButtonGameObject(
            this,
			"PosResume",
			MenuButtonIcon.newButtonDialog(
				Tr.get("Activity11.Button.Resume"),
				"MenuButton.Play"
			)
		);

        buttonOptions = createButtonGameObject(
            this,
			"PosOptions",
			MenuButtonIcon.newButtonDialog(
				Tr.get("Activity11.Button.Options"),
				"MenuButton.Options"
			)
		);

	}


    protected override void onButtonClick(MenuButtonBehavior menuButton) {

        if (menuButton == buttonGiveUp) {

            pop(POP_CODE_GIVE_UP, null);

        } else if (menuButton == buttonResume) {

            pop();

        } else if (menuButton == buttonOptions) {

            BundlePush30 b = new BundlePush30 {
                originActivityName = getActivityName()
            };

            push(new Activity30().setBundlePush(b));

        } else {

            base.onButtonClick(menuButton);
        }
    }

}

