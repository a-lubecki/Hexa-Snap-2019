/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine.UI;
using System.Linq;


public abstract class Activity2 : BaseUIActivity {


	protected MenuButtonBehavior buttonPlay;
	protected MenuButtonBehavior buttonUpgrades;


	protected override MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager) {
		return markerManager.markerBGameMode;
	}

	protected override Line newPushLine(BaseActivity next) {
		
		if (next is Activity3) {
			
			return new Line(
                clickedMenuButton.transform.position,
				next.markerRef.posSafeAreaBottomLeft,
				SegmentThickness.LARGE,
				1,
				true);
		}

		if (next is Activity4) {
			
			return new Line(
                clickedMenuButton.transform.position,
				(next as Activity4).getTitleLinePos(),
				SegmentThickness.LARGE,
				1,
				true);
		}

		if (next is Activity20) {

			return new Line(
                clickedMenuButton.transform.position,
				next.markerRef.posSafeAreaBottomLeft,
				SegmentThickness.LARGE,
				0.5f,
				true);
		}

		if (next is Activity50) {

			return new Line(
                clickedMenuButton.transform.position,
				next.markerRef.posSafeAreaBottomLeft,
				SegmentThickness.LARGE,
				0.1f,
				true);
		}

		return null;
	}

    protected override bool hasDefaultHexacoinWallet() {
        return true;
    }

    protected abstract Graph getGraph();

    protected abstract string getTextBest();

	protected override void onCreate() {
		base.onCreate();

        buttonPlay = createButtonGameObject(
            this,
			"PosPlay",
			MenuButtonIcon.newButtonDefault(
				Tr.get("Activity2.Button.Play"),
				"MenuButton.Play"
			)
		);

        buttonUpgrades = createButtonGameObject(
            this,
			"PosUpgrades",
			MenuButtonIcon.newButtonDefault(
                Tr.get("Activity2.Button.Upgrades"),
                "MenuButton.Upgrades"
			)
		);

        /*
        buttonHiScores = createButtonGameObject(
            this,
			"PosHiScores",
			MenuButtonIcon.newButtonDefault(
				Tr.get("Activity2.Button.HiScores"),
				"MenuButton.HiScores"
			)
		);*/
	}

    protected override void onPreResume() {
        base.onPreResume();

        //update score as it can change in the next screens after a play
        findChildTransform("TextBest").GetComponent<Text>().text = getTextBest();

        //show badge for advanced players
        if (gameManager.maxArcadeLevel >= 5) {
            buttonUpgrades.setBadgeValue(getGraph().getSortedNodesZone().Count((zone) => zone.state == NodeZoneState.LOCKED));
        } else {
            buttonUpgrades.setBadgeText(null);
        }

    }

}

