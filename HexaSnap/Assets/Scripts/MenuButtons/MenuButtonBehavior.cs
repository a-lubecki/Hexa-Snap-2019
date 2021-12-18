/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MenuButtonBehavior : BaseModelBehavior, MenuButtonListener {


    public BaseMenuButton menuButton {
		get {
			return (BaseMenuButton) model;
		}
	}


    public MenuButtonClickListener clickListener;

    private Transform trButton;
    private Button button;

    private Animation animationButton;

    private Image imageBg;
	private Image imageFg;
	private Text textFg;

    private Text textTitle;

    private BadgeBehavior badge;


	protected override void onAwake() {
		base.onAwake();

        trButton = transform.Find("Button");
        button = trButton.GetComponent<Button>();
        button.onClick.AddListener(onButtonClick);

        animationButton = trButton.GetComponent<Animation>();

        imageBg = trButton.GetComponent<Image>();
        imageFg = trButton.Find("ImageForeground").GetComponent<Image>();
        textFg = trButton.Find("TextForeground").GetComponent<Text>();

        textTitle = transform.Find("TextTitle").GetComponent<Text>();

        badge = trButton.Find("Badge").GetComponent<BadgeBehavior>();
	}

    protected override void onInit() {
        base.onInit();
    
		imageBg.sprite = menuButton.spriteBg;
		imageBg.SetNativeSize();

		if (model is MenuButtonSmall) {

			imageFg.gameObject.SetActive(false);
			textFg.gameObject.SetActive(false);

		} else if (model is MenuButtonIcon) {

			imageFg.gameObject.SetActive(true);
			textFg.gameObject.SetActive(false);

			imageFg.sprite = (model as MenuButtonIcon).spriteFg;
            imageFg.SetNativeSize();

        } else {
			
			imageFg.gameObject.SetActive(false);
			textFg.gameObject.SetActive(true);

			textFg.text = (model as MenuButtonText).textFg;
		}

		textTitle.text = menuButton.title;
		textTitle.color = menuButton.colorFg;

		textFg.color = menuButton.colorFg;

		updateVisibility();
		updateHighlight();

        badge.setText(null);
	}

    protected override void onUpdate() {
		base.onUpdate();

		if (menuButton.isHighlighted) {
			updateHighlight();
		}
	}

    private void updateVisibility() {

        bool isVisible = menuButton.isVisible;

        imageBg.enabled = isVisible;
        imageFg.enabled = isVisible;
        textFg.enabled = isVisible;
        textTitle.enabled = isVisible;

        bool isEnabled = menuButton.isEnabled;
        float alpha = isEnabled ? 1 : 0.3f;

        imageBg.color = getColorAlpha(imageBg.color, alpha);
        imageFg.color = getColorAlpha(imageFg.color, alpha);
        textFg.color = getColorAlpha(textFg.color, alpha);
        textTitle.color = getColorAlpha(textTitle.color, alpha);

        button.interactable = isEnabled;

        updateClickableAnimation();
    }

    private Color getColorAlpha(Color c, float alpha) {
        return new Color(c.r, c.g, c.b, alpha);
    }

	private void updateHighlight() {

        float alpha = Constants.getHighlightedAlpha(menuButton.isHighlighted);

		Color colorBg = imageBg.color;
		if (alpha != colorBg.a) {
			imageBg.color = new Color(colorBg.r, colorBg.g, colorBg.b, alpha);
		}

		Color colorFg = imageFg.color;
		if (alpha != colorFg.a) {
			imageFg.color = new Color(colorFg.r, colorFg.g, colorFg.b, alpha);
		}
	}

    protected void onButtonClick() {

		menuButton.onButtonClick();

        clickListener.onMenuButtonClick(this);

        GameHelper.Instance.getAudioManager().playSound("Button.Clic");
	}


    void MenuButtonListener.onMenuButtonEnableChange(BaseMenuButton menuButton) {

        updateVisibility();
    }

    void MenuButtonListener.onMenuButtonVisibilityChange(BaseMenuButton menuButton) {

        updateVisibility();
    }

    void MenuButtonListener.onMenuButtonHighlightChange(BaseMenuButton menuButton) {

        updateHighlight();
    }

    void MenuButtonListener.onMenuButtonForegroundChange(BaseMenuButton menuButton) {

		if (model is MenuButtonIcon) {

            imageFg.sprite = (menuButton as MenuButtonIcon).spriteFg;
            imageFg.SetNativeSize();

		} else {

			textFg.text = (menuButton as MenuButtonText).textFg;
			textFg.color = menuButton.colorFg;
		}

		textTitle.color = menuButton.colorFg;
	}

    void MenuButtonListener.onMenuButtonTitleChange(BaseMenuButton menuButton) {

        textTitle.text = menuButton.title;
    }

    void MenuButtonListener.onMenuButtonSpriteBgChange(BaseMenuButton menuButton) {

        imageBg.sprite = menuButton.spriteBg;
    }

	void MenuButtonListener.onBump(BaseMenuButton menuButton) {

        Constants.playAnimation(animationButton, "MenuButton.Bump", false);

        Async.call(animationButton.GetClip("MenuButton.Bump").length, () => {
            
            //update anim because it was cleared by the bump
            updateClickableAnimation();
        });
	}

    private void updateClickableAnimation() {

        if (menuButton == null) {
            return;
        }

        //notify the user that the button is clickable with an infinit anim
        if (menuButton.isVisible && menuButton.isEnabled) {
            Constants.playAnimation(animationButton, "MenuButton.Clickable", false);
        } else {
            animationButton.Stop("MenuButton.Clickable");
        }
    }

    public void setBadgeValue(int value) {
        badge.setValue(value);
    }

    public void setBadgeText(string text) {
        badge.setText(text);
    }
}


public interface MenuButtonClickListener {

    void onMenuButtonClick(MenuButtonBehavior menuButtonBehavior);

}
