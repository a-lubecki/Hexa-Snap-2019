/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Activity13b : Activity12_13 {


    private Text textTitle1;
    private Text textTitle2;


    protected override string[] getPrefabNamesToLoad() {
        return new string[] { "Activity12-13.content", "Activity13b" };
    }
    
    protected override string getTextTitle() {
        return Tr.get("Activity13.Title");
    }

    protected override int getScoreValue() {
        return ((BundlePush13b) bundlePush).score;
    }

    protected override int getLevelValue() {
        return ((BundlePush13b)bundlePush).level;
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
        return 100;
    }

    protected override CharacterSituation getEndGameCharacterSituation() {

        if (!GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("13b.Default")) {

            return new CharacterSituation()
                .enqueue(Tr.arr("13b.Default", 0, 3))
                .enqueueExpression(CharacterRes.EXPR_AMAZED, 6)
                .enqueueJoin()
                .enqueue(Tr.arr("13b.Default", 3, 3))
                .enqueueExpression(CharacterRes.EXPR_CUTE, 5)
                .enqueueExpression(CharacterRes.EXPR_MEME_NOTICE_ME_SENPAI, 5)
                .enqueueExpression(CharacterRes.EXPR_SMILE, 3)
                .enqueueJoin()
                .enqueue(Tr.arr("13b.Default", 6, 1))
                .enqueueExpression(CharacterRes.EXPR_EYES_CLOSED, 4)
                .enqueueUniqueDisplay("13b.Default");
        }

        return null;
    }

    protected override void onCreate() {
        base.onCreate();

        textTitle1 = updateText("TextTitle1", Tr.get("Activity13b.Text.End.Hardcore1"));
        textTitle2 = updateText("TextTitle2", Tr.get("Activity13b.Text.End.Hardcore2"));

        hexacoinsWalletBehavior.setOnlyDisplayedOnChanges(true);
        hexacoinsWalletBehavior.transform.localPosition = Vector3.zero;

        //hide all
        textTitle1.gameObject.SetActive(false);
        textTitle2.gameObject.SetActive(false);
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

    protected override IEnumerator showSpecificViews() {

        textTitle1.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        textTitle2.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
    }

}


public struct BundlePush13b : BaseBundle {
    
    public int level;
    public int score;

}
