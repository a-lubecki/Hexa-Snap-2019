/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public abstract class Activity50 : BaseUIActivity {


	protected override MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager) {
		return markerManager.markerGModeSpecificCredits;
	}

	protected override string[] getPrefabNamesToLoad() {
		return new string[] { "Activity50" };
	}

	protected override string getTitleForInit() {
		return Tr.get("Activity50.Title");
	}


	protected override void onCreate() {
		base.onCreate();


	}


}

