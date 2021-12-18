/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public interface ItemBonusListener : ItemListener {

    void onBonusTypeChange(ItemBonus itemBonus);

    void onEnqueuedItemBonusClick(ItemBonus itemBonus);

	void onStackedItemBonusClick(ItemBonus itemBonus);

	void onItemBonusInstantSelect(ItemBonus itemBonus);

	void onItemBonusPersistentSelect(ItemBonus itemBonus);

	void onTimerMalusTriggerStart(ItemBonus itemBonus);

	void onTimerMalusTriggerProgress(ItemBonus itemBonus);

	void onTimerMalusTriggerFinish(ItemBonus itemBonus);

	void onTimerMalusTriggerCancel(ItemBonus itemBonus);

}

