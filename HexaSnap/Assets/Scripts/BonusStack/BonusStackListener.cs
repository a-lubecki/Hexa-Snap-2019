/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public interface BonusStackListener : BaseModelListener {

    void onBonusStackEnableChange(BonusStack bonusStack);

    void onBonusStackSizeChange(BonusStack bonusStack, int previousStackSize);

	void onBonusStackItemBonusAdd(BonusStack bonusStack, ItemBonus itemBonus);

	void onBonusStackItemBonusRemove(BonusStack bonusStack, ItemBonus itemBonus);

	void onBonusStackClear(BonusStack bonusStack);

}

