/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public class Activity1b : Activity1 {
    

    protected override string[] getPrefabNamesToLoad() {
		return new string[] { "Activity1b" };
	}

	protected override Line newPushLine(BaseActivity next) {

		if (next is Activity4) {

			return new Line(
                clickedMenuButton.transform.position,
				(next as Activity4).getTitleLinePos(),
				SegmentThickness.LARGE,
				1,
				true);
		}

		return base.newPushLine(next);
	}

	protected override BaseMenuButton newMenuButtonStartArcade() {

		return MenuButtonIcon.newButtonDefault(
			Tr.get("Activity1b.Button.Play"),
			"MenuButton.Play"
		);
	}

    protected override CharacterSituation getFirstCharacterSituation(bool isFirstResume) {

        if (!isFirstResume) {
            return null;
        }

        return new CharacterSituation()
            .enqueueTr("1b.Default")
            .enqueueDelayExpression(3)
            .enqueueExpression(CharacterRes.EXPR_SMILE_RIGHT, 2)
            .enqueueDelayMove(3)
            .enqueueMove(CharacterRes.MOVE_BOUNCE)
            .enqueueHide();
    }

    protected override bool hasAdBanner() {
        return false;
    }

	protected override void onPause() {
		base.onPause();
		
		if (nextActivity is Activity4) {

			replaceBy(new Activity1a());
		}
	}

	protected override void onButtonStartArcadeClick() {

		push(new Activity4a());

	}


}

