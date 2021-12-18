/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Activity13c : Activity12_13 {
    

    private Text textTarget;
    private Text textTargetValue;

    private Transform trAdvance;


    protected override string[] getPrefabNamesToLoad() {
        return new string[] { "Activity12-13.content", "Activity13c" };
    }
    
    protected override string getTextTitle() {
        return Tr.get("Activity13c.Title");
    }
    
    protected override int getScoreValue() {
        return ((BundlePush13c) bundlePush).score;
    }

    protected override int getLevelValue() {
        //no level in time attack mode
        return 0;
    }

    protected override float getTimeSecValue() {
        return ((BundlePush13c)bundlePush).timeSec;
    }

    protected override string getTextStats() {

        return string.Format(
            Tr.get("Activity13c.Text.Stats"),
            Constants.getDisplayableScore(gameManager.maxTimeAttackScore));
    }

    protected override int getNbHexacoinsToEarn() {
        return 0;
    }

    protected override CharacterSituation getEndGameCharacterSituation() {

        if (!GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("13c.Default")) {

            return new CharacterSituation()
                .enqueueTr("13c.Default")
                .enqueueDelayMove(4)
                .enqueueMove(CharacterRes.MOVE_JUMP)
                .enqueueExpression(CharacterRes.EXPR_CUTE, 3)
                .enqueueExpression(CharacterRes.EXPR_DEFAULT, 1)
                .enqueueExpression(CharacterRes.EXPR_EMOJI_LAUGH, 2)
                .enqueueExpression(CharacterRes.EXPR_DEFAULT, 2)
                .enqueueUniqueDisplay("13c.Default");
        }

        return null;
    }

    protected override void onCreate() {
        base.onCreate();

        float timeSec = ((BundlePush13c) bundlePush).timeSec;
        trAdvance = updateStars(timeSec);

        textTarget = updateText("TextTarget", Tr.get("Activity13c.Text.Time"));
        textTargetValue = updateText("TextTargetValue", Constants.getDisplayableTimeSec(timeSec));

        //hide all
        trAdvance.gameObject.SetActive(false);
        textTarget.gameObject.SetActive(false);
        textTargetValue.gameObject.SetActive(false);
    }

    protected override bool isGameOverScreen() {
        return false;
    }

    protected override bool canContinuePlaying() {
        return false;
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


public struct BundlePush13c : BaseBundle {

    public float timeSec;
    public int score;

}
