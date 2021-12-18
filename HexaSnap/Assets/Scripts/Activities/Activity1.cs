/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public abstract class Activity1 : BaseUIActivity {
		

	protected MenuButtonBehavior buttonStartArcade;
	private MenuButtonBehavior buttonOptions;


	protected override MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager) {
		return markerManager.markerAMainMenu;
	}

	protected override Line newPushLine(BaseActivity next) {

		if (next is Activity30) {

			return new Line(
                clickedMenuButton.transform.position,
				next.markerRef.posSafeAreaBottomLeft,
				SegmentThickness.LARGE,
				0.3f,
				true);
		}

		return null;
	}

	protected abstract BaseMenuButton newMenuButtonStartArcade();

	protected override void onCreate() {
		base.onCreate();

        //update the last saved start date event
        Prop.lastStart.put(Prop.currentStart.get());
        Prop.currentStart.putNow();

		buttonStartArcade = createButtonGameObject(
            this,
			"PosStartArcade",
			newMenuButtonStartArcade()
		);

        buttonOptions = createButtonGameObject(
            this,
			"PosOptions",
			MenuButtonIcon.newButtonDefault(
				Tr.get("Activity1.Button.Options"),
				"MenuButton.Options"
			)
        );

        playMusic(null);
	}

    protected abstract void onButtonStartArcadeClick();

	protected override void onButtonClick(MenuButtonBehavior menuButton) {

		if (menuButton == buttonStartArcade) {

            onButtonStartArcadeClick();

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

