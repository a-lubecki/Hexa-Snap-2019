/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Activity13a : Activity12_13 {


    private Text textTitle1;
    private Text textTitle2;
    private Text textTitle3;


    protected override string[] getPrefabNamesToLoad() {
        return new string[] { "Activity12-13.content", "Activity13a" };
    }
    
    protected override string getTextTitle() {
        return Tr.get("Activity13.Title");
    }

    protected override int getScoreValue() {
        return ((BundlePush13a) bundlePush).score;
    }

    protected override int getLevelValue() {
        return ((BundlePush13a)bundlePush).level;
    }

    protected override float getTimeSecValue() {
        //no time in arcade mode
        return 0;
    }

    protected override string getTextStats() {

        return string.Format(
            Tr.get("Activity13.Text.Stats"),
            gameManager.maxArcadeLevel.ToString(),
            Constants.getDisplayableScore(gameManager.maxArcadeScore));
    }

    protected override bool hasDefaultHexacoinWallet() {
        return true;
    }

    protected override int getNbHexacoinsToEarn() {
        return 20;
    }

    protected override CharacterSituation getEndGameCharacterSituation() {

        if (!GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("13a.Default")) {

            return new CharacterSituation()
                .enqueue(Tr.arr("13a.Default", 0, 2))
                .enqueueExpression(CharacterRes.EXPR_HUNGRY, 4)
                .enqueueMove(CharacterRes.MOVE_BOUNCE)
                .enqueueJoin()
                .enqueue(Tr.arr("13a.Default", 2, 4))
                .enqueueExpression(CharacterRes.EXPR_SAD, 5)
                .enqueueExpression(CharacterRes.EXPR_DEFAULT_MEH, 3)
                .enqueueExpression(CharacterRes.EXPR_WORRIED, 3)
                .enqueueJoin()
                .enqueue(Tr.arr("13a.Default", 6, 2))
                .enqueueMove(CharacterRes.MOVE_TURN)
                .enqueueExpression(CharacterRes.EXPR_DETERMINED, 3)
                .enqueueExpression(CharacterRes.EXPR_MEME_CHALLENGE_ACCEPTED, 5)
                .enqueueUniqueDisplay("13a.Default");
        }
        
        return null;
    }

    protected override void onCreate() {
        base.onCreate();

        textTitle1 = updateText("TextTitle1", Tr.get("Activity13a.Text.End.Arcade1"));
        textTitle2 = updateText("TextTitle2", Tr.get("Activity13a.Text.End.Arcade2"));
        textTitle3 = updateText("TextTitle3", Tr.get("Activity13a.Text.End.Arcade3"));

        hexacoinsWalletBehavior.setOnlyDisplayedOnChanges(true);
        hexacoinsWalletBehavior.transform.localPosition = Vector3.zero;

        //hide all
        textTitle1.gameObject.SetActive(false);
        textTitle2.gameObject.SetActive(false);
        textTitle3.gameObject.SetActive(false);
    }

    protected override IEnumerator showSpecificViews() {

        textTitle1.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        textTitle2.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        textTitle3.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
    }

    protected override bool isGameOverScreen() {
        return false;
    }

    protected override bool canContinuePlaying() {
        return true;
    }

    protected override bool isArcadeMode() {
        return true;
    }
    
}


public struct BundlePush13a : BaseBundle {
    
    public int level;
    public int score;

}
