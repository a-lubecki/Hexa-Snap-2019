/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public class BonusCommandRegression : BaseBonusCommand {

    public override void onItemBonusUsed(ItemBonus item) {

        if (!(item.activity is Activity10a)) {
            return;
        }

        Activity10a activity = (item.activity as Activity10a);

        //pause the game during the speech
        activity.timeManager.pause(this);

        GameHelper.Instance.getCharacterAnimator()
                  .show(activity, true)
                  .enqueue(getBonusSituation())
                  .enqueueEvent(true, () => {

                      //take the time to hide the character before showing the next level
                      Async.call(1, () => {

                          activity.timeManager.resume(this);

                          activity.triggerPreviousLevel();
                      });

                  })
                  .enqueueHide();

    }

    private CharacterSituation getBonusSituation() {
        
        return new CharacterSituation()
            .enqueueTrRandom("10a.Regression", 3)
            .enqueueMove(CharacterRes.MOVE_TURN)
            .enqueueExpression(CharacterRes.EXPR_MEME_TROLLFACE, 4);
    }

}

