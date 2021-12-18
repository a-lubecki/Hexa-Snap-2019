/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;

public class MenuButtonText : BaseMenuButton {


	public static MenuButtonText newButtonDefault(string title, string textFg) {
		return new MenuButtonText(
			title,
			Constants.PATH_DESIGNS_MENUS + "MenuButton.Background",
			Constants.COLOR_MENU_BUTTON_FG,
			textFg
		);
	}


	public string textFg { get; private set; }

	public MenuButtonText(string title, string spriteBgPath, Color colorFg, string textFg) : base(title, spriteBgPath, colorFg) {

		if (string.IsNullOrEmpty(textFg)) {
			throw new ArgumentException();
		}

		this.textFg = textFg;

	}

	public void changeIconText(string text) {

		if (string.IsNullOrEmpty(text)) {
			throw new ArgumentException();
		}

		this.textFg = text;

		notifyListeners(listener => {
			to(listener).onMenuButtonForegroundChange(this);
		});
	}
}

