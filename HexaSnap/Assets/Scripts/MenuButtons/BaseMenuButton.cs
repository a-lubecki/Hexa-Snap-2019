/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;

public abstract class BaseMenuButton : BaseModel {

	protected static MenuButtonListener to(BaseModelListener listener) {
		return (MenuButtonListener) listener;
	}


	public string title { get; private set; }
	public Sprite spriteBg { get; private set; }
	public Color colorFg { get; private set; }

    public bool isEnabled { get; private set; }
    public bool isVisible { get; private set; }
    public bool isHighlighted { get; private set; }


    public BaseMenuButton(string title, string spriteBgPath, Color colorFg) : this(title, GameHelper.Instance.loadSpriteAsset(spriteBgPath), colorFg) {

        isEnabled = true;
        isVisible = true;
        isHighlighted = false;
    }

	public BaseMenuButton(string title, Sprite spriteBg, Color colorFg) : base() {

		this.title = title;
		this.spriteBg = spriteBg;
		this.colorFg = colorFg;

        isEnabled = true;
        isVisible = true;
        isHighlighted = false;
    }

    public void changeTitle(string title) {

        this.title = title;

        notifyListeners(listener => {
            to(listener).onMenuButtonTitleChange(this);
        });
    }

    public void changeSpriteBg(Sprite spriteBg) {

        this.spriteBg = spriteBg;

        notifyListeners(listener => {
            to(listener).onMenuButtonSpriteBgChange(this);
        });
    }

	public void changeColorFg(Color colorFg) {

		this.colorFg = colorFg;

		notifyListeners(listener => {
			to(listener).onMenuButtonForegroundChange(this);
		});
	}

	public virtual void onButtonClick() {

		bump();
	}
    
    public void setEnabled(bool isEnabled) {

        if (isEnabled == this.isEnabled) {
            return;
        }

        this.isEnabled = isEnabled;

        notifyListeners(listener => {
            to(listener).onMenuButtonEnableChange(this);
        });
    }

    public void setVisible(bool isVisible) {
        
        if (isVisible == this.isVisible) {
            return;
        }

        this.isVisible = isVisible;

        notifyListeners(listener => {
            to(listener).onMenuButtonVisibilityChange(this);
        });
    }

    public void setHighlighted(bool isHighlighted) {

        if (isHighlighted == this.isHighlighted) {
            return;
        }

        this.isHighlighted = isHighlighted;

        notifyListeners(listener => {
            to(listener).onMenuButtonHighlightChange(this);
        });
    }

	public void bump() {

		notifyListeners(listener => {
			to(listener).onBump(this);
		});
	}

}
