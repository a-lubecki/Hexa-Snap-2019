/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public class BonusCommandProgression : BaseBonusCommand {

	public override void onItemBonusUsed(ItemBonus item) {

        Activity10a activity = (item.activity as Activity10a);

        //pause the game during the speech
        activity.timeManager.pause(this);

        //avoid showing the character speech too much often
        if (Constants.newRandomInt(0, 30) != 42) {

            //call on next frame to let the item destroying and avoid a crash when it is destroyed again when it is part of the farthest items (level up behavior destroys farthest items)
            Async.call(0, () => {
                showNextLevel(item);
            });
            return;
        }

        GameHelper.Instance.getCharacterAnimator()
                  .show(activity, true)
                  .enqueue(getBonusSituation())
                  .enqueueEvent(true, () => {

                      //take the time to hide the character before showing the next level
                      Async.call(1, () => {

                            showNextLevel(item);
                      });

                  })
                  .enqueueHide();

	}

    private void showNextLevel(ItemBonus item) {

        Activity10a activity = (item.activity as Activity10a);

        activity.timeManager.resume(this);
        activity.triggerNextLevel();
    }

    private CharacterSituation getBonusSituation() {

        string[] exprs = {
            CharacterRes.EXPR_SAD,
            CharacterRes.EXPR_UNHAPPY,
            CharacterRes.EXPR_KNOCKED_OUT,
            CharacterRes.EXPR_MEME_ME_GUSTA,
            CharacterRes.EXPR_MEME_POKERFACE,
            CharacterRes.EXPR_MEME_LOL,
            CharacterRes.EXPR_EMOJI_POOP,
            CharacterRes.EXPR_EMOJI_FEAR
        };

        return new CharacterSituation()
            .enqueueTrRandom("10a.Progression", 2)
            .enqueueExpression(exprs[Constants.newRandomPosInArray(exprs.Length)], 4);
    }

}

