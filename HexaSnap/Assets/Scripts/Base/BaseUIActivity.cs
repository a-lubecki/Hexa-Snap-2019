/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public abstract class BaseUIActivity : BaseActivity, MenuButtonClickListener {


    private string title;
    private Text textTitle;

    //can be null
    protected LineDrawer lineDrawerPush;
    protected LineDrawer lineDrawerTitle;

	private Dictionary<string, GameObject> loadedGameObjects = new Dictionary<string, GameObject>();

    public MenuButtonBehavior buttonBack { get; protected set; }
    protected MenuButtonBehavior clickedMenuButton { get; private set; }

    public HexacoinsWalletBehavior hexacoinsWalletBehavior { get; private set; }


	protected abstract string[] getPrefabNamesToLoad();

    protected virtual bool hasDefaultHexacoinWallet() {
        //override if necessary
        return false;
    }

    protected virtual Line newPushLine(BaseActivity next) {
        //override if necessary
        return null;
    }

    protected virtual string getTitleForInit() {
        //override if necessary
        return null;
    }

    protected virtual bool hasAdBanner() {
        //override if necessary
        return true;
    }

    private bool hasBackButton() {
        return (title != null);
    }

    protected Vector3 getBackButtonPos() {
        return markerRef.posSafeAreaBottomLeft;
    }

    public Transform getParentTransform() {
        return markerRef.transform;
    }

    protected GameObject getFirstLoadedGameObject() {

        if (loadedGameObjects.Count <= 0) {
            return null;
        }

        return loadedGameObjects.First().Value;
    }

    protected GameObject getLoadedGameObject(string name) {
        return loadedGameObjects[name];
    }

    public string getActivityName() {
        return GetType().Name;
    }

    protected override sealed void animatePush(BaseActivity next, Action completion) {

        if (!(next is BaseUIActivity)) {
            //not managed
            completion();
            return;
        }

        BaseUIActivity nextUIActivity = next as BaseUIActivity;

        //animate line if any
        Line line = newPushLine(nextUIActivity);
        if (line != null) {

            GameObject goLine = GameHelper.Instance.getPool().pickLineGameObject(line);
            //put in background
            goLine.transform.SetAsFirstSibling();

            nextUIActivity.lineDrawerPush = new LineDrawer(line);
            GameHelper.Instance.getLineDrawersManager().register(nextUIActivity.lineDrawerPush);

            nextUIActivity.lineDrawerPush.drawAnimated(
                Constants.INTERPOLATION_LINE_PUSH,
                InterpolatorCurve.EASE_IN_OUT,
                (_) => {
                    //animate title if any
                    nextUIActivity.animateTitleLine();
                });
        }

        GameHelper.Instance.getMainCameraBehavior().animatePosition(
            new PositionInterpolatorBundle(
                nextUIActivity.markerRef.posCamera,
                Constants.INTERPOLATION_ACTIVITY_PUSH,
                InterpolatorCurve.EASE_IN_OUT
            ), (_) => {

                if (nextUIActivity.buttonBack != null) {
                    nextUIActivity.buttonBack.menuButton.bump();
                }

                completion();
            }
        );

        GameHelper.Instance.getAudioManager().playSound("Activity.Push");
    }

    protected override sealed void animatePopActivity(Action completion) {

        //animate line in reverse
        if (lineDrawerPush != null) {

            lineDrawerPush.eraseAnimated(
                isResumed ? Constants.INTERPOLATION_LINE_POP : 0,
                InterpolatorCurve.EASE_IN_OUT,
                (_) => {

                    GameHelper.Instance.getLineDrawersManager().unregister(lineDrawerPush);
                    GameHelper.Instance.getPool().storeLineGameObject(BaseModelBehavior.findModelBehavior<LineBehavior>(lineDrawerPush.line));

                    //release
                    lineDrawerPush = null;
                }
            );

        }

        if (isResumed) {

            GameHelper.Instance.getMainCameraBehavior().animatePosition(
                new PositionInterpolatorBundle(
                    previousActivity.markerRef.posCamera,
                    Constants.INTERPOLATION_ACTIVITY_POP,
                    InterpolatorCurve.EASE_IN_OUT
                ), (_) => {

                    completion();
                }
            );

            GameHelper.Instance.getAudioManager().playSound("Activity.Pop");

        } else {

            completion();
        }
    }

    protected override sealed void createActivityContent() {
        base.createActivityContent();

        title = getTitleForInit();

		//load the menu elements with the marker as parent
		string[] prefabNames = getPrefabNamesToLoad();
		if (prefabNames != null) {

			foreach (string name in prefabNames) {
				GameObject prefab = GameHelper.Instance.loadPrefabAsset(name);
				GameObject go = GameObject.Instantiate(prefab, markerRef.transform);
				go.name = prefab.name;
                go.transform.localPosition = prefab.transform.position;//Vector3.zero;

				loadedGameObjects.Add(name, go);
			}
		}

        if (!string.IsNullOrEmpty(title)) {

            GameObject prefabTitle = GameHelper.Instance.loadPrefabAsset(Constants.PREFAB_NAME_ACTIVITY_TITLE);

            GameObject goTitle = GameObject.Instantiate(prefabTitle);
            goTitle.transform.SetParent(markerRef.transform);
            goTitle.transform.localScale = Vector3.one;
            goTitle.transform.position = new Vector3(markerRef.posSafeAreaBottomLeft.x + 1.5f, markerRef.posSafeAreaBottomLeft.y, markerRef.posCenter.z);

            textTitle = goTitle.GetComponent<Text>();
            textTitle.text = title;
            textTitle.enabled = false;
        }

        //init the back button if any
        if (hasBackButton()) {
            
            GameObject goButtonBack = GameHelper.Instance.getPool().pickMenuButtonGameObject(
                MenuButtonIcon.newButtonBack(),
                this,
                markerRef.transform,
                false,
                getBackButtonPos()
            );

            goButtonBack.transform.localScale = Vector3.one;

            buttonBack = goButtonBack.GetComponent<MenuButtonBehavior>();
        }

        if (hasDefaultHexacoinWallet()) {

            GameObject goWallet = GameObject.Instantiate(
                GameHelper.Instance.loadPrefabAsset("HexacoinWallet"),
                loadedGameObjects.First().Value.transform
            );

            goWallet.transform.position = markerRef.posSafeAreaTopRight;

            hexacoinsWalletBehavior = goWallet.GetComponent<HexacoinsWalletBehavior>();
            hexacoinsWalletBehavior.init(GameHelper.Instance.getHexacoinsWallet());
        }
    }

    protected override sealed void destroyActivityContent() {
        base.destroyActivityContent();

        //clear line if any
        if (lineDrawerTitle != null) {
            GameHelper.Instance.getLineDrawersManager().unregister(lineDrawerTitle);
            GameHelper.Instance.getPool().storeLineGameObject(BaseModelBehavior.findModelBehavior<LineBehavior>(lineDrawerTitle.line));
            lineDrawerTitle = null;
        }

        if (hexacoinsWalletBehavior != null) {

            hexacoinsWalletBehavior.stopGlittering();
            //free, game object will be destroyed after
            hexacoinsWalletBehavior = null;
        }

        if (buttonBack != null) {

            GameHelper.Instance.getPool().storeMenuButtonGameObject(buttonBack);
            buttonBack = null;
        }

        if (textTitle != null) {
            GameObject.Destroy(textTitle.gameObject);
            textTitle = null;
        }

		//destroy every created object of the current activity in the marker transform
		foreach (GameObject go in loadedGameObjects.Values) {
			GameObject.Destroy(go);
		}

		loadedGameObjects = null;
	}

    protected override sealed void setActivityContentActive(bool active) {
        
        if (hexacoinsWalletBehavior != null) {

            if (active) {
                hexacoinsWalletBehavior.startGlittering();
            } else {
                hexacoinsWalletBehavior.stopGlittering();
            }
        }
    }


    private void animateTitleLine() {

        if (textTitle == null) {
            return;
        }

        textTitle.enabled = true;
        //force the content size fitter to update the text size in order to get the width immediately
        textTitle.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();


        float xEnd = textTitle.transform.position.x + textTitle.GetComponent<RectTransform>().sizeDelta.x * 0.01f;

        Line line = new Line(
            markerRef.posSafeAreaBottomLeft,
            new Vector3(xEnd + 0.2f, markerRef.posSafeAreaBottomLeft.y, markerRef.posSafeAreaBottomLeft.z),
            SegmentThickness.LARGE,
            0,
            false);

        GameObject goLine = GameHelper.Instance.getPool().pickLineGameObject(line);
        //put in background
        goLine.transform.SetAsFirstSibling();

        lineDrawerTitle = new LineDrawer(line);
        GameHelper.Instance.getLineDrawersManager().register(lineDrawerTitle);

        lineDrawerTitle.drawAnimated(0.1f, InterpolatorCurve.EASE_OUT, null);

    }


    void MenuButtonClickListener.onMenuButtonClick(MenuButtonBehavior menuButtonBehavior) {

        clickedMenuButton = menuButtonBehavior;

        if (menuButtonBehavior == buttonBack) {
			onBackButtonClick();
		} else {
            onButtonClick(menuButtonBehavior);
		}
	}

	protected virtual void onBackButtonClick() {
		//override if necessary
		pop();
	}

	protected virtual void onButtonClick(MenuButtonBehavior menuButton) {
		//override this
	}


	protected MarkerBehavior getPreviousMarkerRef() {

		if (previousActivity == null) {
			return null;
		}

		return previousActivity.markerRef;
	}

	protected MarkerBehavior getNextMarkerRef() {

		if (nextActivity == null) {
			return null;
		}

		return nextActivity.markerRef;
	}

	protected Transform findChildTransform(string objectName) {
		
		foreach (GameObject go in loadedGameObjects.Values) {

			Transform objectTransform = go.transform.Find(objectName);

			if (objectTransform != null) {
				return objectTransform;
			}
		}

        Debug.Log("GameObject not found : " + objectName);

        return null;
	}

    protected MenuButtonBehavior createButtonGameObject(MenuButtonClickListener clickListener, string buttonPosName, BaseMenuButton button) {

		Transform buttonPosTransform = findChildTransform(buttonPosName);

		return createButtonGameObject(
            clickListener,
			buttonPosTransform.parent,
			buttonPosTransform.position,
			button
		);
	}

    protected MenuButtonBehavior createButtonGameObject(MenuButtonClickListener clickListener, Transform activityTransform, Vector3 pos, BaseMenuButton button) {

		GameObject go = GameHelper.Instance.getPool().pickMenuButtonGameObject(
            button,
            clickListener,
			activityTransform,
			false,
			pos
		);

		go.transform.localScale = Vector3.one;

        return go.GetComponent<MenuButtonBehavior>();
	}

	protected Text updateText(string textName, string textContent) {

		Text text = findChildTransform(textName).GetComponent<Text>();
		text.text = textContent;

		return text;
	}

    protected override void onPreResume() {
        base.onPreResume();

        //hide the banner before resuming if necessary
        if (!hasAdBanner()) {
            GameHelper.Instance.getAdsManager().hideAdBanner();
        }
    }

    protected override void onResume(bool isFirst) {
        base.onResume(isFirst);

        //show the banner on resume if necessary
        if (hasAdBanner()) {
            GameHelper.Instance.getAdsManager().showAdBanner();
        }

        if (isOverlay()) {
            //character is not managed for overlays
            return;
        }

        //reset to play the next anims on this activity
        CharacterAnimator character = GameHelper.Instance.getCharacterAnimator();

        Vector3 characterPos;

        Transform trCharacter = findChildTransform("PosCharacter");
        if (trCharacter != null) {
            characterPos = trCharacter.position;
        } else {
            characterPos = getCharacterPosInside();
        }

        character.reset(
            this,
            calculateCharacterPosOutside(characterPos),
            characterPos,
            getCharacterBubblePosition(),
            getCharacterTimelineEndPolicy()
        );

        //play situation  
        CharacterSituation situation = getFirstCharacterSituation(isFirst);
        if (situation == null) {
            return;
        }

        Async.call(2, () => {

            if (!isResumed) {
                return;
            }

            if (character.isSpeaking()) {
                //don't stop the current speech
                return;
            }

            character.show(this, true).enqueue(situation);
        });
    }

    protected override void onPause() {
        base.onPause();

        GameHelper.Instance.getCharacterAnimator().hide(this, true);
    }

    protected virtual Vector3 getCharacterPosInside() {
        return markerRef.posCenter;
    }

    private Vector3 calculateCharacterPosOutside(Vector3 posInside) {

        float x = markerRef.posCenter.x;

        if (posInside.x < x) {
            //left of the screen
            x = markerRef.posLeft.x - 5;

        } else {
            //right of the screen
            x = markerRef.posRight.x + 5;
        }

        return new Vector3(x, posInside.y + 2, posInside.z);
    }

    protected virtual CharacterBubblePosition getCharacterBubblePosition() {
        return CharacterBubblePosition.RIGHT;
    }

    protected virtual CharacterTimelineEndPolicy getCharacterTimelineEndPolicy() {
        return CharacterTimelineEndPolicy.HIDE;
    }

    protected virtual CharacterSituation getFirstCharacterSituation(bool isFirstResume) {
        //override this method for facility
        return null;
    }

    protected void playMusic(MusicInfo musicInfo) {
        
        if (musicInfo == null) {

            //play default musics
            if (!gameManager.isArcadeHarcoreModeUnlocked()) {
                musicInfo = Constants.MUSIC_INFO_MENU_DEFAULT;
            } else {
                musicInfo = Constants.MUSIC_INFO_MENU_HARDCORE;
            }
        }

        GameHelper.Instance.getAudioManager().playMusic(musicInfo);
    }

    protected void stopMusic() {

        playMusic(null);
    }

    protected override void trackActivityChange(BaseActivity newSeenActivity, BaseActivity lastSeenActivity) {
        
        TrackingManager.instance.prepareEvent(T.Event.ACTIVITY)
                       .add(T.Param.CURRENT, newSeenActivity.GetType().Name)
                       .add(T.Param.PREVIOUS, lastSeenActivity.GetType().Name)
                       .track();
    }

}

