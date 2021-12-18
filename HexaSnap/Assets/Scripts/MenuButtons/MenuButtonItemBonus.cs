/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;

public class MenuButtonItemBonus : MenuButtonIcon {


	public ItemBonus itemBonus { get; private set; }


	public MenuButtonItemBonus(BonusManager bonusManager, ItemBonus itemBonus) 
        : base(
            "", 
            getBackgroundSpriteHexagon(itemBonus),
            Constants.COLOR_TITLE_NEGATIVE, 
            bonusManager.getSpriteItemBonus(itemBonus.getTag())
        ) {

		this.itemBonus = itemBonus;

	}

    private static Sprite getBackgroundSpriteHexagon(ItemBonus itemBonus) {

        if (itemBonus.bonusType.isMalus) {
            return ItemBonusBehavior.spriteBgHexagonMalus;
        }

        return ItemBonusBehavior.spriteBgHexagonBonus;
    }

}
