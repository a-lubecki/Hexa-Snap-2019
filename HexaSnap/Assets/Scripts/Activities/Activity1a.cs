/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;
using UnityEngine.UI;


public class Activity1a : Activity1 {


    private MenuButtonBehavior buttonStartTimeAttack;
    private MenuButtonBehavior buttonTimeAttackDisabled;
    private MenuButtonBehavior buttonShop;
    private MenuButtonBehavior buttonAbout;

#if DEBUG
    private MenuButtonBehavior buttonAdd10Hexacoins;
    private MenuButtonBehavior buttonAdd100Hexacoins;
    private MenuButtonBehavior buttonFinishOnboarding;
#endif


    protected override string[] getPrefabNamesToLoad() {
		return new string[] { "Activity1a" };
    }

    protected override Line newPushLine(BaseActivity next) {
		
        if (next is Activity2 || next is Activity23) {
			
            return new Line(
                clickedMenuButton.transform.position,
                next.markerRef.posSafeAreaBottomLeft,
                SegmentThickness.LARGE,
                1,
                true
            );
        }

		return base.newPushLine(next);
    }

	protected override BaseMenuButton newMenuButtonStartArcade() {

		return MenuButtonText.newButtonDefault(
			"",
			"1"
		);
	}

    protected override bool hasDefaultHexacoinWallet() {
        return true;
    }

    protected override CharacterSituation getFirstCharacterSituation(bool isFirstResume) {

        //when the player has unlocked the hardcore mode, display specific speech once only
        if (gameManager.isArcadeHarcoreModeUnlocked() && 
            !GameHelper.Instance.getUniqueDisplaySpeechesManager().hasTag("1a.Hardcore")) {

            return new CharacterSituation()
                .enqueue(Tr.arr("1a.Hardcore", 0, 3))
                .enqueueMove(CharacterRes.MOVE_BOUNCE)
                .enqueueDelayMove(5)
                .enqueueMove(CharacterRes.MOVE_JUMP)
                .enqueueExpression(CharacterRes.EXPR_AMAZED, 3)
                .enqueueDelayExpression(2)
                .enqueueExpression(CharacterRes.EXPR_BLINK, 0.25f)
                .enqueueJoin()
                .enqueueExpression(CharacterRes.EXPR_EMOJI_DEVIL, 3)
                .enqueueMove(CharacterRes.MOVE_SHIVER)
                .enqueue(Tr.get("1a.Hardcore", 3))
                .enqueueUniqueDisplay("1a.Hardcore")
                .enqueueHide();
        }

        if (isFirstResume) {

            DateTime lastStart = Prop.lastStart.get();

            if (DateTime.Now > lastStart.Add(TimeSpan.FromDays(3))) {

                //didn't open the game for 3 days
                return new CharacterSituation()
                    .enqueueTr("1a.ComeBack3")
                    .enqueueMove(CharacterRes.MOVE_SPIRAL)
                    .enqueueExpression(CharacterRes.EXPR_CUTE, 2)
                    .enqueueDelayExpression(1)
                    .enqueueExpression(CharacterRes.EXPR_SAD, 5)
                    .enqueueExpression(CharacterRes.EXPR_EYES_CLOSED, 0.5f);
            }

            if (DateTime.Now > lastStart.Add(TimeSpan.FromDays(10))) {

                //didn't open the game for 10 days
                return new CharacterSituation()
                    .enqueueExpression(CharacterRes.EXPR_CUTE, 2)
                    .enqueueMove(CharacterRes.MOVE_SPIRAL)
                    .enqueueDelayMove(2)
                    .enqueueMove(CharacterRes.MOVE_STRETCH)
                    .enqueueDelayExpression(1)
                    .enqueueExpression(CharacterRes.EXPR_SAD, 5)
                    .enqueueExpression(CharacterRes.EXPR_EYES_CLOSED, 0.5f)
                    .enqueueDelayExpression(1)
                    .enqueueExpression(CharacterRes.EXPR_BLINK, 0.5f)
                    .enqueueTr("1a.ComeBack10");
            }

            return new CharacterSituation()
                .enqueueMove(CharacterRes.MOVE_BOUNCE)
                .enqueueExpression(CharacterRes.EXPR_CUTE, 2)
                .enqueueTr("1a.StartUp");
            
        } else {

            //show a speech
            if (gameManager.isTimeAttackModeAvailable &&
                gameManager.maxTimeAttackTimeSec <= 0 &&
                !(nextActivity is Activity2b)) {

                return new CharacterSituation()
                    .enqueueTr("1a.TimeAttackUnlocked")
                    .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_MEME_NOTICE_ME_SENPAI, 2))
                    .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_SMILE, 2))
                    .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_BLINK, 0.5f))
                    .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_SMILE, 3))
                    .enqueueDelay(CharacterTimeline.MOVE, 3)
                    .enqueue(new CharacterAnimationMove(CharacterRes.MOVE_SPIRAL))
                    .enqueueHide();
            } 

            if (ShopItem.getNbFreeAvailableItems() > 0 &&
                !(nextActivity is Activity23)) {

                return new CharacterSituation()
                    .enqueueTr("1a.FreeHexacoins")
                    .enqueueHide();
            }
        }
       
        return null;
    }

    protected override void onCreate() {
        base.onCreate();

        buttonStartTimeAttack = createButtonGameObject(
            this,
            "PosStartTimeAttack",
            MenuButtonIcon.newButtonDefault(
                Tr.get("Activity1a.Button.StartTimeAttack"),
                "MenuButton.TimeAttack"
            )
        );

        //if the time attack mode is disabled, add an event to show why it is
        buttonTimeAttackDisabled = createButtonGameObject(
            this,
            "PosStartTimeAttack",
            new MenuButtonIcon(
                null,
                (Sprite) null,
                new Color(0, 0, 0, 0),
                null
            )
        );

        buttonShop = createButtonGameObject(
            this,
            "PosShop",
            MenuButtonIcon.newButtonDefault(
                Tr.get("Activity1a.Button.Shop"),
                "MenuButton.Shop"
            )
        );

        buttonAbout = createButtonGameObject(
            this,
            "PosAbout",
            MenuButtonIcon.newButtonDefault(
                Tr.get("Activity1a.Button.About"),
                "MenuButton.About"
            )
        );

        Transform trCopyright = getFirstLoadedGameObject()
            .transform
            .Find("TextCopyright");

        trCopyright.position = Constants.newVector3(markerRef.posBottom, 0, 2.5f, 0);

        trCopyright.GetComponent<Text>().text = "Hexa Snap © Aurélien Lubecki 2019 - All Rights Reserved";        

#if DEBUG

        trCopyright.GetComponent<Text>().text = "Size : " + Screen.width + " x " + Screen.height + " / DPI : " + Screen.dpi + " / Width " + (Screen.width / Screen.dpi) + "\"";

        //add a button for debugging hexacoins
        buttonAdd10Hexacoins = createButtonGameObject(
            this,
            getFirstLoadedGameObject().transform,
            Constants.newVector3(markerRef.posTop, -2, -1, 0),
            MenuButtonSmall.newButtonDefault(
                "MenuButton.Plus"
            )
        );

        buttonAdd100Hexacoins = createButtonGameObject(
            this,
            getFirstLoadedGameObject().transform,
            Constants.newVector3(markerRef.posTop, 0, -1, 0),
            MenuButtonSmall.newButtonDefault(
                "MenuButton.PlusPlus"
            )
        );

        buttonFinishOnboarding = createButtonGameObject(
            this,
            getFirstLoadedGameObject().transform,
            Constants.newVector3(markerRef.posTop, 2, -1, 0),
            MenuButtonSmall.newButtonDefault(
                "MenuButton.MinusMinus"
            )
        );
#endif

    }

    protected override void onPreResume() {
        base.onPreResume();

        BaseMenuButton b = buttonStartArcade.menuButton;

        var maxArcadeLevel = gameManager.maxArcadeLevel;
        if (maxArcadeLevel > 0) {
            (b as MenuButtonText).changeIconText(maxArcadeLevel.ToString());
        }

        if (!gameManager.isArcadeHarcoreModeUnlocked()) {

            b.changeTitle(Tr.get("Activity1a.Button.StartArcade"));

            b.changeColorFg(Constants.COLOR_SUBTITLE);

        } else {

            b.changeTitle(Tr.get("Activity1a.Button.StartHardcore"));

            b.changeColorFg(Constants.COLOR_ARCADE_HARDCORE);
        }

        //add badge on shop for advanced players
        if (gameManager.maxArcadeLevel >= 3) {
            buttonShop.setBadgeValue(ShopItem.getNbFreeAvailableItems());
        } else {
            buttonShop.setBadgeText(null);
        }

        //the time attack mode is only available when level 10 is finished in arcade mode
        bool isTimeAttackModeAvailable = gameManager.isTimeAttackModeAvailable;
        buttonStartTimeAttack.menuButton.setEnabled(isTimeAttackModeAvailable);
        buttonTimeAttackDisabled.menuButton.setVisible(!isTimeAttackModeAvailable);

        //incitate the player to play the time attack mode
        bool isTimeAttackNew = (isTimeAttackModeAvailable && gameManager.maxTimeAttackTimeSec <= 0);
        buttonStartTimeAttack.setBadgeText(isTimeAttackNew ? "!" : null);

        //set button invisible but clickable
        foreach (var image in buttonTimeAttackDisabled.gameObject.GetComponentsInChildren<Image>()) {
            image.color = new Color(1, 1, 1, 0);
        }
    }

    protected override void onButtonStartArcadeClick() {

		push(new Activity2a());
	}

    protected override void onButtonClick(MenuButtonBehavior menuButton) {

#if DEBUG
        if (menuButton == buttonAdd10Hexacoins) {

            gameManager.addHexacoins(10, getActivityName(), "test");

        } else if (menuButton == buttonAdd100Hexacoins) {

            gameManager.addHexacoins(100, getActivityName(), "test");
        
        } else if (menuButton == buttonFinishOnboarding) {

            gameManager.setOnboardingAsPassed(true);
            gameManager.updateMaxArcadeLevel(5);//to show bonus items
            SaveManager.Instance.saveBestScores();
        }
#endif

        if (menuButton == buttonStartTimeAttack) {

            push(new Activity2b());
        
        } else if (menuButton == buttonTimeAttackDisabled) {

            //show the text once to avoid click spam
            buttonTimeAttackDisabled.menuButton.setVisible(false);

            //if the time attack mode is disabled, add an event to show why it is
            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("1a.TimeAttackLocked")
                      .enqueueHide();
            
        } else if (menuButton == buttonShop) {

            push(new Activity23());

        } else if (menuButton == buttonAbout) {

            showAboutPopup();

		} else {

			base.onButtonClick(menuButton);
		}
    }

    private void showAboutPopup() {

        TrackingManager.instance.trackEvent(T.Event.SHOW_POPUP_ABOUT);

        GameHelper.Instance.getNativePopupManager().show(
            Tr.get("P2.Title"),
            null,
            Tr.get("P2.Cancel"),
            null,
            new string[] {
                Tr.get("P2.Share"),
                string.Format(Tr.get("P2.5Stars"), SpecificDeviceManager.Instance.getSpecificStoreText()),
                //Tr.get("P2.Facebook"), //TODO disabled for launch
                Tr.get("P2.More")
            },
            new Action[] {
                () => ShareManager.instance.retrieveShareUrl((sharUrl) => {

                    TrackingManager.instance.trackEvent(T.Event.REDIRECT_SHARE);

                    NativeCallsManager.share(
                        Tr.get("S2.ChooserTitle"),
                        Tr.get("S.Subject"),
                        Tr.get("S2.Message"),
                        sharUrl,
                        null
                    );
                }),
                () => {

                    TrackingManager.instance.trackEvent(T.Event.REDIRECT_5STARS);

                    Application.OpenURL(SpecificDeviceManager.Instance.getUrlStoreHexaSnap());
                },
                /*() => { //TODO disabled for launch

                    TrackingManager.instance.trackEvent(T.Event.REDIRECT_FACEBOOK);

                    Application.OpenURL(Constants.URL_FACEBOOK_HEXASNAP);
                },*/
                () => {

                    TrackingManager.instance.trackEvent(T.Event.REDIRECT_MORE);

                    Application.OpenURL(Constants.URL_SITE_HEXASNAP_ABOUT);
                }
            }
        );
    }

}

