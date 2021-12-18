/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public class Activity2b : Activity2 {


	protected override string[] getPrefabNamesToLoad() {
		return new string[] { "Activity2b" };
	}

	protected override string getTitleForInit() {
		return Tr.get("Activity2b.Title");
	}

    protected override CharacterSituation getFirstCharacterSituation(bool isFirstResume) {

        if (!GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("2b.Onboarding")) {

            return new CharacterSituation()
                .enqueueTr("2b.Onboarding")
                .enqueueDelayExpression(5)
                .enqueueExpression(CharacterRes.EXPR_SURPRISED, 2)
                .enqueueExpression(CharacterRes.EXPR_EMOJI_POOP, 3)
                .enqueueUniqueDisplay("2b.Onboarding");
        }

        NodeZone nodeRoot = getGraph().getRootNode();
        if (nodeRoot.state == NodeZoneState.DISABLED || nodeRoot.state == NodeZoneState.LOCKED) {

            //no activated bonus activated by the player
            return new CharacterSituation()
                .enqueue(Tr.arr("2b.Bonus", 0, 2))
                .enqueueJoin()
                .enqueue(Tr.arr("2b.Bonus", 2, 1))
                .enqueueMove(CharacterRes.MOVE_SPIRAL)
                .enqueueExpression(CharacterRes.EXPR_UNHAPPY, 1)
                .enqueueExpression(CharacterRes.EXPR_HUNGRY, 4);
        }

        return new CharacterSituation()
            .enqueueTr("2b.Default");
    }

    protected override Graph getGraph() {
        return GameHelper.Instance.getUpgradesManager().graphTimeAttack;
    }

    protected override string getTextBest() {
        return string.Format(Tr.get("Activity2b.Text.Best"), Constants.getDisplayableTimeSec(gameManager.maxTimeAttackTimeSec), Constants.getDisplayableScore(gameManager.maxTimeAttackScore));
    }

	protected override void onButtonClick(MenuButtonBehavior menuButton) {

		if (menuButton == buttonPlay) {

			push(new Activity4c());

		} else if (menuButton == buttonUpgrades) {

			push(new Activity20b());
            /*
		} else if (menuButton == buttonHiScores) {

			push(new Activity50b());
*/
		} else {

			base.onButtonClick(menuButton);
		}
	}

}

