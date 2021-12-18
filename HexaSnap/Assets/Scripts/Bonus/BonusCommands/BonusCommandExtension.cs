/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;

public class BonusCommandExtension : BaseBonusCommand {

    public override void onItemBonusUsed(ItemBonus item) {

        BonusStackBehavior bonusStackBehavior = item.activity.bonusStackBehavior;
        BonusStack bonusStack = bonusStackBehavior.bonusStack;

        //calculate values before incrementing stack size
        int newSlot = bonusStack.stackSize;

        bonusStack.incrementSize();

        if (newSlot < BonusStack.maxStackSize) {

            Vector3 currentStackItemPos = bonusStackBehavior.getItemEndPosition(newSlot);

            playFX("FX.Bonus.Extension", 0.75f, item, currentStackItemPos);

            GameHelper.Instance.getAudioManager().playSound("Bonus.STACK");
        }

    }

}

