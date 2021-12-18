/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine.UI;


public class Activity30 : BaseUIActivity {


    private MenuButtonBehavior buttonMusic;
    private MenuButtonBehavior buttonSounds;
    private MenuButtonBehavior buttonControls;
    private MenuButtonBehavior buttonDisableCharacter;
	private MenuButtonBehavior buttonInfo;

	private Text textLogin;

    private MenuButtonBehavior buttonFacebook;
    private MenuButtonBehavior buttonLogout;
    private MenuButtonBehavior buttonDeleteAccount;


    protected override MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager) {
		
		if (getPreviousMarkerRef() == markerManager.markerAMainMenu) {
			return markerManager.markerFOptionsLogin;
		}

		return markerManager.MarkerJOptionsLogin;
	}

	protected override string[] getPrefabNamesToLoad() {
		return new string[] { "Activity30" };
	}

	protected override string getTitleForInit() {
		return Tr.get("Activity30.Title");
	}

    protected override bool hasDefaultHexacoinWallet() {
        return true;
    }

    protected override bool hasAdBanner() {
        //can't show banner if coming from the shop thanks screen (about to login) 
        return !(previousActivity is Activity24);
    }

    protected override CharacterSituation getFirstCharacterSituation(bool isFirstResume) {

        if (!isFirstResume) {
            return null;
        }

        if (previousActivity is Activity11) {

            return new CharacterSituation()
                .enqueueTr("30.InGame");
        }

        if (LoginManager.Instance.isLoggedInFacebook()) {

            if (GameHelper.Instance.getHexacoinsWallet().nbHexacoins <= 0) {

                return new CharacterSituation()
                    .enqueueTr("30.LoggedNoHexacoins");
            }

            return new CharacterSituation()
                .enqueueTr("30.Logged");
        }

        if (GameHelper.Instance.getHexacoinsWallet().nbHexacoins <= 5) {

            return new CharacterSituation()
                .enqueueTr("30.NoHexacoins");
        }

        return new CharacterSituation()
            .enqueueTr("30.NotLogged");
    }

    protected override void onCreate() {
		base.onCreate();

        buttonMusic = createButtonGameObject(
            this,
			"PosMusic",
			MenuButtonIconToggle.newButtonToggleDefault(
				Tr.get("Activity30.Button.Music"),
				"MenuButton.Music.ACTIVATED",
				"MenuButton.Music.DEACTIVATED",
				!gameManager.isMusicOptionDeactivated
			)
		);

        buttonSounds = createButtonGameObject(
            this,
			"PosSounds",
			MenuButtonIconToggle.newButtonToggleDefault(
				Tr.get("Activity30.Button.Sounds"),
				"MenuButton.Sounds.ACTIVATED",
				"MenuButton.Sounds.DEACTIVATED",
				!gameManager.isSoundsOptionDeactivated
			)
		);

        buttonControls = createButtonGameObject(
            this,
            "PosControls",
            MenuButtonIconToggle.newButtonToggleDefault(
                Tr.get("Activity30.Button.Controls"),
                "MenuButton.Controls.HORIZONTAL",
                "MenuButton.Controls.AROUND_AXIS",
                gameManager.isControlsOptionDragHorizontal
            )
        );

        buttonDisableCharacter = createButtonGameObject(
            this,
            "PosDisableCharacter",
            MenuButtonIcon.newButtonDefault(
                Tr.get("Activity30.Button.DisableCharacter"),
                "MenuButton.DisableCharacter"
            )
        );

        buttonInfo = createButtonGameObject(
            this,
			markerRef.transform.Find("Activity30"),
			markerRef.posSafeAreaBottomRight,
			MenuButtonSmall.newButtonDefault(
				"MenuButton.Info"
			)
		);

		textLogin = updateText("TextLogin", null);

        buttonFacebook = createButtonGameObject(
            this,
            "PosFacebook",
            new MenuButtonIcon(
                Tr.get("Activity30.Button.Facebook"),
                Constants.PATH_DESIGNS_MENUS + "MenuButton.Background.Facebook",
                Constants.COLOR_MENU_BUTTON_FG,
                Constants.PATH_DESIGNS_MENUS + "MenuButton.Facebook"
            )
        );

        buttonLogout = createButtonGameObject(
            this,
            "PosLogout",
            new MenuButtonIcon(
                Tr.get("Activity30.Button.Logout"),
                Constants.PATH_DESIGNS_MENUS + "MenuButton.Background.Facebook",
                Constants.COLOR_MENU_BUTTON_HIGHLIGHT,
                Constants.PATH_DESIGNS_MENUS + "MenuButton.Facebook"
            )
        );

        buttonDeleteAccount = createButtonGameObject(
            this,
            "PosDeleteAccount",
            new MenuButtonIcon(
                Tr.get("Activity30.Button.DeleteAccount"),
                Constants.PATH_DESIGNS_MENUS + "MenuButton.Background.Destructive",
                Constants.COLOR_MENU_BUTTON_HIGHLIGHT,
                Constants.PATH_DESIGNS_MENUS + "MenuButton.DeleteAccount"
            )
        );
	}

	protected override void onPreResume() {
		base.onPreResume();

		updateUIFacebook();

        buttonDisableCharacter.menuButton.setEnabled(true);
	}

    private void updateButtonControls() {

        if (gameManager.isControlsOptionDragHorizontal) {
            
        } else {
            
        }
    }

	private void updateUIFacebook() {

		if (LoginManager.Instance.isLoggedInFacebook()) {

			textLogin.text = Tr.get("Activity30.Text.Logged");

            buttonFacebook.menuButton.setVisible(false);
            buttonLogout.menuButton.setVisible(true);
            buttonDeleteAccount.menuButton.setVisible(true);

		} else {

            textLogin.text = Tr.get("Activity30.Text.Login");

            buttonFacebook.menuButton.setVisible(true);
            buttonLogout.menuButton.setVisible(false);
            buttonDeleteAccount.menuButton.setVisible(false);
		}
	}

	protected override void onButtonClick(MenuButtonBehavior menuButton) {

		if (menuButton == buttonMusic) {

            toggleMusic();

		} else if (menuButton == buttonSounds) {

            toggleSounds();

        } else if (menuButton == buttonControls) {

            toggleControls();

        } else if (menuButton == buttonDisableCharacter) {

            toggleCharacter();

		} else if (menuButton == buttonInfo) {

            LegalPopupManager.show();

        } else if (menuButton == buttonFacebook) {

            loginFacebook();

        } else if (menuButton == buttonLogout) {

            logoutFacebook();

        } else if (menuButton == buttonDeleteAccount) {

            deleteAccountFacebook();

		} else {

			base.onButtonClick(menuButton);
		}
	}

    private void toggleMusic() {

        gameManager.setMusicOptionDeactivated(!gameManager.isMusicOptionDeactivated);

        if (gameManager.isMusicOptionDeactivated) {

            GameHelper.Instance.getAudioManager().stopMusic();

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("30.MusicOff")
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_KNOCKED_OUT, 3));
        } else {

            playMusic(null);

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("30.MusicOn")
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_DEFAULT_MEH, 3));
        }
    }

    private void toggleSounds() {

        gameManager.setSoundsOptionDeactivated(!gameManager.isSoundsOptionDeactivated);

        if (gameManager.isSoundsOptionDeactivated) {

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("30.SoundsOff")
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_DETERMINED, 1))
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_DETERMINED_RIGHT, 0.5f))
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_DEFAULT_RIGHT, 0.5f))
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_DETERMINED, 1))
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_DETERMINED_LEFT, 0.5f))
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_DEFAULT_LEFT, 0.5f))
                      .enqueueJoin()
                      .enqueue(new CharacterAnimationMove(CharacterRes.MOVE_JUMP))
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_EMOJI_LAUGH, 2));
        } else {

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("30.SoundsOn")
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_EYES_CLOSED, 3));
        }
    }

    private void toggleControls() {

        gameManager.setControlsOptionDragHorizontal(!gameManager.isControlsOptionDragHorizontal);

        if (gameManager.isControlsOptionDragHorizontal) {

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("30.ControlsHorizontal")
                      .enqueue(new CharacterAnimationMove(CharacterRes.MOVE_JUMP))
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_SMILE_LEFT, 0.7f))
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_SMILE_RIGHT, 0.7f));
            
        } else {

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("30.ControlsAroundAxis")
                      .enqueue(new CharacterAnimationMove(CharacterRes.MOVE_SPIRAL))
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_WORRIED, 2))
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_SURPRISED, 2));
        }
    }

    private void toggleCharacter() {
        
        buttonDisableCharacter.menuButton.setEnabled(false);

        float delayDisableSec = 0;

        if (!GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("30.CharacterFirst")) {

            delayDisableSec = 7.5f;

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("30.CharacterFirst")
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_SAD, 5))
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_UNHAPPY, 2))
                      .enqueueUniqueDisplay("30.CharacterFirst");
            
        } else {

            delayDisableSec = 3.5f;

            //display random expression
            string[] exprs = {
                CharacterRes.EXPR_MEME_TROLLFACE,
                CharacterRes.EXPR_MEME_NOTICE_ME_SENPAI,
                CharacterRes.EXPR_MEME_ME_GUSTA,
                CharacterRes.EXPR_MEME_POKERFACE,
                CharacterRes.EXPR_MEME_LOL,
                CharacterRes.EXPR_EMOJI_POOP,
                CharacterRes.EXPR_EMOJI_DEVIL,
                CharacterRes.EXPR_SUNGLASSES,
                CharacterRes.EXPR_RETARDED
            };

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("30.Character")
                      .enqueue(new CharacterAnimationMove(CharacterRes.MOVE_BOUNCE))
                      .enqueue(new CharacterAnimationExpr(exprs[Constants.newRandomPosInArray(exprs.Length)], 3));
        }

        //reinit the button after delay
        Async.call(delayDisableSec, () => {

            if (!isResumed) {
                //activity is destroyed
                return;
            }

            buttonDisableCharacter.menuButton.setEnabled(true);
        });

        TrackingManager.instance.prepareEvent(T.Event.OPTION_CHARACTER)
                       .add(T.Param.TYPE, T.Value.DEACTIVATED)
                       .track();
    }

    private void loginFacebook() {

        //propose to log in
        BundlePush31 b = new BundlePush31 {
            originActivityName = ((BundlePush30)bundlePush).originActivityName
        };

        push(new Activity31().setBundlePush(b));
    }

    private void logoutFacebook() {

        //propose to log out

        GameHelper.Instance.getNativePopupManager().show(
            Tr.get("P1c.Title"),
            Tr.get("P1c.Message"),
            Tr.get("P1c.No"),
            null,
            Tr.get("P1c.Yes"),
            () => {

                LoginManager.Instance.logoutFromFacebook();

                updateUIFacebook();

                TrackingManager.instance.trackEvent(T.Event.LOGOUT);
            }
        );
    }

    private void deleteAccountFacebook() {

        //makes sure the user really want to delete his account

        GameHelper.Instance.getNativePopupManager().show(
            Tr.get("P1d.Title"),
            Tr.get("P1d.Message"),
            Tr.get("P.Cancel"),
            null,
            Tr.get("P.Continue"),
            () => {

                //block UI to avoid quitting the screen
                gameManager.setUIEventsEnabled(false);

                FirebaseFunctionsManager.instance.deleteAccount(() => {

                    //reactivate clicks
                    gameManager.setUIEventsEnabled(true);

                    LoginManager.Instance.logoutFromFacebook();

                    //reset the remote hexacoins as to avoid bugs by creating a new account (disappearing hexacoins)
                    gameManager.hexacoinsWallet.updateLastRemoteHexacoins(0);

                    updateUIFacebook();

                    TrackingManager.instance.trackEvent(T.Event.DELETE_ACCOUNT); 

                }, (error) => {
                    
                    //reactivate clicks
                    gameManager.setUIEventsEnabled(true);

                    GameHelper.Instance.getNativePopupManager().show(
                        Tr.get("P1e.Title"),
                        Tr.get("P1e.Message"),
                        Tr.get("P.Close"),
                        null
                    );
                });
            }
        );
    }

}

public class BundlePush30 : BaseBundle {

    public string originActivityName;

}
