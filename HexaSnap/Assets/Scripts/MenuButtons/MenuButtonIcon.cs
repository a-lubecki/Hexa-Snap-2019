/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public class MenuButtonIcon : BaseMenuButton {


	public static MenuButtonIcon newButtonDefault(string title, string spriteFgName) {
		return new MenuButtonIcon(
			title, 
			Constants.PATH_DESIGNS_MENUS + "MenuButton.Background",
			Constants.COLOR_MENU_BUTTON_FG,
			Constants.PATH_DESIGNS_MENUS + spriteFgName
		);
	}

	public static MenuButtonIcon newButtonDialog(string title, string spriteFgName) {
		return new MenuButtonIcon(
			title, 
			Constants.PATH_DESIGNS_MENUS + "MenuButton.Background",
			Constants.COLOR_MENU_BUTTON_DIALOG,
			Constants.PATH_DESIGNS_MENUS + spriteFgName
		);
	}

	public static MenuButtonIcon newButtonBack() {
		return newButtonDefault(null, "MenuButton.Back");
	}


	public Sprite spriteFg { get; private set; }

	public MenuButtonIcon(string title, string spriteBgPath, Color colorFg, string spriteFgPath) : this(title, GameHelper.Instance.loadSpriteAsset(spriteBgPath), colorFg, GameHelper.Instance.loadSpriteAsset(spriteFgPath)) {
		
	}

	public MenuButtonIcon(string title, Sprite spriteBg, Color colorFg, Sprite spriteFg) : base(title, spriteBg, colorFg) {
		
        this.spriteFg = spriteFg;
	}

	public void changeIconSprite(Sprite sprite) {
		changeIconSprite(sprite, colorFg);
	}

	public void changeIconSprite(Sprite spriteFg, Color colorFg) {

		this.spriteFg = spriteFg;

		//the color change will trigger the listeners
		changeColorFg(colorFg);
	}

}

