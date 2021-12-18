/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

public interface MenuButtonListener : BaseModelListener {
    
    void onMenuButtonEnableChange(BaseMenuButton menuButton);

    void onMenuButtonVisibilityChange(BaseMenuButton menuButton);

    void onMenuButtonHighlightChange(BaseMenuButton menuButton);

    void onMenuButtonForegroundChange(BaseMenuButton menuButton);

	void onMenuButtonTitleChange(BaseMenuButton menuButton);

    void onMenuButtonSpriteBgChange(BaseMenuButton menuButton);

	void onBump(BaseMenuButton menuButton);
}

