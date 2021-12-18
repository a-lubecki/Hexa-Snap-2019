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
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;


public class Activity23 : BaseUIActivity, IPurchasesManagerListener {


    private static readonly int SHOP_LIST_PADDING = 50;
    private static readonly int SHOP_ITEMS_HEIGHT = 300;

    private static readonly GameObject PREFAB_SHOP_ITEM = GameHelper.Instance.loadPrefabAsset(Constants.PREFAB_NAME_IN_APP_PURCHASE);


    private MenuButtonBehavior buttonInfo;

    private Transform trScrollView;
    private Transform trScrollViewContent;

    private List<ShopItem> shopItems = new List<ShopItem>();

    private ShopItem processingItem = null;


    protected override MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager) {
		return markerManager.markerIShop;
	}

	protected override string[] getPrefabNamesToLoad() {
		return new string[] { "Activity23" };
	}

	protected override string getTitleForInit() {
		return Tr.get("Activity23.Title");
	}

    protected override bool hasDefaultHexacoinWallet() {
        return true;
    }

    protected override Vector3 getCharacterPosInside() {
        return Constants.newVector3(markerRef.posTopLeft, 2, -2.5f, 0);
    }

    protected override CharacterSituation getFirstCharacterSituation(bool isFirstResume) {
        
        if (ShopItem.getNbFreeAvailableItems() > 0) {

            return new CharacterSituation()
                .enqueueTr("23.BonusAvailable")
                .enqueueMove(CharacterRes.MOVE_JUMP);
        }

        if (gameManager.hasRemovedAds) {

            return new CharacterSituation()
                .enqueueTr("23.AdsFree");
        }

        //propose to remove ads when the player has done at least 2 buys
        if (gameManager.purchasesManager.nbPurchasesDone >= 2) {
            
            return new CharacterSituation()
                .enqueueTr("23.AdsAsk");
        }

        return new CharacterSituation()
            .enqueueTr("23.Default")
            .enqueueExpression(CharacterRes.EXPR_AMAZED, 3);
    }

    protected override bool hasAdBanner() {
        return false;
    }

    protected override void onCreate() {
		base.onCreate();

        buttonInfo = createButtonGameObject(
            this,
            markerRef.transform.Find("Activity23"),
            markerRef.posSafeAreaBottomRight,
            MenuButtonSmall.newButtonDefault(
                "MenuButton.Info"
            )
        );

        trScrollView = findChildTransform("ScrollView");
        trScrollViewContent = trScrollView.Find("Viewport").Find("Content");

        gameManager.purchasesManager.initIAP(this);

        SpecificDeviceManager.Instance.adaptScroll(trScrollView.GetComponent<ScrollRect>());
    }

    protected override void onPreResume() {
        base.onPreResume();

        updateListItems(false);
    }

    private void updateListItems(bool mustCheckIfResumed = true) {
        
        if (mustCheckIfResumed && !isResumed) {
            //fix bug when the activity is closed
            return;
        }

        shopItems.Clear();

        //fill list with items
        foreach (var item in ShopItem.PURCHASES) {
            
            if (!item.isVisible()) {
                continue;
            }
            
            shopItems.Add(item);
        }

        //add a loader item
        if (gameManager.purchasesManager.isInitializingIAP) {
            shopItems.Add(ShopItem.LOADER);
        }

        updateScrollView();
    }

    private void updateScrollView() {
        
        int nbItems = trScrollViewContent.childCount;
        for (int i = 0 ; i < nbItems ; i++) {

            var child = trScrollViewContent.GetChild(0);
            child.SetParent(null);
            GameObject.Destroy(child.gameObject);
        }

        int pos = 0;

        foreach (var item in shopItems) {

            GameObject goItem = GameObject.Instantiate(PREFAB_SHOP_ITEM, trScrollViewContent);

            ShopItemBehavior shopItemBehavior = goItem.GetComponent<ShopItemBehavior>();
            shopItemBehavior.init(item);

            if (item == processingItem) {
                shopItemBehavior.forceLoading();
            }

            goItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -SHOP_LIST_PADDING - 0.5f * SHOP_ITEMS_HEIGHT - pos * SHOP_ITEMS_HEIGHT);

            goItem.GetComponent<Button>().onClick.AddListener(() => {

                processPurchase(item);
            });

            pos++;
        }

        //set the final size of the scrollview with the number of elements
        RectTransform tr = trScrollViewContent.GetComponent<RectTransform>();
        tr.sizeDelta = new Vector2(tr.sizeDelta.x, shopItems.Count() * SHOP_ITEMS_HEIGHT + 2 * SHOP_LIST_PADDING);
    }

    protected override void onButtonClick(MenuButtonBehavior menuButton) {

        if (menuButton == buttonInfo) {
            LegalPopupManager.show();
        }

        base.onButtonClick(menuButton);
    }

    private void processPurchase(ShopItem item) {
        
        if (gameManager.rewardsManager.isShopItemBlocked(item)) {

            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueTr("23.BonusBlocked");
            return;
        }

        //prevent from displaying the speech on the wallet
        GameHelper.Instance.getCharacterAnimator().stop();

        if (item == ShopItem.REWARD_DAILY) {

            //retrieve the time of the server to ensure the delay between rewards is ensured
            prepareReward(item, () => onRewardNeeded(item));

        } else if (item == ShopItem.REWARD_AD_VIDEO) {

            var adsManager = GameHelper.Instance.getAdsManager();

            //preinit the video
            adsManager.loadRewardVideoOnce();

            //retrieve the time of the server to ensure the delay between rewards is ensured
            prepareReward(item, () => {

                startProcessingItem(item);

                adsManager.showRewardedAd(
                    () => onRewardNeeded(item),
                    () => {
                        
                        endProcessingItem();

                        GameHelper.Instance.getNativePopupManager().show(
                            Tr.get("P5b.Title"),
                            adsManager.lastRewardFailMessage,
                            null,
                            null
                        );
                    },
                    endProcessingItem
                );
            });

        } else if (item.type == ShopItemType.IAP) {
            
            gameManager.purchasesManager.processPurchase(item);

            //update item loading
            updateListItems();
        }

        if (item.type == ShopItemType.REWARD) {

            TrackingManager.instance.prepareEvent(T.Event.REWARD_CLICK)
                           .add(T.Param.ID, item.tag)
                           .add(T.Param.NB_HEXACOINS, item.nbEarnedHexacoins)
                           .track();
            
        } else if (item.type == ShopItemType.IAP) {
            
            TrackingManager.instance.prepareEvent(T.Event.IAP_CLICK)
                           .add(T.Param.ID, item.tag)
                           .add(T.Param.NB_HEXACOINS, item.nbEarnedHexacoins)
                           .add(T.Param.REMOVING_ADS, item.isRemovingAds ? T.Value.TRUE : T.Value.FALSE)
                           .track();
        }
    }

    private void prepareReward(ShopItem item, Action completion) {

        startProcessingItem(item);
        
        gameManager.rewardsManager.retrieveDateTimeShift(
            item, 
            canCollectReward => {

                if (!canCollectReward) {
                    endProcessingItem();
                    return;
                }

                completion();
            }, 
            () => {

                endProcessingItem();

                showPurchaseTransactionError();
            }
        );
    }

    private void onRewardNeeded(ShopItem item) {

        endProcessingItem();

        gameManager.rewardsManager.onRewardCollected(item, getActivityName());

        //propose to come back after a time
        if (isResumed && item == ShopItem.REWARD_DAILY) {
            
            GameHelper.Instance.getCharacterAnimator()
                      .show(this, true)
                      .enqueueDelay(CharacterTimeline.SPEECH, 2)
                      .enqueueTr("23.BonusEarned")
                      .enqueue(new CharacterAnimationExpr(CharacterRes.EXPR_BLINK, 0.5f));
        }
    }

    #region IPurchasesManagerListener
    void IPurchasesManagerListener.onPurchaseInitDone() {

        //reset all list
        updateListItems();
    }

    void IPurchasesManagerListener.onPurchaseInitFailed() {

        //remove loader item
        updateListItems();

        GameHelper.Instance.getNativePopupManager().show(
            Tr.get("P3c.Title"),
            Tr.get("P3c.Message"),
            null,
            null
        );
    }

    void IPurchasesManagerListener.onProcessPurchaseStart(ShopItem item) {
        
        startProcessingItem(item);
    }

    void IPurchasesManagerListener.onProcessPurchaseFailed(ShopItem item, bool wasCanceled) {

        endProcessingItem();

        if (!wasCanceled) {
            showPurchaseTransactionError();
        }
    }

    void IPurchasesManagerListener.onPurchaseSendingStart(ShopItem item) {

        startProcessingItem(item);
    }

    void IPurchasesManagerListener.onPurchaseSendingDone(ShopItem item, Purchase purchase) {

        endProcessingItem();

        //show login or confirmation popup
        BundlePush24 b = new BundlePush24 {
            nbPaidHexacoins = purchase.purchaseNbHexacoins,
            hadRemovedAds = purchase.hasRemovedAds
        };

        if (!LoginManager.Instance.isLoggedInFacebook()) {
            push(new Activity24a().setBundlePush(b));
        } else {
            push(new Activity24b().setBundlePush(b));
        }
    }

    void IPurchasesManagerListener.onPurchaseSendingFailed(ShopItem item, PurchaseEventArgs pendingArgs, IPurchaseReceipt pendingReceipt) {

        endProcessingItem();

        //propose to retry
        GameHelper.Instance.getNativePopupManager().show(
            Tr.get("P3b.Title"),
            Tr.get("P3b.Message"),
            Tr.get("P.Retry"),
            () => gameManager.purchasesManager.sendPurchaseToFirebase(pendingArgs, pendingReceipt),
            Tr.get("P.Close"),
            null
        );
    }
    #endregion

    private void showPurchaseTransactionError() {

        //can't retrieve the timeshift
        GameHelper.Instance.getNativePopupManager().show(
            Tr.get("P3d.Title"),
            Tr.get("P3d.Message"),
            null,
            null
        );
    }

    private void startProcessingItem(ShopItem item) {

        //block UI to avoid quitting the screen
        gameManager.setUIEventsEnabled(false);

        //display loader
        processingItem = item;

        if (isResumed) {
            updateListItems();
        }
    }

    private void endProcessingItem() {

        //reactivate clicks
        gameManager.setUIEventsEnabled(true);

        //hide loader
        processingItem = null;

        if (isResumed) {
            updateListItems();
        }
    }

}

