

using UnityEngine;
/**
* Hexa Snap
* © Aurélien Lubecki 2019
* All Rights Reserved
*/
public class Activity2a : Activity2 {



	protected override string[] getPrefabNamesToLoad() {
		return new string[] { "Activity2a" };
	}

    protected override string getTitleForInit() {

        if (!gameManager.isArcadeHarcoreModeUnlocked()) {
            return Tr.get("Activity2a.Title");
        }

        return Tr.get("Activity2a.Title.Hardcore");
    }

    protected override CharacterSituation getFirstCharacterSituation(bool isFirstResume) {
        
        NodeZone nodeRoot = getGraph().getRootNode();
        if (nodeRoot.state == NodeZoneState.DISABLED || nodeRoot.state == NodeZoneState.LOCKED) {

            //no activated bonus activated by the player
            return new CharacterSituation()
                .enqueueTr("2a.Bonus");    
        }

        int maxLevel = gameManager.maxArcadeLevel;

        if (maxLevel <= 1) {

            return new CharacterSituation()
                .enqueueTr("2a.Level1")
                .enqueueDelayExpression(5)
                .enqueueExpression(CharacterRes.EXPR_MEME_CHALLENGE_ACCEPTED, 4);
        }

        if (maxLevel == 2) {

            return new CharacterSituation()
                .enqueueTr("2a.Level2")
                .enqueueMove(CharacterRes.MOVE_BOUNCE)
                .enqueueExpression(CharacterRes.EXPR_AMAZED, 3);
        }

        if (maxLevel == 19) {

            return new CharacterSituation()
                .enqueueTr("2a.Level19")
                .enqueueMove(CharacterRes.MOVE_BOUNCE)
                .enqueueDelayMove(5)
                .enqueueMove(CharacterRes.MOVE_STRETCH)
                .enqueueExpression(CharacterRes.EXPR_AMAZED, 3)
                .enqueueExpression(CharacterRes.EXPR_DEFAULT, 1)
                .enqueueExpression(CharacterRes.EXPR_BLINK, 0.5f)
                .enqueueExpression(CharacterRes.EXPR_DEFAULT, 1)
                .enqueueExpression(CharacterRes.EXPR_MEME_ME_GUSTA, 2.1f);
        }

        if (maxLevel == 20) {

            return new CharacterSituation()
                .enqueueTr("2a.Level20")
                .enqueueExpression(CharacterRes.EXPR_SAD, 2)
                .enqueueExpression(CharacterRes.EXPR_DEFAULT_MEH, 2)
                .enqueueExpression(CharacterRes.EXPR_MEME_CHALLENGE_ACCEPTED, 4)
                .enqueueDelayMove(5)
                .enqueueMove(CharacterRes.MOVE_BOUNCE)
                .enqueueMove(CharacterRes.MOVE_BOUNCE);
        }

        if (gameManager.isArcadeHarcoreModeBeaten()) {

            return new CharacterSituation()
                .enqueueTr("2a.End")
                .enqueueMove(CharacterRes.MOVE_SHIVER)
                .enqueueExpression(CharacterRes.EXPR_EMOJI_FEAR, 2)
                .enqueueExpression(CharacterRes.EXPR_DEFAULT_MEH, 3)
                .enqueueExpression(CharacterRes.EXPR_MEME_NOTICE_ME_SENPAI, 7);
        }

        if (gameManager.isArcadeHarcoreModeUnlocked()) {

            return new CharacterSituation()
                .enqueueTr("2a.Hardcore")
                .enqueueExpression(CharacterRes.EXPR_SUNGLASSES, 12)
                .enqueueMove(CharacterRes.MOVE_ROTATE)
                .enqueueDelayMove(7)
                .enqueueMove(CharacterRes.MOVE_JUMP);
        }

        return new CharacterSituation()
            .enqueueTr("2a.Default");
    }

    protected override Graph getGraph() {
        return GameHelper.Instance.getUpgradesManager().graphArcade;
    }

    protected override string getTextBest() {

        string level = "";
        if (!gameManager.isArcadeHarcoreModeUnlocked()) {
            level = gameManager.maxArcadeLevel + "/" + Constants.MAX_LEVEL_ARCADE;
        } else if (!gameManager.isArcadeHarcoreModeBeaten()) {
            level = gameManager.maxArcadeLevel + "/" + Constants.MAX_LEVEL_HARDCORE;
        } else {
            level = gameManager.maxArcadeLevel.ToString();
        }

        return string.Format(Tr.get("Activity2a.Text.Best"), level, Constants.getDisplayableScore(gameManager.maxArcadeScore));
    }

	protected override void onButtonClick(MenuButtonBehavior menuButton) {

		if (menuButton == buttonPlay) {

			push(new Activity3());

		} else if (menuButton == buttonUpgrades) {

			push(new Activity20a());
            /*
		} else if (menuButton == buttonHiScores) {

			push(new Activity50a());
*/
		} else {
			
			base.onButtonClick(menuButton);
		}
	}

}

