/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;

public class BonusCommandLimitation : BaseBonusCommand {

	public override void onItemBonusUsed(ItemBonus item) {

        BonusStackBehavior bonusStackBehavior = item.activity.bonusStackBehavior;
        BonusStack bonusStack = bonusStackBehavior.bonusStack;

        //calculate values before decrementing stack size
        int currentNbSlots = bonusStack.stackSize;
        int newSlot = currentNbSlots - 1;

        bonusStack.decrementSize();

        if (currentNbSlots > 0) {

            Vector3 currentStackItemPos = bonusStackBehavior.getItemEndPosition(newSlot);

            playFX("FX.Bonus.Limitation", 0.75f, item, currentStackItemPos);

            GameHelper.Instance.getAudioManager().playSound("Bonus.STACK");
        }

    }

}

