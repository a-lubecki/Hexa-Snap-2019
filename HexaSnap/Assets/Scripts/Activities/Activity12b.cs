/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Activity12b : Activity12 {


    private Text textTarget;
    private Text textTargetValue;

    private Transform trAdvance;


    protected override string[] getPrefabNamesToLoad() {
        return new string[] { "Activity12-13.content", "Activity12b" };
    }

    protected override int getLastScoreValue() {
        return ((BundlePush12b)bundlePush).lastScore;
    }

    protected override int getScoreValue() {
        return ((BundlePush12b) bundlePush).score;
    }

    protected override int getLastLevelValue() {
        //no level in time attack mode
        return 0;
    }

    protected override int getLevelValue() {
        //no level in time attack mode
        return 0;
    }

    protected override float getLastTimeSecValue() {
        return ((BundlePush12b)bundlePush).lastTimeSec;
    }

    protected override float getTimeSecValue() {
        return ((BundlePush12b)bundlePush).timeSec;
    }

    protected override string getTextStats() {
        
        return string.Format(
            Tr.get("Activity12b.Text.Stats"),
            Constants.getDisplayableScore(gameManager.maxTimeAttackScore));
    }


    protected override void onCreate() {
        base.onCreate();

        float timeSec = getTimeSecValue();
        trAdvance = updateStars(timeSec);

        textTarget = updateText("TextTarget", Tr.get("Activity12b.Text.Time"));
        textTargetValue = updateText("TextTargetValue", Constants.getDisplayableTimeSec(timeSec));

        //hide all
        trAdvance.gameObject.SetActive(false);
        textTarget.gameObject.SetActive(false);
        textTargetValue.gameObject.SetActive(false);
    }

    protected override bool isArcadeMode() {
        return false;
    }

    protected override IEnumerator showSpecificViews() {

        trAdvance.gameObject.SetActive(true);

        yield return new WaitForSeconds(1);

        textTarget.gameObject.SetActive(true);
        textTargetValue.gameObject.SetActive(true);
    }

}

public struct BundlePush12b : BaseBundle {

    public float lastTimeSec;
    public float timeSec;
    public int lastScore;
	public int score;

}
