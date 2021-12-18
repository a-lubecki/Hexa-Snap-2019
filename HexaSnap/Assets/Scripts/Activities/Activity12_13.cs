/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public abstract class Activity12_13 : BaseUIActivity {


	public static readonly int POP_CODE_QUIT = 6845;

    private static readonly string SCREENSHOT_NAME = "hexasnap-share.png";
    private static readonly string SCREENSHOT_PATH = Application.persistentDataPath + "/" + SCREENSHOT_NAME;

    
    private MenuButtonBehavior buttonShare;
	private MenuButtonBehavior buttonQuit;
    private MenuButtonBehavior buttonFinish;
    private MenuButtonBehavior buttonContinue;

    private Image imageBgScore;
    private Image imageScoreSeparator;

    private Text textScore;
    private Text textScoreValue;
    private Text textStats;

    private GameObject goImageLogo;

    protected override MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager) {
		return markerManager.markerDTransition;
	}

    protected abstract bool isGameOverScreen();
    protected abstract bool canContinuePlaying();

    protected abstract string getTextTitle();
    protected abstract string getTextStats();

    protected abstract int getScoreValue();
    protected abstract int getLevelValue();
    protected abstract float getTimeSecValue();

    protected override CharacterBubblePosition getCharacterBubblePosition() {
        return CharacterBubblePosition.TOP;
    }

    protected abstract int getNbHexacoinsToEarn();

    protected abstract CharacterSituation getEndGameCharacterSituation();

    protected abstract IEnumerator showSpecificViews();


    protected override void onCreate() {
		base.onCreate();

        buttonShare = createButtonGameObject(
            this,
            "PosShare",
            new MenuButtonIcon(
                Tr.get("Activity12.Button.Share"),
                Constants.PATH_DESIGNS_MENUS + "MenuButton.Background.Specific",
                Constants.COLOR_MENU_BUTTON_FG,
                Constants.PATH_DESIGNS_MENUS + SpecificDeviceManager.Instance.getButtonShareIcon()
            )
        );

        if (!canContinuePlaying()) {

            buttonFinish = createButtonGameObject(
                this,
                "PosFinish",
                MenuButtonIcon.newButtonDefault(
                    Tr.get("Activity12.Button.Finish"),
                    "MenuButton.Stop"
                )
            );

        } else {

            buttonQuit = createButtonGameObject(
                this,
                "PosQuit",
                MenuButtonIcon.newButtonDefault(
                    Tr.get("Activity13.Button.Quit"),
                    "MenuButton.Stop"
                )
            );

            buttonContinue = createButtonGameObject(
                this,
                "PosContinue",
                MenuButtonIcon.newButtonDefault(
                    Tr.get("Activity13.Button.Continue"),
                    "MenuButton.Play"
                )
            );

        }

        imageBgScore = findChildTransform("ImageBgScore").GetComponent<Image>();
        imageScoreSeparator = findChildTransform("ImageScoreSeparator").GetComponent<Image>();

        updateText("TextTitle", getTextTitle());
        textScore = updateText("TextScore", Tr.get("Activity12.Text.Score"));
        textScoreValue = updateText("TextScoreValue", Constants.getDisplayableScore(getScoreValue()));
		textStats = updateText("TextStats", getTextStats());
        
        //hide all
        textScore.gameObject.SetActive(false);
        textScoreValue.gameObject.SetActive(false);
        textStats.gameObject.SetActive(false);

        imageBgScore.gameObject.SetActive(false);
        imageScoreSeparator.gameObject.SetActive(false);

        buttonShare.menuButton.setVisible(false);

        goImageLogo = findChildTransform("Imagelogo").gameObject;

        buttonQuit?.menuButton.setVisible(false);
        buttonFinish?.menuButton.setVisible(false);
        buttonContinue?.menuButton.setVisible(false);

        //hide logo, only used for snapshot
        goImageLogo.SetActive(false);
    }

    protected override void onResume(bool isFirst) {
        base.onResume(isFirst);

        if (!isFirst) {
            return;
        }

        Async.call(showGameEnd());

        //play a jingle
        if (isGameOverScreen()) {
            GameHelper.Instance.getAudioManager().playSound("GameOver.Failure");
        } else {
            Async.call(2, () => GameHelper.Instance.getAudioManager().playSound("GameOver.Success"));
        }
    }

    /**
     * Begin animating the screen UI
     */
    protected IEnumerator showGameEnd() {
        
        //animate hexacoin if any
        int nbHexacoins = getNbHexacoinsToEarn();

        if (nbHexacoins > 0) {

            yield return new WaitForSeconds(2f);

            Vector3 walletPos = hexacoinsWalletBehavior.transform.position;

            GameObjectPoolBehavior pool = GameHelper.Instance.getPool();
            GameObject go = pool.pickFlyingGameObject(
                false,
                getFirstLoadedGameObject().transform,
                true,
                new Vector3(walletPos.x + 20, walletPos.y - 400, 0)
            );

            go.GetComponent<FlyingScoreBehavior>().startFlying("+" + nbHexacoins, () => {
                pool.storeFlyingGameObject(false, go);
            });

            hexacoinsWalletBehavior.reveal();

            yield return new WaitForSeconds(0.3f);

            //earn hexacoin after delay before displaying the character
            gameManager.addHexacoins(nbHexacoins, getActivityName(), T.Value.EARN_REASON_END_GAME);

            yield return new WaitForSeconds(2.5f);
        }

        //show a speech then show result
        CharacterSituation characterSituation = getEndGameCharacterSituation();

        if (characterSituation == null) {
            //no speech, show the result now
            Async.call(showGameResults());
            yield break;
        }

        yield return new WaitForSeconds(1f);

        GameHelper.Instance.getCharacterAnimator()
              .show(this, true)
              .enqueue(characterSituation)
              .enqueueEvent(true, () => {

                  //trigger the results after the speech
                  Async.call(1.5f, () => {

                      Async.call(showGameResults());
                  });
              })
              .enqueueHide();
    }

    /**
     * End animating the screen UI
     */
    protected IEnumerator showGameResults() {

        yield return showSpecificViews();

        yield return new WaitForSeconds(0.2f);

        imageBgScore.gameObject.SetActive(true);
        textScore.gameObject.SetActive(true);
        textScoreValue.gameObject.SetActive(true);
        imageScoreSeparator.gameObject.SetActive(true);
        buttonShare.menuButton.setVisible(true);

        buttonShare.menuButton.setEnabled(false);

        yield return new WaitForSeconds(0.2f);
        
        textStats.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        buttonQuit?.menuButton.setVisible(true);
        buttonFinish?.menuButton.setVisible(true);
        buttonContinue?.menuButton.setVisible(true);

        buttonShare.menuButton.setEnabled(true);
    }

	protected override void onButtonClick(MenuButtonBehavior menuButton) {

		if (menuButton == buttonShare) {

            share();

        } else if (menuButton == buttonFinish || menuButton == buttonQuit) {

            //return to the menu (Activity3 or Activity2b)
            pop();

		} else if (menuButton == buttonContinue) {

            //return to the game with the current level and score (Activity4a then Activity10a)
            BundlePush4a b = new BundlePush4a {
                hasCustomTitleLine = true,
                mustOverlayGame = false,
                nextLevel = gameManager.maxArcadeLevel,
                savedScore = getScoreValue()
            };

            replaceBy(new Activity4a().setBundlePush(b));

        } else {

			base.onButtonClick(menuButton);
		}
	}

    private void share() {

        //block UI to avoid quitting the screen
        gameManager.setUIEventsEnabled(false);

        //retrieve url with firebase dynamic links then share
        ShareManager.instance.retrieveShareUrl(shareWithUrl);

        //track nb shares
        if (isArcadeMode()) {

            TrackingManager.instance.setUserProperty(
                T.Property.A_NB_SHARE,
                Prop.nbShareArcade.increment()
            );

            TrackingManager.instance.prepareEvent(T.Event.A_SHARE_SCORE)
                           .add(T.Param.LEVEL, getLevelValue())
                           .add(T.Param.SCORE, getScoreValue())
                           .add(T.Param.REFERRER, UserIdManager.Instance.getPublicReferrer())
                           .track();

        } else {

            TrackingManager.instance.setUserProperty(
                T.Property.T_NB_SHARE,
                Prop.nbShareTimeAttack.increment()
            );

            TrackingManager.instance.prepareEvent(T.Event.T_SHARE_SCORE)
                           .add(T.Param.TIME_SEC, (int)getTimeSecValue())
                           .add(T.Param.SCORE, getScoreValue())
                           .add(T.Param.REFERRER, UserIdManager.Instance.getPublicReferrer())
                           .track();
        }
    }

    private void shareWithUrl(string url) {

        //deactivate all
        GameHelper.Instance.getAdsManager().hideAdBanner();

        if (isGameOverScreen() || !isArcadeMode()) {
            updateText("TextTitle", getGameModeText());
        }

        buttonQuit?.menuButton.setVisible(false);
        buttonFinish?.menuButton.setVisible(false);
        buttonContinue?.menuButton.setVisible(false);

        goImageLogo.SetActive(true);

        generateScorePicture(() => {

            //take a screenshot and share it
            NativeCallsManager.share(
                Tr.get("S1.ChooserTitle"),
                Tr.get("S.Subject"),
                getShareMessage(),
                url,
                SCREENSHOT_PATH
            );

            //reactivate clicks after share
            gameManager.setUIEventsEnabled(true);

            updateText("TextTitle", getTextTitle());

            //activate all
            GameHelper.Instance.getAdsManager().showAdBanner();

            buttonQuit?.menuButton.setVisible(true);
            buttonFinish?.menuButton.setVisible(true);
            buttonContinue?.menuButton.setVisible(true);

            goImageLogo.SetActive(false);
        });
    }

    private void generateScorePicture(Action completion) {

        //erase the last screenshot
        try {
            if (File.Exists(SCREENSHOT_PATH)) {
                File.Delete(SCREENSHOT_PATH);
            }
        } catch (Exception e) {
            Debug.LogWarning(e);
        }

        //asynchronous capture
        Async.call(captureScreenshot(completion));
    }

    private IEnumerator captureScreenshot(Action completion) {

        yield return new WaitForEndOfFrame();

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);

        //Get Image from screen
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        //Save image to file
        File.WriteAllBytes(SCREENSHOT_PATH, screenImage.EncodeToPNG());

        completion();
    }

    protected abstract bool isArcadeMode();

    private string getGameModeText() {

        if (isArcadeMode()) {

            if (!gameManager.isArcadeHarcoreModeUnlocked()) {
                return Tr.get("S1.Mode.ARCADE");
            } 

            return Tr.get("S1.Mode.HARDCORE");
        }

        return Tr.get("S1.Mode.TIME_ATTACK");
    }

    private string getShareMessage() {
        return string.Format(Tr.get("S1.Message"), getGameModeText());
    }

    protected Transform updateStars(float durationSec) {

        int nbStars = 5;

        float percentage = durationSec / Constants.TARGET_TIME_ATTACK_TIME_S;
        if (percentage > 1) {
            percentage = 1;
        }

        int reachedStarPos = Mathf.FloorToInt(percentage * nbStars);

        Transform trAdvance = findChildTransform("Advance");

        for (int i = 0 ; i < nbStars ; i++) {

            MaskableGraphic star = trAdvance.Find("Star" + i).GetComponent<MaskableGraphic>();

            Color c = star.color;
            c.a = (i < reachedStarPos) ? 1 : 0.3f;
            star.color = c;
        }

        return trAdvance;
    }

}

