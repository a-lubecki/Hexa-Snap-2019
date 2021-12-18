/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;

public class MarkerManager {

    public readonly MarkerBehavior marker0;
	public readonly MarkerBehavior markerAMainMenu;
	public readonly MarkerBehavior markerBGameMode;
	public readonly MarkerBehavior markerCLevelChoice;
	public readonly MarkerBehavior markerDTransition;
	public readonly MarkerBehavior markerEPlay;
	public readonly MarkerBehavior markerFOptionsLogin ;
	public readonly MarkerBehavior markerGModeSpecificCredits;
	public readonly MarkerBehavior markerHBonusSpecific;
	public readonly MarkerBehavior markerIShop;
	public readonly MarkerBehavior MarkerJOptionsLogin;

	public MarkerManager() {

        marker0 = findMarkerBehavior("Marker0");
        markerAMainMenu = findMarkerBehavior("MarkerAMainMenu");
		markerBGameMode = findMarkerBehavior("MarkerBGameMode");
		markerCLevelChoice = findMarkerBehavior("MarkerCLevelChoice");
		markerDTransition = findMarkerBehavior("MarkerDTransition");
		markerEPlay = findMarkerBehavior("MarkerEPlay");
		markerFOptionsLogin  = findMarkerBehavior("MarkerFOptionsLoginCredits");
		markerGModeSpecificCredits = findMarkerBehavior("MarkerGModeSpecific");
		markerHBonusSpecific = findMarkerBehavior("MarkerHBonusSpecific");
		markerIShop = findMarkerBehavior("MarkerIShop");
		MarkerJOptionsLogin = findMarkerBehavior("MarkerJOptionsLogin");

	}

	private MarkerBehavior findMarkerBehavior(string markerName) {
		return GameObject.Find(markerName).GetComponent<MarkerBehavior>();
	}


}

