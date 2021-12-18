/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;

public class MenuButtonSmall : BaseMenuButton {
		

	public static MenuButtonSmall newButtonDefault(string spriteBgName) {
		return new MenuButtonSmall(
			Constants.PATH_DESIGNS_MENUS + spriteBgName,
			Constants.COLOR_MENU_BUTTON_FG
		);
	}


	public MenuButtonSmall(string spriteBgPath, Color colorFg) : base(null, spriteBgPath, colorFg) {

	}

    public void changeSpriteBg(string spriteBgName) {
        base.changeSpriteBg(GameHelper.Instance.loadSpriteAsset(Constants.PATH_DESIGNS_MENUS + spriteBgName));
    }

}

