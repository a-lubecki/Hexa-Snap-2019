/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;


public class MenuButtonIconToggle : MenuButtonIcon {


	public static MenuButtonIconToggle newButtonToggleDefault(string title, string spriteFgNameActivated, string spriteFgNameDeactivated, bool isToggleActivated) {
		
        return new MenuButtonIconToggle(
			title, 
			Constants.PATH_DESIGNS_MENUS + "MenuButton.Background",
			Constants.COLOR_MENU_BUTTON_FG,
			Constants.PATH_DESIGNS_MENUS + spriteFgNameActivated,
			Constants.PATH_DESIGNS_MENUS + spriteFgNameDeactivated,
			isToggleActivated
		);
	}


	public Sprite spriteFgActivated { get; private set; }
	public Sprite spriteFgDeactivated { get; private set; }

	public bool isToggleActivated { get; private set; }


	public MenuButtonIconToggle(string title, string spriteBgPath, Color colorFg, string spriteFgPathActivated, string spriteFgPathDeactivated, bool isToggleActivated) : base(title, spriteBgPath, colorFg, isToggleActivated ? spriteFgPathActivated : spriteFgPathDeactivated) {

		if (spriteFg == null) {
			throw new ArgumentException();
		}

		this.spriteFgActivated = GameHelper.Instance.loadSpriteAsset(spriteFgPathActivated);
		this.spriteFgDeactivated = GameHelper.Instance.loadSpriteAsset(spriteFgPathDeactivated);

		this.isToggleActivated = isToggleActivated;
	}

	public void setToggleActivated(bool activated) {

		if (isToggleActivated == activated) {
			//no changes
			return;
		}

		isToggleActivated = activated;

		if (activated) {
			changeIconSprite(spriteFgActivated);
		} else {
			changeIconSprite(spriteFgDeactivated);
		}
	}

	public override void onButtonClick() {

		setToggleActivated(!isToggleActivated);

		base.onButtonClick();
	}

}

