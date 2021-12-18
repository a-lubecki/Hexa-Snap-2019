/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public class Activity24b : Activity24 {


	protected override string[] getPrefabNamesToLoad() {
		return new string[] { "Activity24b" };
	}

    protected override string getTextThanks(bool hadRemovedAds) {

        //say there will be no more ads if there where ads before and no more now
        if (!hadRemovedAds && gameManager.hasRemovedAds) {
            return Tr.get("Activity24b.Text.ThanksNoAds");
        }

        return Tr.get("Activity24b.Text.Thanks");
	}

}

