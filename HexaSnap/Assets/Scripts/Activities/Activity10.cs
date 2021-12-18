/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;


public abstract class Activity10 : BaseUIActivity, ItemListener, ItemBonusListener, BonusStackListener, BonusQueueListener, ItemsGeneratorListener {
    

    private static readonly string COROUTINE_TAG_RANDOM_SPEECHES = "randomSpeeches";


    public MenuButtonBehavior buttonPause { get; protected set; }
    public MenuButtonBehavior buttonStackFirstBonus { get; protected set; }

    protected List<Item> items = new List<Item>();

    public BonusManager bonusManager { get; private set; }
    public GraphPercentagesHolder graphPercentagesHolder { get; private set; }

    public TimeManager timeManager { get; private set; }

    public BonusQueueBehavior bonusQueueBehavior { get; private set; }
    public BonusQueue bonusQueue {
        get {
            return bonusQueueBehavior.bonusQueue;
        }
    }

    public BonusStackBehavior bonusStackBehavior { get; private set; }
    public BonusStack bonusStack {
        get {
            return bonusStackBehavior.bonusStack;
        }
    }

    public AxisBehavior axisBehavior { get; private set; }
    public Axis axis {
        get {
            return axisBehavior.axis;
        }
    }

    public ItemsGeneratorBehavior itemsGeneratorBehavior { get; private set; }
    public ItemsGenerator itemsGenerator {
        get {
            return itemsGeneratorBehavior.itemsGenerator;
        }
    }

    public ScoreCounterBehavior scoreCounterBehavior { get; private set; }
    public ScoreCounter scoreCounter {
        get {
            return scoreCounterBehavior.scoreCounter;
        }
    }

    public ScoreMultiplierBehavior scoreMultiplierBehavior { get; private set; }
    public ScoreMultiplier scoreMultiplier {
        get {
            return scoreMultiplierBehavior.scoreMultiplier;
        }
    }

    private Animation animFadeHUD;

    private DateTime lastRandomSpeechTime;

    protected float? startTime { get; private set; }
    protected float totalDuration { get; private set; }

    private OnboardingHandler onboardingHandler;
    public OnboardingControlsIndicatorBehavior onboardingControlsIndicator { get; private set; }


    protected override MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager) {
        return markerManager.markerEPlay;
    }

    protected override string[] getPrefabNamesToLoad() {
        //put the HUD first to see the wallet over the HUD
        return new string[] { getHUDGameObjectName(), "Activity10" };
    }

    protected abstract string getHUDGameObjectName();

    public GameObject getHUD() {
        return getLoadedGameObject(getHUDGameObjectName());
    }

    protected override bool hasDefaultHexacoinWallet() {
        return true;
    }

    protected override bool hasAdBanner() {
        return false;
    }

    protected abstract GraphPercentagesHolder getGraphPercentagesHolder();

    protected override CharacterBubblePosition getCharacterBubblePosition() {
        return CharacterBubblePosition.TOP_RIGHT;
    }

    public abstract int getMaxLevel();

    public abstract int getCurrentLevel();

    public abstract int getCurrentCappedLevel();


    protected override void onCreate() {
        base.onCreate();

        buttonPause = createButtonGameObject(
            this,
            markerRef.transform.Find("Activity10"),
            markerRef.posSafeAreaBottomLeft,
            MenuButtonIcon.newButtonDefault(
                null,
                "MenuButton.Pause"
            )
        );
        buttonPause.menuButton.setEnabled(false);

        buttonStackFirstBonus = createButtonGameObject(
            this,
            markerRef.transform.Find("Activity10"),
            markerRef.posSafeAreaBottomRight,
            MenuButtonIcon.newButtonDefault(
                null,
                "MenuButton.StackBonus"
            )
        );
        buttonStackFirstBonus.menuButton.setEnabled(false);

        //init activity in cam
        gameManager.setInGameActivity(this);
        gameManager.setInGameObjectsActive(true);

        bonusManager = GameHelper.Instance.getBonusManager();
        graphPercentagesHolder = getGraphPercentagesHolder();

        timeManager = new TimeManager(this);

        //init bonus queue first to avoid null pointers during the other inits
        bonusQueueBehavior = GameObject.Find(Constants.GAME_OBJECT_NAME_BONUS_QUEUE).GetComponent<BonusQueueBehavior>();
        bonusQueueBehavior.init(new BonusQueue(this));
        bonusQueue.addListener(this);

        bonusStackBehavior = GameObject.Find(Constants.GAME_OBJECT_NAME_BONUS_STACK).GetComponent<BonusStackBehavior>();
        bonusStackBehavior.init(new BonusStack(this));
        bonusStack.addListener(this);

        axisBehavior = GameHelper.Instance.getAxis().GetComponent<AxisBehavior>();
        axisBehavior.init(new Axis(this));

        itemsGeneratorBehavior = GameObject.Find(Constants.GAME_OBJECT_NAME_ITEMS_GENERATOR).GetComponent<ItemsGeneratorBehavior>();
        itemsGeneratorBehavior.init(new ItemsGenerator(this, getMaxLevel()));
        itemsGenerator.addListener(this);

        findChildTransform("TextScore").GetComponent<Text>().text = Tr.get("Activity10.Text.Score");

        scoreCounterBehavior = findChildTransform("ScoreCounter").GetComponent<ScoreCounterBehavior>();
        scoreCounterBehavior.init(new ScoreCounter(this));

        scoreMultiplierBehavior = findChildTransform("ImageMultiplier").Find("ScoreMultiplier").GetComponent<ScoreMultiplierBehavior>();
        scoreMultiplierBehavior.init(new ScoreMultiplier(this));

        //play anims at the beginning
        Constants.playAnimation(itemsGeneratorBehavior.GetComponent<Animation>(), null, false);
        Constants.playAnimation(bonusQueueBehavior.GetComponent<Animation>(), null, false);
        Constants.playAnimation(bonusStackBehavior.GetComponent<Animation>(), null, false);

        //put above the goal completion view
        hexacoinsWalletBehavior.transform.SetParent(getLoadedGameObject("Activity10").transform);
        hexacoinsWalletBehavior.transform.SetAsFirstSibling();

        //not displayed most of the time because it's not necessary
        hexacoinsWalletBehavior.setOnlyDisplayedOnChanges(true);

        //put the wallet at a more visible position
        hexacoinsWalletBehavior.transform.position = Constants.newVector3(
            markerRef.posSafeAreaTopRight,
            -0.5f,
            -1,
            0
        );

        animFadeHUD = getHUD().GetComponent<Animation>();

        updateStackEnabling();

        onboardingHandler = new OnboardingHandler(this);

        //create the touch indicators
        var goIndicator = GameObject.Instantiate(
            GameHelper.Instance.loadPrefabAsset(Constants.PREFAB_NAME_ONBOARDING_CONTROLS_INDICATOR), 
            getParentTransform()
        );
        onboardingControlsIndicator = goIndicator.GetComponent<OnboardingControlsIndicatorBehavior>();
    }

    protected override void onPreResume() {
        base.onPreResume();

        if (previousActivity is Activity4) {

            //remove transition line
            if (lineDrawerPush != null) {

                Line oldLine = lineDrawerPush.line;

                //remove the current line instantly
                GameHelper.Instance.getLineDrawersManager().unregister(lineDrawerPush);
                GameHelper.Instance.getPool().storeLineGameObject(BaseModelBehavior.findModelBehavior<LineBehavior>(oldLine));

                //release
                lineDrawerPush = null;
            }

            //new game : first init
            axisBehavior.transform.localRotation = Quaternion.identity;

            generateFirstSnappedItems();
        }

        //update control indicator as it may changed in the settings screen after the pause
        onboardingControlsIndicator.gameObject.SetActive(gameManager.isControlsOptionDragHorizontal);
    }

    protected override void onResume(bool isFirst) {
        base.onResume(isFirst);

        GameHelper.Instance.getMainCameraBehavior().initInGameCamera();

        timeManager.resume(this);

        //preinit game :
        gameManager.startPlaying();

        buttonPause.menuButton.setVisible(true);

        //prepare the start time to calculate the time since the beginning of the game
        resetStartTime();


        //start timer of malus that have been stopped before starting a new level
        foreach (Item item in items) {

            if (item is ItemBonus) {

                ItemBonus itemBonus = item as ItemBonus;
                if (itemBonus.bonusType.isMalus &&
                    (itemBonus.isSnapped() || itemBonus.isStacked)) {
                    itemBonus.startSnappedMalusTimer();
                }
            }
        }


        if (popCode == Activity10c.POP_CODE_CHOSEN_BONUS) {

            //trigger chosen item
            BundlePop10c b = (BundlePop10c)bundlePop;

            ItemBonus selectedItem = b.selectedItem;

            selectedItem.addListener(this);
            selectedItem.setSelectable(true);

            //before selecting create a behavior to move it to the impact point if necessary
            if (selectedItem.bonusType.mustMoveItemToImpactPos(selectedItem)) {

                GameHelper.Instance.getPool().pickItemGameObject(
                    selectedItem,
                    null,
                    false,
                    Constants.newVector3(markerRef.posRight, 1, 0, 0)
                );
            }

            selectItemBonus(selectedItem);

        } else if (popCode == Activity11.POP_CODE_GIVE_UP) {

            //stop the game
            stopGame(pushGameOverActivity, T.Value.GAME_OVER_REASON_GIVE_UP);

        } else if (previousActivity is Activity4 || 
                   (previousActivity is Activity3 && nextActivity is Activity11 && !itemsGenerator.isGeneratingItems)) {//special case bugfix

            if (!gameManager.hasPassedOnboarding) {
                
                //trigger all onboarding speeches
                onboardingHandler.startOnboarding();

            } else {
                
                //generate items after a delay to avoid bugs
                Async.call(1, () => {

                    if (!isResumed) {
                        return;
                    }

                    itemsGenerator.startGeneration();

                    buttonPause.menuButton.setEnabled(true);

                    //show a random speech
                    CharacterSituation startSituation = newStartSituation();
                    if (startSituation != null) {

                        GameHelper.Instance.getCharacterAnimator()
                                  .show(this, true)
                                  .enqueue(startSituation)
                                  .enqueueHide();
                    }
                });
            }

        } else if (this is Activity10a && getCurrentLevel() == Constants.LEVEL_WHEN_BONUS_AVAILABLE_FOR_ONBOARDING) {

            updateStackEnabling();
        }

        //show some speeches frequently to put the player in trouble
        Async.call(playRandomSpeeches(), COROUTINE_TAG_RANDOM_SPEECHES);
    }

    protected override void onPrePause(bool isLast) {
        base.onPrePause(isLast);

        //stop showing random speeches
        Async.cancel(COROUTINE_TAG_RANDOM_SPEECHES);

        //save the elapsed duration since the beginning of the play
        updateElapsedDuration();
        startTime = null;
    }

    protected override void onPause() {
        base.onPause();

        timeManager.pause(this);

        //preinit game :
        gameManager.stopPlaying();
    }

    protected override void onDestroy() {
        base.onDestroy();

        //release activity in cam
        gameManager.setInGameActivity(null);

        //deinit all init GameObject
        bonusQueueBehavior.deinit();
        bonusStackBehavior.deinit();
        axisBehavior.deinit();
        itemsGeneratorBehavior.deinit();

        gameManager.setInGameObjectsActive(false);

        GameObject.Destroy(onboardingControlsIndicator.gameObject);

        //play the default menu music as the game stops
        playMusic(null);
    }


    protected virtual CharacterSituation newStartSituation() {
        
        return new CharacterSituation()
            .enqueueTrRandom("10.Start", 10)
            .enqueueExpression(CharacterRes.EXPR_DETERMINED, 3);
    }

    protected virtual CharacterSituation newRandomSituation() {

        return new CharacterSituation()
            .enqueueTrRandom("10.Random", 40);
    }

    private IEnumerator playRandomSpeeches() {

        while (true) {

            //show a random message every 40-60sec
            yield return new WaitForSeconds(Constants.newRandomFloat(60, 90));

            if (!GameHelper.Instance.getGameManager().isGamePlaying) {
                //can't show speeches if not playing
                continue;
            }
            if (timeManager.getTotalTimeScalePlay() <= 0) {
                //can't show speeches if about to switch level
                continue;
            }
            if (!gameManager.hasPassedOnboarding || onboardingHandler.isHandlingOnboarding) {
                //don't show any speech during onboarding to not disturb the new players
                continue;
            }

            CharacterSituation situation = newRandomSituation();
            if (situation == null) {
                continue;
            }

            GameHelper.Instance.getCharacterAnimator()
                  .show(this, true)
                  .enqueue(situation)
                  .enqueueHide();
        }
    }

    private void selectItemBonus(ItemBonus itemBonus, Action actionBeforeDestroy = null, Action completion = null) {

        ItemBonusBehavior itemBonusBehavior = BaseModelBehavior.findModelBehavior<ItemBonusBehavior>(itemBonus);

        //stacked or chosen bonus => move item bonus behavior then select
        string tag = "item_bonus_select_" + itemBonus.id;
        timeManager.pause(tag);

        //play zoom anim on the item bonus
        if (itemBonus.isSnapped() || itemBonus.wasStacked) {

            GameObjectPoolBehavior pool = GameHelper.Instance.getPool();
            GameObject goFx = pool.pickFXBonusSelectGameObject(itemBonusBehavior);

            Async.call(goFx.GetComponent<Animation>().clip.length, () => {
                pool.storeFXBonusSelectGameObject(goFx);
            });
        }

        bool mustMoveToImpact = itemBonus.bonusType.mustMoveItemToImpactPos(itemBonus);
        ItemSnapPosition impactPos = null;
        if (mustMoveToImpact) {
            impactPos = itemBonus.bonusType.getImpactPos(axis, itemBonus);
            itemBonus.impactPos = impactPos;
        }

        //if item doesn't require to move to impact pos or if item is already snapped on the impact pos, select it now
        if (!mustMoveToImpact || (itemBonus.isSnapped() && itemBonus.snapPosition == impactPos)) {

            //delay call to have the time to see the selection FX
            Async.call(0.5f, () => {

                timeManager.resume(tag);

                endItemSelect(itemBonus, actionBeforeDestroy, completion);
            });

            return;
        }

        Vector3 impactPosInWorld = Vector3.zero;

        //get the impact pos in grid to move the item game object
        if (impactPos != null) {
            impactPosInWorld = axisBehavior.calculateGameObjectPositionInGrid(impactPos, false);
        }

        //cancel current anim (triggered with the stack removing)
        itemBonusBehavior.cancelAnimationPosition();

        //move item bonus behavior
        itemBonusBehavior.animatePosition(
            new PositionInterpolatorBundle(impactPosInWorld, 0.6f, InterpolatorCurve.EASE_IN_OUT),
            true,
            (_) => {

                timeManager.resume(tag);

                endItemSelect(itemBonus, actionBeforeDestroy, completion);
            }
        );

    }

    private void endItemSelect(ItemBonus itemBonus, Action actionBeforeDestroy, Action completion) {

        if (itemBonus.bonusType.canBeRegisteredForGoals) {
            registerSelectedItems(itemBonus);
        }

        itemBonus.select();

        actionBeforeDestroy?.Invoke();

        itemBonus.destroy(ItemDestroyCause.Select);

        completion?.Invoke();
    }

    protected void updateStackEnabling() {

        bool isBonusStackEnabled = itemsGenerator.canGenerateBonusOrMalus();

        bonusStack.setEnabled(isBonusStackEnabled);
        buttonStackFirstBonus.menuButton.setVisible(isBonusStackEnabled);
    }


    public void generateFirstSnappedItems() {

        //generate first 6 items snapped on the axis
        for (int i = 0; i < 6; i++) {

            ItemSnapDirection direction = (ItemSnapDirection)i;

            ItemSnapPosition itemPos = new ItemSnapPosition(direction, 0, 0);

            ItemType newType;
            if (i == 1 || i == 3) {
                newType = ItemType.Type20;
            } else if (i == 5) {
                newType = ItemType.Type5;
            } else {
                newType = ItemType.Type1;
            }

            Item newItem = new Item(this, newType);
            registerItem(newItem);

            //let pop the item close to the pos it must be, it will then snap naturally
            GameHelper.Instance.getPool().pickItemGameObject(
                newItem,
                null,
                false,
                axisBehavior.calculateGameObjectPositionInGrid(itemPos, false)
            );

            //attach the new item to the current item pos
            axis.snapItem(newItem, itemPos);
        }

	}

    public void stopGame(Action completion, string trackingTagReason) {

        gameManager.stopPlaying();

        //reinit objects for the next playing
        itemsGenerator.stopGeneration();
        bonusStack.reset();
        bonusQueue.clear();

        //stop timers if any
        scoreMultiplier.clearItemsMultiplier();

        buttonPause.menuButton.setVisible(false);
        buttonStackFirstBonus.menuButton.setVisible(false);

        if (completion != null) {
            Async.call(animateAxisRelease(completion));
        }

        onGameStopped(trackingTagReason);

        updateElapsedDuration();
        trackTotalTime();
    }

    protected virtual void onGameStopped(string trackingTagReason) {
        //override this
    }

    protected abstract void pushGameOverActivity();

    protected abstract void trackTotalTime();

    private void resetStartTime() {
        
        startTime = Time.realtimeSinceStartup;
    }

    protected virtual void updateElapsedDuration() {

        if (startTime.HasValue) {
            totalDuration += Time.realtimeSinceStartup - startTime.Value;
        }

        resetStartTime();
    }


	public IEnumerator animateAxisRelease(Action completion) {

		timeManager.pause("axis_release_items");

		yield return new WaitForSeconds(0.5f);

		timeManager.resume("axis_release_items");

        axis.unsnapAllItems();

		//bump to expulse the items
		axisBehavior.bump();

		//wait for bump to finish
		yield return new WaitForSeconds(3f);


		//destroy all items game objects to avoid bugs (copy list to manage removing) 
		List<Item> itemsToDestroy = new List<Item>(items);
		foreach (Item item in itemsToDestroy) {

            if (item.isEnqueued) {
                //exception for remaining enqueued items
                continue;
            }

            item.destroy(ItemDestroyCause.System);
		}

        completion?.Invoke();
    }


	public List<Item> getItems() {
		return new List<Item>(items);
    }

    public List<Item> getFreeItems() {
        
        return items.FindAll((item) => 
            !item.isEnqueued && !item.isStacked && !item.isSnapped()
        );
    }

	public void registerItem(Item item) {

		if (items.Contains(item)) {
			return;
		}

		items.Add(item);
		item.addListener(this);
	}

	public void unregisterItem(Item item) {

		if (!items.Contains(item)) {
			return;
		}

		items.Remove(item);
		item.removeListener(this);
    }

	public void stackFirstBonus() {

		if (timeManager.getTotalTimeScalePlay() <= 0) {
			//can't interact with non UI elements if game is paused
			return;
		}

		ItemBonus pickedItemBonus = null;
        for (int i = 0 ; i < itemsGenerator.getQueueSize() ; i++) {

            Item item = itemsGenerator.getItemAt(i);
			if (item is ItemBonus) {
				pickedItemBonus = item as ItemBonus;
				break;
			}
		}

		if (pickedItemBonus != null) {
			stackItemBonus(pickedItemBonus);
		}
        
	}


#region BaseModelListener
	BaseModelBehavior BaseModelListener.getModelBehavior() {
		return null;
	}
#endregion

#region ItemListener
	void ItemListener.onItemTypeChange(Item item) {
		//do nothing
	}

    void ItemListener.onFallingItemCollide(Item item, Vector3 currentPos) {

        onFallingItemCollide(item, axisBehavior.findFreeAdjacentPosition(currentPos));

		GameHelper.Instance.getMainCameraBehavior().shakeVertical();
	}

	void ItemListener.onSnappedItemClick(Item item) {
		selectSnappedItem(item);
	}

    void ItemListener.onItemSelectableChange(Item item) {
        //do nothing
    }

	void ItemListener.onItemSelect(Item item) {

		GameHelper.Instance.getMainCameraBehavior().shakeHorizontal();

		string tag = "item_select_" + item.id;
		timeManager.pause(tag);

        Async.call(Constants.DELAY_BETWEEN_ITEMS_SELECTION_S, () => {

			timeManager.resume(tag);
		});

	}

	void ItemListener.onItemChangeZone(Item item) {

        //track before processing
        trackItemChangeZone(item);

		if (item.isStacked) {

			string tag = "item_stack_" + item.id;
			timeManager.pause(tag);

            Async.call(Constants.DELAY_BETWEEN_ITEMS_SELECTION_S, () => {

				timeManager.resume(tag);

			});

            return;
		}

        if (item.isSnapped()) {

            //check if the item is outside the limit : end
            if (axis.isItemOutsideLimit(item.snapPosition)) {

                if (!gameManager.hasPassedOnboarding) {
                    //show a speech to incitate to finish the first level
                    onboardingHandler.onFailedTooSoon();
                                     
                } else {
                    //finish
                    stopGame(pushGameOverActivity, T.Value.GAME_OVER_REASON_OUSIDE_LIMIT);
                }

                return;
            }

            onboardingHandler.onItemAttach(item);
        }

	}

    private void trackItemChangeZone(Item item) {

        if (item.isSnapped()) {

            TrackingManager.instance.prepareEvent(T.Event.ITEM_SNAP)
                           .add(T.Param.ITEM_TYPE, item.itemType.ToString())
                           .add(T.Param.BONUS_TYPE, (item as ItemBonus)?.getTag())
                           .add(T.Param.NB_SNAPPED_ITEMS, axis.getNbSnappedItems())
                           .track();

        } else if (item.wasSnapped) {

            TrackingManager.instance.prepareEvent(T.Event.ITEM_UNSNAP)
                           .add(T.Param.ITEM_TYPE, item.itemType.ToString())
                           .add(T.Param.BONUS_TYPE, (item as ItemBonus)?.getTag())
                           .add(T.Param.NB_SNAPPED_ITEMS, axis.getNbSnappedItems())
                           .track();

        } else if (item.isStacked) {

            TrackingManager.instance.prepareEvent(T.Event.ITEM_STACK)
                           .add(T.Param.ITEM_TYPE, item.itemType.ToString())
                           .add(T.Param.BONUS_TYPE, (item as ItemBonus)?.getTag())
                           .add(T.Param.NB_SNAPPED_ITEMS, axis.getNbSnappedItems())
                           .track();

        } else if (item.wasStacked) {

            TrackingManager.instance.prepareEvent(T.Event.ITEM_UNSTACK)
                           .add(T.Param.ITEM_TYPE, item.itemType.ToString())
                           .add(T.Param.BONUS_TYPE, (item as ItemBonus)?.getTag())
                           .add(T.Param.NB_SNAPPED_ITEMS, axis.getNbSnappedItems())
                           .track();
        }
    }

    void ItemListener.onItemDestroyRequest(Item item, bool wasSnapped, ItemDestroyCause cause) {

        unregisterItem(item);

        if (cause == ItemDestroyCause.BonusErosion) {

            Constants.playFX("FX.Bonus.Erosion", Constants.newVector3(BaseModelBehavior.findTransform(item).position, 0, 0, -1));

        } else {

            if (wasSnapped) {
                Constants.playFX("FX.Item.Disappear", Constants.newVector3(BaseModelBehavior.findTransform(item).position, 0, 0, -1));
            }
        }

    }
#endregion

#region ItemBonusListener
    void ItemBonusListener.onBonusTypeChange(ItemBonus itemBonus) {
        //do nothing
    }

    void ItemBonusListener.onEnqueuedItemBonusClick(ItemBonus itemBonus) {

		stackItemBonus(itemBonus);
	}

	void ItemBonusListener.onStackedItemBonusClick(ItemBonus itemBonus) {

		selectStackedItem(itemBonus);
	}

	void ItemBonusListener.onItemBonusInstantSelect(ItemBonus itemBonus) {
        //do nothing
	}

	void ItemBonusListener.onItemBonusPersistentSelect(ItemBonus itemBonus) {

		//enqueue the bonus, no need to stop time, it has been done in base.select()
		bonusQueue.enqueueBonus(itemBonus);
	}

	void ItemBonusListener.onTimerMalusTriggerStart(ItemBonus itemBonus) {
		//do nothing
	}

	void ItemBonusListener.onTimerMalusTriggerProgress(ItemBonus itemBonus) {
		//do nothing
	}

	void ItemBonusListener.onTimerMalusTriggerFinish(ItemBonus itemBonus) {

		if (itemBonus.isSnapped()) {
			selectSnappedItem(itemBonus);
		} else if (itemBonus.isStacked) {
			selectStackedItem(itemBonus);
		}
	}

	void ItemBonusListener.onTimerMalusTriggerCancel(ItemBonus itemBonus) {
		//do nothing
	}
#endregion

#region BonusStackListener
    void BonusStackListener.onBonusStackEnableChange(BonusStack bonusStack) {
        //do nothing
    }

    void BonusStackListener.onBonusStackSizeChange(BonusStack bonusStack, int previousStackSize) {

        //avoid sending an event at the end of the game
        if (gameManager.isGamePlaying) {
            return;
        }

        int stackSize = bonusStack.stackSize;
        if (stackSize == previousStackSize) {
            //no changes to track
            return;
        }

        var eventName = (stackSize > previousStackSize) ? T.Event.STACK_INCREASE : T.Event.STACK_DECREASE;

        TrackingManager.instance.prepareEvent(eventName)
                   .add(T.Param.STACK_SIZE, stackSize)
                   .add(T.Param.NB_STACKED_ITEMS, bonusStack.getStackCount())
                   .track();
	}

	void BonusStackListener.onBonusStackItemBonusAdd(BonusStack bonusStack, ItemBonus itemBonus) {

        onboardingHandler.onItemBonusStacked(itemBonus);

		itemBonus.bonusType.onItemBonusSnapped(itemBonus);

		//shake the camera when the item has finished the falling animation
        Async.call(BaseModelBehavior.findModelBehavior<ItemBehavior>(itemBonus).getAnimationRemainingTimeSec(), () => {

			GameHelper.Instance.getMainCameraBehavior().shakeVertical();
		});
	}

	void BonusStackListener.onBonusStackItemBonusRemove(BonusStack bonusStack, ItemBonus itemBonus) {
		//do nothing
	}

	void BonusStackListener.onBonusStackClear(BonusStack bonusStack) {
		//do nothing
	}
#endregion

#region BonusQueueListener
    void BonusQueueListener.onBonusQueueEnqueue(BonusQueue bonusQueue, TimerRunningBonus timer) {

        scoreMultiplier.updateBonusMultiplier();
    }

    void BonusQueueListener.onBonusQueueDequeue(BonusQueue bonusQueue, TimerRunningBonus timer) {

        scoreMultiplier.updateBonusMultiplier();
    }

    void BonusQueueListener.onBonusQueueClear(BonusQueue bonusQueue) {

        scoreMultiplier.updateBonusMultiplier();
    }
#endregion

#region ItemsGeneratorListener
    void ItemsGeneratorListener.onItemsGeneratorStart(ItemsGenerator itemsGenerator) {
        //do nothing
    }

    void ItemsGeneratorListener.onItemsGeneratorStop(ItemsGenerator itemsGenerator) {
        //do nothing
    }

    void ItemsGeneratorListener.onItemsGeneratorAddItem(ItemsGenerator itemsGenerator, Item item) {

        updateButtonStackEnabling();

        onboardingHandler.onItemGenerated(item);

        TrackingManager.instance.prepareEvent(T.Event.ITEM_GENERATION)
                       .add(T.Param.ITEM_TYPE, item.itemType.ToString())
                       .add(T.Param.BONUS_TYPE, (item as ItemBonus)?.getTag())
                       .add(T.Param.NB_SNAPPED_ITEMS, axis.getNbSnappedItems())
                       .track();
    }

    void ItemsGeneratorListener.onItemsGeneratorRemoveItem(ItemsGenerator itemsGenerator, Item item) {
        updateButtonStackEnabling();
    }

    void ItemsGeneratorListener.onItemsGeneratorDequeueItem(ItemsGenerator itemsGenerator, Item item) {
        updateButtonStackEnabling();
    }

    void ItemsGeneratorListener.onItemsGeneratorClearItems(ItemsGenerator itemsGenerator) {
        updateButtonStackEnabling();
    }
#endregion

    private void updateButtonStackEnabling() {
        buttonStackFirstBonus.menuButton.setEnabled(bonusStack.isEnabled &&
                                                    bonusStack.stackSize > 0 &&
                                                    itemsGenerator.hasItemBonus());
    }

    public void stackItemBonus(ItemBonus itemBonus) {

		if (bonusStack.stackSize <= 0) {
			//stack item only if the stack size is not zero
			return;
		}

		itemsGenerator.pickItem(itemBonus);
		bonusStack.stackItem(itemBonus);
	}

    private void onFallingItemCollide(Item item, ItemSnapPosition newPosition) {

		if (newPosition == null) {
			throw new ArgumentException();
		}

		//get the state of the item before snapping it in order to check if the item must me eroded
		bool wasSnapped = item.hasBeenSnappedBefore;

		axis.snapItem(item, newPosition);

		//if the erosion bonus is active, destroy the non-bonus and non-unsnapped items
		if (bonusQueue.hasEnqueuedErosion() && 
			item.itemType != ItemType.Bonus &&
			!wasSnapped) {

			HashSet<Item> attachedItems = axis.getNearbyItemsGroup(item);

			//group contains itself : can't destroy if the item is alone
			if (attachedItems.Count > 1) {
                axis.destroySnappedItems(attachedItems, true, ItemDestroyCause.BonusErosion);
			}
		}

	}

	private void selectStackedItem(ItemBonus itemBonus) {

		if (!itemBonus.isClickable()) {
			return;
		}
		if (!itemBonus.isStacked) {
			return;
		}

        onboardingHandler.onItemSelect(itemBonus);

		bonusStack.unstackItem(itemBonus, false);

        selectItemBonus(itemBonus);
	}

	private void selectSnappedItem(Item item) {

		if (!item.isClickable()) {
			return;
		}
		if (!item.isSnapped()) {
			return;
		}

		if (item.itemType == ItemType.Bonus) {

			selectSnappedItemBonus((ItemBonus)item);

		} else {

			selectSnappedItemsGroup(item);
		}
	}

	private void selectSnappedItemBonus(ItemBonus item) {

        //track before animating retrieving
        TrackingManager.instance.prepareEvent(T.Event.ITEM_SELECT)
                       .add(T.Param.ITEM_TYPE, item.itemType.ToString())
                       .add(T.Param.BONUS_TYPE, (item as ItemBonus)?.getTag())
                       .add(T.Param.NB_SNAPPED_ITEMS, axis.getNbSnappedItems())
                       .add(T.Param.NB_GROUPPED_ITEMS, 1)
                       .track();
        
        selectItemBonus(
            item,
            () => axis.unsnapItem(item), 
            axis.unsnapFreeItems
        );

        onboardingHandler.onItemSelect(item);
	}

    private void selectSnappedItemsGroup(Item memberItem) {

        HashSet<Item> itemsGroup = axis.getAttachedItemsGroup(memberItem, true);

        if (itemsGroup.Count < Constants.NB_ITEMS_TO_SELECT_GROUP) {
            //can only select groups of more than one item
            return;
        }

        //track before animating retrieving
        TrackingManager.instance.prepareEvent(T.Event.ITEM_SELECT)
                       .add(T.Param.ITEM_TYPE, memberItem.itemType.ToString())
                       .add(T.Param.BONUS_TYPE, (memberItem as ItemBonus)?.getTag())
                       .add(T.Param.NB_SNAPPED_ITEMS, axis.getNbSnappedItems())
                       .add(T.Param.NB_GROUPPED_ITEMS, itemsGroup.Count)
                       .track();

        if (memberItem.itemType == ItemType.Type100) {
            
            //select directly, no new item will be generated
            endSelectSnappedItemsGroup(memberItem, itemsGroup);
            return;
        }

        //animate items join to generate a new one

        string tag = "item_select_move_" + memberItem.id;
        timeManager.pause(tag);

        Vector3 impactPosInWorld = BaseModelBehavior.findTransform(memberItem).position;

        foreach (Item item in itemsGroup) {

            //move item
            BaseModelBehavior.findModelBehavior<ItemBehavior>(item).animatePosition(
                new PositionInterpolatorBundle(impactPosInWorld, 0.1f, InterpolatorCurve.EASE_IN),
                true,
                null
            );
        }

        //finish the selection
        Async.call(0.12f, () => {

            timeManager.resume(tag);

            endSelectSnappedItemsGroup(memberItem, itemsGroup);
        });
    }

    private void endSelectSnappedItemsGroup(Item memberItem, HashSet<Item> itemsGroup) {
        
        GameObjectPoolBehavior pool = GameHelper.Instance.getPool();
        Transform flyingObjectsParent = getLoadedGameObject("Activity10").transform;
        Dictionary<Item, Vector3> itemsUIPositions = new Dictionary<Item, Vector3>();

        //store temporarily the item pos before unsnapping it
        ItemSnapPosition memberItemPos = memberItem.snapPosition;

        registerSelectedItems(memberItem, itemsGroup);

		foreach (Item item in itemsGroup) {

			if (item.itemType != memberItem.itemType) {
				throw new InvalidOperationException("BUG : not possible");
			}
            
            itemsUIPositions.Add(item, axisBehavior.calculateGameObjectPositionInGrid(item.snapPosition, false));
            
            item.select();
			axis.unsnapItem(item);
            item.destroy(ItemDestroyCause.Select);
		}

		if (memberItem.itemType == ItemType.Type100) {

            //earn 1 hexacoin when black items group is selected
            earnHexacoinsAfterItemsSelect(1, flyingObjectsParent, itemsUIPositions[memberItem], T.Value.EARN_REASON_BLACK_ITEMS);

        } else {

			//generate a new item at the selected place
			Item newItem = new Item(this, memberItem.itemType + 1);
			registerItem(newItem);

			//let pop the item close to the pos it must be, it will then snap naturally
			GameHelper.Instance.getPool().pickItemGameObject(
				newItem,
                null, 
				false, 
                axisBehavior.calculateGameObjectPositionInGrid(memberItemPos, false)
			);

            //snap it if there are items near
            if (memberItemPos.level <= 0 || axis.getAdjacentItems(memberItemPos).Count > 0) {
                axis.snapItem(newItem, memberItemPos);
            }
        }

        axis.unsnapFreeItems();

        //hidden feature : add an extra hexacoin when the user shake his device for 1sec vertically
        if (itemsGroup.Count >= Constants.NB_ITEMS_TO_SHAKE_DEVICE) {

            DeviceShakeDetector.detectShaking(
                1,
                () => earnHexacoinsAfterItemsSelect(1, flyingObjectsParent, itemsUIPositions[memberItem], T.Value.EARN_REASON_SHAKE_DEVICE)
            );
        }

        onboardingHandler.onItemSelect(memberItem);

        //bake item score
        int itemScore = (int)(ItemTypeMethods.getScore(memberItem.itemType) * scoreMultiplier.totalMultiplier);
        if (itemScore <= 0) {
            itemScore = 1;
        }

        //optim : call after a small delay to avoid a lag
        Async.call(0.05f, () => {

            //add current score for removed items
            scoreCounter.addScore(itemScore * itemsGroup.Count);

            //increment the multiplier after the score to avoid adding the multiplied score
            scoreMultiplier.addItemsMultiplier(1);
        });

        //optim : call after a small delay to avoid a lag
        Async.call(0.1f, () => {
            
            //show the flying scores
            foreach (Item item in itemsGroup) {

                GameObject go = pool.pickFlyingGameObject(
                    true,
                    null,
                    false,
                    itemsUIPositions[item]);

                //move from 3D object to UI position
                go.transform.SetParent(flyingObjectsParent);
                go.transform.localScale = Vector3.one;

                go.GetComponent<FlyingScoreBehavior>().startFlying("+" + itemScore, () => {
                    pool.storeFlyingGameObject(true, go);
                });
            }
        });
	}

    protected virtual void registerSelectedItems(Item memberItem, HashSet<Item> itemsGroup = null) {
		//override if needed
	}

    private void earnHexacoinsAfterItemsSelect(int nbEarnedHexacoins, Transform flyingObjectsParent, Vector3 selectedItemPos, string trackingTagReason) {

        GameObjectPoolBehavior pool = GameHelper.Instance.getPool();

        //hide then the HUD
        Constants.playAnimation(animFadeHUD, "UIHUD.SemiFade", false);

        //then reveal the HUD
        Async.call(2.5f, () => {
            Constants.playAnimation(animFadeHUD, "UIHUD.SemiFade", true);
        });

        //show the flying hexacoin
        GameObject go = pool.pickFlyingGameObject(
            false,
            null,
            false,
            Constants.newVector3(selectedItemPos, 0, 1, 0)
        );

        //move from 3D object to UI position
        go.transform.SetParent(flyingObjectsParent);
        go.transform.localScale = Vector3.one;

        go.GetComponent<FlyingScoreBehavior>().startFlying("+" + nbEarnedHexacoins, () => {
            pool.storeFlyingGameObject(false, go);
        });

        Async.call(0.3f, () => {
            gameManager.addHexacoins(nbEarnedHexacoins, getActivityName(), trackingTagReason);
        });
    }


	protected override void onButtonClick(MenuButtonBehavior menuButton) {

        if (menuButton == buttonPause) {

            pauseGame();

        } else if (menuButton == buttonStackFirstBonus) {

            stackFirstBonus();

		} else {

			base.onButtonClick(menuButton);
		}

	}

    public void pauseGame() {

        if (!isResumed) {
            //already paused or destroying
            return;
        }

        if (onboardingHandler.isHandlingOnboarding) {
            //can't pause the game when showing the onboarding speeches
            return;
        }

        if (!gameManager.isGamePlaying) {
            //can't pause the game when not playing
            return;
        }
        if (timeManager.getTotalTimeScalePlay() <= 0) {
            //can't pause the game when freeze
            return;
        }

        push(new Activity11());
    }

	public void showBonusChoiceDialog(ItemBonus[] items) {

        BundlePush10c b = new BundlePush10c {
            itemsToChoose = items
        };
        
		push(new Activity10c().setBundlePush(b));

	}

    public void pauseAllMalusTimers() {
        
        foreach (var item in items) {

            if (!(item is ItemBonus)) {
                continue;
            }

            var itemBonus = item as ItemBonus;

            if (!itemBonus.bonusType.isMalus) {
                continue;
            }

            if (!itemBonus.isSnapped() && !itemBonus.isStacked) {
                continue;
            }

            itemBonus.cancelSnappedMalusTimer();
            itemBonus.setSnappedMalusTimerDisabled(true);
        }
    }

    public void resumeAllMalusTimers() {

        foreach (var item in items) {

            if (!(item is ItemBonus)) {
                continue;
            }

            var itemBonus = item as ItemBonus;

            if (!itemBonus.bonusType.isMalus) {
                continue;
            }

            if (!itemBonus.isSnapped() && !itemBonus.isStacked) {
                continue;
            }

            itemBonus.setSnappedMalusTimerDisabled(false);
            itemBonus.startSnappedMalusTimer();
        }
    }

}

