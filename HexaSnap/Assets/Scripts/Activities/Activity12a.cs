/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections;
using UnityEngine.UI;


public class Activity12a : Activity12 {


    private Text textTarget;
    private Text textTargetValue;


    protected override string[] getPrefabNamesToLoad() {
        return new string[] { "Activity12-13.content", "Activity12a" };
    }

    protected override int getLastScoreValue() {
        return ((BundlePush12a)bundlePush).lastScore;
    }

    protected override int getScoreValue() {
        return ((BundlePush12a)bundlePush).score;
    }

    protected override int getLastLevelValue() {
        return ((BundlePush12a)bundlePush).lastLevel;
    }

    protected override int getLevelValue() {
        return ((BundlePush12a)bundlePush).level;
    }

    protected override float getLastTimeSecValue() {
        //no time in arcade mode
        return 0;
    }

    protected override float getTimeSecValue() {
        //no time in arcade mode
        return 0;
    }

    protected override string getTextStats() {

		return string.Format(
            Tr.get("Activity12a.Text.Stats"), 
            gameManager.maxArcadeLevel.ToString(),
            Constants.getDisplayableScore(gameManager.maxArcadeScore));
	}


    protected override void onCreate() {
        base.onCreate();
        
        textTarget = updateText("TextTarget", Tr.get("Activity12a.Text.Level"));
        textTargetValue = updateText("TextTargetValue", ((BundlePush12a) bundlePush).level.ToString());

        //hide all
        textTarget.gameObject.SetActive(false);
        textTargetValue.gameObject.SetActive(false);
    }

    protected override bool isArcadeMode() {
        return true;
    }

    protected override IEnumerator showSpecificViews() {

        textTarget.gameObject.SetActive(true);
        textTargetValue.gameObject.SetActive(true);

        yield break;
    }

}

public struct BundlePush12a : BaseBundle {

    public int lastLevel;
    public int level;
    public int lastScore;
    public int score;

}
