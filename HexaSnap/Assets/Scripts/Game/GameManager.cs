/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;
using Facebook.Unity;
using Firebase.DynamicLinks;
using Firebase.Crashlytics;


public class GameManager : MonoBehaviour {


	public BonusManager bonusManager { get; private set; }
	public UpgradesManager upgradesManager { get; private set; }

	public MarkerManager markerManager { get; private set; }

	public HexacoinsWallet hexacoinsWallet { get; private set; }

    public PurchasesManager purchasesManager { get; private set; }
    public RewardsManager rewardsManager { get; private set; }

	private WeakReference inGameActivityRef;

	public bool isGamePlaying { get; private set; }

	public bool hasPassedOnboarding { get; private set; }

	public bool isMusicOptionDeactivated { get; private set; }
    public bool isSoundsOptionDeactivated { get; private set; }
    public bool isControlsOptionDragHorizontal { get; private set; }

    public bool hasRemovedAds { get; private set; }

    public int maxArcadeLevel { get; private set; }
    public int maxArcadeScore { get; private set; }
    public int maxTimeAttackScore { get; private set; }
    public float maxTimeAttackTimeSec { get; private set; }

    public bool isTimeAttackModeAvailable { 
        get {
            return maxArcadeLevel > 10;
        }
    }


    void Awake() {

        if (!SpecificDeviceManager.Instance.isMobile()) {
            throw new NotSupportedException("Only Android and iOS are managed");
        }

        //remove the test screen
        Transform trTest = GameHelper.Instance.getCanvasGameObject().transform.Find("MarkersTest");
        if (trTest != null) {
            GameObject.Destroy(trTest.gameObject);
        }

        //init facebook sdk
        if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init(fbInitCallback, fbOnHideUnity);

        } else {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }

        TrackingManager.instance.setEnabled(!Debug.isDebugBuild);
        //Crashlytics.IsCrashlyticsCollectionEnabled = !Debug.isDebugBuild; TODO line commented to simplify debug before release

        //init firebase sdk
        FirebaseInitManager.instance.init(() => {

            DynamicLinks.DynamicLinkReceived += onOpenFromDynamicLink;

            //init user id in tracking on start
            updateUserId();
        });

        GameSaverLocal.instance.loadAllFromFile();
    }

    private void fbInitCallback() {

        if (FB.IsInitialized) {
            // Signal an app activation App Event
            FB.ActivateApp();

        } else {
            
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void fbOnHideUnity(bool isGameShown) {

        if (!isGameShown) {
            // Pause the game - we will need to hide
            Time.timeScale = 0;

        } else {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    void onOpenFromDynamicLink(object sender, ReceivedDynamicLinkEventArgs args) {

        var url = args.ReceivedDynamicLink.Url;

        Debug.Log("Received dynamic link : " + url.OriginalString);

        ShareManager.instance.onOpenFromDynamicLink(url);
    }

    public void updateUserId() {

        //user id can be the firebase one or an anonymous id
        var userId = UserIdManager.Instance.getUserId();

        TrackingManager.instance.updateUserId(userId);

        TrackingManager.instance.setUserProperty(
            T.Property.REFERRER,
            UserIdManager.Instance.getPublicReferrer()
        );

        Crashlytics.SetUserId(userId);
    }

	void Start() {

		bonusManager = new BonusManager();
		upgradesManager = new UpgradesManager();

        markerManager = new MarkerManager();

        purchasesManager = new PurchasesManager();
        rewardsManager = new RewardsManager();

        //deactivate objects during menu navigation
        setInGameObjectsActive(false);

        //hide the character
        GameHelper.Instance.getCharacterAnimator().hide(null, false);

        loadSavedData();


        trackNbHexacoins();
        trackHasRemovedAds();

        //track the number of time the app is opened on start
        TrackingManager.instance.setUserProperty(
            T.Property.TOTAL_OPEN,
            Prop.totalAppOpen.increment()
        );


        #if DEBUG && UNITY_EDITOR

        addHexacoins(HexaSnapEditor.getHexacoinsToAdd(), "test", "test");
		HexaSnapEditor.resetHexacoinsToAdd();

		maxArcadeLevel += HexaSnapEditor.getLevelsToAdd();
		HexaSnapEditor.resetLevelsToAdd();

        SaveManager.Instance.saveBestScores();

		//start the chosen activity
		BaseActivity.startFirstActivity(HexaSnapEditor.newCheckedActivity());

        #else

        if (Debug.isDebugBuild) {
            //start the main menu to test faster
            BaseActivity.startFirstActivity(new Activity1a());
        } else {
            //default behavior : start the splashscreen
            BaseActivity.startFirstActivity(new ActivityS());
        }

        #endif

        //sync the game if didn't sync the game for 3 days
        if (DateTime.Now > Prop.lastSync.get().Add(TimeSpan.FromDays(3))
            //or the user is on iphone, on a fresh install and the credentials are stored in the keychain (not deleted when erasing the app)
            || (!Prop.lastStart.has() && LoginManager.Instance.isLoggedInFacebook())) {

            //sync after game load and when firebase is init
            FirebaseInitManager.instance.init(() => FirebaseFunctionsManager.instance.synchronize(_ => {

                //save sync date to know when to sync again in the future
                Prop.lastSync.putNow();

                //when an advanced player logs in before onboarding, disable the onboarding without tracking the onbording ending
                if (!hasPassedOnboarding && maxArcadeLevel > 1) {
                    setOnboardingAsPassed(false);
                }

            },
            (exception) => {

                Debug.LogWarning("Err sync firebase : " + exception);
            }));
        }
	}

    private void loadSavedData() {
        
        //load props first as it contains some tracking data that can change by loading other data
        var propertiesSaveData = GameSaverLocal.instance.getPropertiesSaveData();
        if (propertiesSaveData != null) {

            PropertyManager.Instance.init(
                propertiesSaveData.getPropertiesBool(),
                propertiesSaveData.getPropertiesInt(),
                propertiesSaveData.getPropertiesDateTime(),
                propertiesSaveData.getPropertiesString()
            );
        }

        var optionsSaveData = GameSaverLocal.instance.getOptionsSaveData();
        if (optionsSaveData != null) {
            
            hasPassedOnboarding = optionsSaveData.hasPassedOnboarding();
            isMusicOptionDeactivated = optionsSaveData.isMusicOptionDeactivated();
            isSoundsOptionDeactivated = optionsSaveData.isSoundsOptionDeactivated();
            isControlsOptionDragHorizontal = optionsSaveData.isControlsOptionDragHorizontal();
            InputsManager.Instance.updateDragControls();
            hasRemovedAds = optionsSaveData.hasRemovedAds();
        }

        var bestScoresSaveData = GameSaverLocal.instance.getBestScoresSaveData();
        if (bestScoresSaveData != null) {

            maxArcadeLevel = bestScoresSaveData.getMaxArcadeLevel();
            maxArcadeScore = bestScoresSaveData.getMaxArcadeScore();
            maxTimeAttackScore = bestScoresSaveData.getMaxTimeAttackScore();
            maxTimeAttackTimeSec = bestScoresSaveData.getTimeAttackTimeSec();
        } else {
            
            maxArcadeLevel = 1;
            maxArcadeScore = 0;
            maxTimeAttackScore = 0;
            maxTimeAttackTimeSec = 0;
        }

        var hexacoinsWalletSaveData = GameSaverLocal.instance.getHexacoinsWalletSaveData();
        if (hexacoinsWalletSaveData != null) {
            
            hexacoinsWallet = new HexacoinsWallet(
                hexacoinsWalletSaveData.getNbHexacoins(),
                hexacoinsWalletSaveData.getLastRemoteNbHexacoins()
            );

        } else {
            
            hexacoinsWallet = new HexacoinsWallet(0, 0);
        }

        var upgradesSaveData = GameSaverLocal.instance.getUpgradesSaveData();
        if (upgradesSaveData != null) {

            initGraphArcade(upgradesSaveData.getGraphArcadeData());
            initGraphTimeAttack(upgradesSaveData.getGraphTimeAttack());

        } else {

            initGraphArcade();
            initGraphTimeAttack();

            //save game to update the unlocked slots of bonus nodes in the save file
            SaveManager.Instance.saveGraphs();
        }

        upgradesManager.graphArcade.updateGraphPercentagesHolder();
        upgradesManager.graphTimeAttack.updateGraphPercentagesHolder();

        var characterSaveData = GameSaverLocal.instance.getCharacterSaveData();
        if (characterSaveData != null) {
            
            GameHelper.Instance.getUniqueDisplaySpeechesManager().addTags(characterSaveData.getUniqueDisplayTags());
        }

        var purchasesSaveData = GameSaverLocal.instance.getShopItemsSaveData();
        if (purchasesSaveData != null) {

            purchasesManager.init(
                purchasesSaveData.getNbPurchasesDone(),
                purchasesSaveData.getPendingPurchases()
            );

            rewardsManager.init(
                purchasesSaveData.getShopItemsUnblockDateTime()
            );
        }
    }

#if UNITY_ANDROID
    void Update() {
        
        //manage physical back button on android
        if (Input.GetKeyUp(KeyCode.Escape)) {

            //pause if currently playing
            getNullableInGameActivity()?.pauseGame();

            //ask if the player wants to quit the game
            GameHelper.Instance.getNativePopupManager().show(
                Tr.get("P7.Title"),
                Tr.get("P7.Message"),
                Tr.get("P7.No"),
                null,
                Tr.get("P7.Yes"),
                Application.Quit
            );
        }
    }
#endif

    void OnApplicationPause(bool pauseStatus) {

        if (!pauseStatus) {
            //clear timeshift on resume to get it again on reward click, in case the player has changed the device datetime
            rewardsManager?.clearTimeshift();
        }

        //pause the game, example during a phone call
        if (pauseStatus && isGamePlaying) {
            //trigger pause menu
            getNullableInGameActivity()?.pauseGame();
        }
    }

    public void initGraph(Graph graph, UpgradeGraphSaveData graphSaveData = null) {

        if (graph == upgradesManager.graphArcade) {
            initGraphArcade(graphSaveData);
            return;
        }
            
        if (graph == upgradesManager.graphTimeAttack) {
            initGraphTimeAttack(graphSaveData);
            return;
        }

        throw new NotSupportedException();
    }

    public void initGraphArcade(UpgradeGraphSaveData graphSaveData = null) {

        upgradesManager.graphArcade.initNodesStates(graphSaveData);

        //init tracking user properties for all nodes
        foreach (var zone in upgradesManager.graphArcade.getNodesZone()) {
            
            trackNodeZoneState(getTrackingPropertyZoneArcade(zone.tag), getTrackingValueZone(zone));

            if (zone.state == NodeZoneState.ACTIVATED || zone.state == NodeZoneState.DEACTIVATED) {
                trackMaxUnlockedNodeZone(T.Property.A_MAX_ZONE, zone.tag);
            }
        }
    }

    public void initGraphTimeAttack(UpgradeGraphSaveData graphSaveData = null) {

        upgradesManager.graphTimeAttack.initNodesStates(graphSaveData);

        //init tracking user properties for all nodes
        foreach (var zone in upgradesManager.graphTimeAttack.getNodesZone()) {

            trackNodeZoneState(getTrackingPropertyZoneTimeAttack(zone.tag), getTrackingValueZone(zone));

            if (zone.state == NodeZoneState.ACTIVATED || zone.state == NodeZoneState.DEACTIVATED) {
                trackMaxUnlockedNodeZone(T.Property.T_MAX_ZONE, zone.tag);
            }
        }
    }

    private string getTrackingPropertyZoneArcade(string zoneTag) {

        if ("1".Equals(zoneTag)) {
            return T.Property.A_ZONE1;
        }

        if ("2".Equals(zoneTag)) {
            return T.Property.A_ZONE2;
        }

        if ("3".Equals(zoneTag)) {
            return T.Property.A_ZONE3;
        }

        if ("4".Equals(zoneTag)) {
            return T.Property.A_ZONE4;
        }

        if ("5".Equals(zoneTag)) {
            return T.Property.A_ZONE5;
        }

        if ("6".Equals(zoneTag)) {
            return T.Property.A_ZONE6;
        }

        return null;
    }

    private string getTrackingPropertyZoneTimeAttack(string zoneTag) {

        if ("1".Equals(zoneTag)) {
            return T.Property.T_ZONE1;
        }

        if ("2".Equals(zoneTag)) {
            return T.Property.T_ZONE2;
        }

        if ("3".Equals(zoneTag)) {
            return T.Property.T_ZONE3;
        }

        if ("4".Equals(zoneTag)) {
            return T.Property.T_ZONE4;
        }

        return null;
    }

    private string getTrackingValueZone(NodeZone zone) {

        switch(zone.state) {
            
            case NodeZoneState.ACTIVATED:
                return T.Value.ZONE_ACTIVATED;

            case NodeZoneState.DEACTIVATED:
                return T.Value.ZONE_DEACTIVATED;
        }

        return T.Value.ZONE_LOCKED;
    }

    public void trackNodeZoneState(string propertyZone, string valueZone) {

        //TODO tracking disabled as firebase only allows 25 properties to be stored

        /*
        if (propertyZone == null) {
            return;
        }

        TrackingManager.instance.setUserProperty(
            propertyZone,
            valueZone
        );*/
    }

    public void trackMaxUnlockedNodeZone(string propertyZone, string zoneTag) {

        int zoneNumber;
        if (!int.TryParse(zoneTag, out zoneNumber)) {
            return;
        }

        //track max node number unlock
        TrackingManager.instance.setUserProperty(
            propertyZone,
            Prop.maxZoneUnlockedArcade.putIfGreater(zoneNumber)
        );
    }

	public void setUIEventsEnabled(bool enabled) {

		//TODO if necessary, put a caller object that retain the call to the disable/enable action

		foreach(MonoBehaviour b in GameHelper.Instance.getEventSystemGameObject().GetComponents<MonoBehaviour>()) {
			b.enabled = enabled;
		}
	}

	public void startPlaying() {
		isGamePlaying = true;
	}

	public void stopPlaying() {
		isGamePlaying = false;
	}

	public void setOnboardingAsPassed(bool mustTrack) {
		
		hasPassedOnboarding = true;

        SaveManager.Instance.saveOptions();

        if (mustTrack) {
            TrackingManager.instance.trackEvent(T.Event.ONBOARDING_END);
        }
	}

    public void setMusicOptionDeactivated(bool deactivated) {

		isMusicOptionDeactivated = deactivated;

        SaveManager.Instance.saveOptions();

        string value = deactivated ? T.Value.DEACTIVATED : T.Value.ACTIVATED;

        TrackingManager.instance.setUserProperty(
            T.Property.OPTION_MUSIC,
            value
        );

        TrackingManager.instance.prepareEvent(T.Event.OPTION_MUSIC)
               .add(T.Param.TYPE, value)
               .track();
	}

    public void setSoundsOptionDeactivated(bool deactivated) {

        isSoundsOptionDeactivated = deactivated;

        SaveManager.Instance.saveOptions();

        string value = deactivated ? T.Value.DEACTIVATED : T.Value.ACTIVATED;

        TrackingManager.instance.setUserProperty(
            T.Property.OPTION_SOUNDS,
            value
        );

        TrackingManager.instance.prepareEvent(T.Event.OPTION_SOUNDS)
               .add(T.Param.TYPE, value)
               .track();
    }

    public void setControlsOptionDragHorizontal(bool isDragHorizontal) {

        isControlsOptionDragHorizontal = isDragHorizontal;

        SaveManager.Instance.saveOptions();

        InputsManager.Instance.updateDragControls();

        string value = isDragHorizontal ? T.Value.CONTROLS_DRAG_HORIZONTAL : T.Value.CONTROLS_DRAG_AROUND_AXIS;

        TrackingManager.instance.setUserProperty(
            T.Property.OPTION_CONTROLS,
            value
        );

        TrackingManager.instance.prepareEvent(T.Event.OPTION_CONTROLS)
               .add(T.Param.TYPE, value)
               .track();
    }

    public void removeAds() {

        if (hasRemovedAds) {
            //already removed
            return;
        }

        hasRemovedAds = true;

        GameHelper.Instance.getAdsManager().hideAdBanner();

        SaveManager.Instance.saveOptions();

        trackHasRemovedAds();
    }

    public void reactivateAds() {

        if (!hasRemovedAds) {
            //not removed yet
            return;
        }

        hasRemovedAds = false;

        GameHelper.Instance.getAdsManager().showAdBanner();

        SaveManager.Instance.saveOptions();

        trackHasRemovedAds();
    }

    private void trackHasRemovedAds() {

        TrackingManager.instance.setUserProperty(
            T.Property.ADS,
            hasRemovedAds ? T.Value.DEACTIVATED : T.Value.ACTIVATED
        );
    }


	public void setInGameActivity(Activity10 inGameActivity) {

		if (inGameActivity == null) {
			inGameActivityRef = null;
		} else {
			inGameActivityRef = new WeakReference(inGameActivity);
		}
	}

	public Activity10 getNullableInGameActivity() {

		if (inGameActivityRef == null) {
			return null;
		}

		object target = inGameActivityRef.Target;
		if (target == null) {
			return null;
		}

		return target as Activity10;
	}

    public void updateMaxArcadeScore(int score) {

        if (score > maxArcadeScore) {

            maxArcadeScore = score;

            TrackingManager.instance.setUserProperty(
                T.Property.A_MAX_SCORE,
                score
            );
        }
    }

    public void updateMaxArcadeLevel(int level) {
        
        if (level > maxArcadeLevel) {

            maxArcadeLevel = level;

            TrackingManager.instance.setUserProperty(
                T.Property.A_MAX_LEVEL,
                level
            );
        }
    }

    public void updateHexacoins(int nb, string trackingTagActivityName = null, string trackingTagReason = null) {

        int diff = nb - hexacoinsWallet.nbHexacoins;
        bool earnMoreHexacoins = (diff > 0);

        hexacoinsWallet.updateHexacoins(nb);
        SaveManager.Instance.saveHexacoinsWallet();

        if (earnMoreHexacoins) {
            GameHelper.Instance.getAudioManager().playSound("Hexacoins.Add");
        } else {
            GameHelper.Instance.getAudioManager().playSound("Hexacoins.Pay");
        }

        trackNbHexacoins();

        if (earnMoreHexacoins && trackingTagActivityName != null && trackingTagReason != null) {
            trackEarnHexacoins(diff, trackingTagActivityName, trackingTagReason);
        }
    }

    public void addHexacoins(int nb, string trackingTagActivityName, string trackingTagReason) {

        if (nb < 0) {
            throw new ArgumentException();
        }
        if (nb <= 0) {
            //ignore
            return;
        }

        hexacoinsWallet.addHexacoins(nb);
        SaveManager.Instance.saveHexacoinsWallet();

        GameHelper.Instance.getAudioManager().playSound("Hexacoins.Add");

        trackNbHexacoins();
        trackEarnHexacoins(nb, trackingTagActivityName, trackingTagReason);
    }

    private void trackEarnHexacoins(int nb, string trackingTagActivityName, string trackingTagReason) {
        
        TrackingManager.instance.setUserProperty(
            T.Property.HEXACOINS_TOTAL_EARNED,
            Prop.totalEarnedHexacoins.add(nb)
        );

        TrackingManager.instance.prepareEvent(T.Event.HEXACOIN_EARN)
                       .add(T.Param.NB_HEXACOINS, nb)
                       .add(T.Param.ORIGIN, trackingTagActivityName)
                       .add(T.Param.REASON, trackingTagReason)
                       .track();
    }

    public void payHexacoins(int nb, string trackingTagActivityName, string trackingTagReason) {

        if (nb < 0) {
            throw new ArgumentException();
        }
        if (nb <= 0) {
            //ignore
            return;
        }

        hexacoinsWallet.payHexacoins(nb);
        SaveManager.Instance.saveHexacoinsWallet();

        GameHelper.Instance.getAudioManager().playSound("Hexacoins.Pay");

        trackNbHexacoins();

        TrackingManager.instance.setUserProperty(
            T.Property.HEXACOINS_TOTAL_PAID,
            Prop.totalPaidHexacoins.add(nb)
        );

        TrackingManager.instance.prepareEvent(T.Event.HEXACOIN_PAY_END)
                       .add(T.Param.NB_HEXACOINS, nb)
                       .add(T.Param.ORIGIN, trackingTagActivityName)
                       .add(T.Param.REASON, trackingTagReason)
                       .track();
    }

    private void trackNbHexacoins() {

        int nb = hexacoinsWallet.nbHexacoins;

        TrackingManager.instance.setUserProperty(
            T.Property.HEXACOINS_NB,
            nb
        );

        TrackingManager.instance.setUserProperty(
            T.Property.HEXACOINS_MAX,
            Prop.nbMaxHexacoins.putIfGreater(nb)
        );
    }


    public void saveFinishedArcadeGame(int score, int level) {

        updateMaxArcadeScore(score);
        updateMaxArcadeLevel(level);

        SaveManager.Instance.saveResultArcade(score, level);
    }

    public void updateMaxTimeAttackScore(int score) {

        if (score > maxTimeAttackScore) {
            
            maxTimeAttackScore = score;

            TrackingManager.instance.setUserProperty(
                T.Property.T_MAX_SCORE,
                score
            );
        }
    }

    public void updateMaxTimeAttackTimeSec(float timeSec) {

        if (timeSec > maxTimeAttackTimeSec) {

            maxTimeAttackTimeSec = timeSec;

            TrackingManager.instance.setUserProperty(
                T.Property.T_MAX_TIME,
                (int) timeSec
            );
        }
    }

    public void saveFinishedTimeAttackGame(int score, float timeSec) {

        updateMaxTimeAttackScore(score);
        updateMaxTimeAttackTimeSec(timeSec);

        SaveManager.Instance.saveResultTimeAttack(score, timeSec);
    }

    public bool isArcadeHarcoreModeUnlocked() {
		return (maxArcadeLevel > Constants.MAX_LEVEL_ARCADE);
	}

    public bool isArcadeHarcoreModeBeaten() {
        return (maxArcadeLevel > Constants.MAX_LEVEL_HARDCORE);
    }
    
    public float getTimeAttackTargetTime() {
        
        if (maxTimeAttackTimeSec < Constants.TARGET_TIME_ATTACK_TIME_S) {
            return Constants.TARGET_TIME_ATTACK_TIME_S;
        }

        return maxTimeAttackTimeSec;
    }

    public void setInGameObjectsActive(bool active) {

        GameHelper h = GameHelper.Instance;

        h.getAxis().gameObject.SetActive(active);
        h.getLimitZone().gameObject.SetActive(active);
        h.getGroundZone().gameObject.SetActive(active);
        h.getItemsGenerator().gameObject.SetActive(active);
        h.getBonusQueue().gameObject.SetActive(active);
        h.getBonusStack().gameObject.SetActive(active);
    }

}

