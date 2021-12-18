/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine.Purchasing;


public class ShopItem : BaseModel {


    public ShopItemType type { get; private set; }

    public string tag { get; private set; }

    public int nbEarnedHexacoins { get; private set; }

    public bool isRemovingAds { get; private set; }

    private Func<string> dynamicText;

    public bool isVisibleWithAds { get; private set; }
    public bool isVisibleWithNoAds { get; private set; }

    public int nbBlockingMinutes { get; private set; }

    public string iconName { get; private set; }

    public Product iapProduct { get; private set; }


    public ShopItem(ShopItemType type, string tag, int nbEarnedHexacoins, bool isRemovingAds, 
                    Func<string> dynamicText, bool isVisibleWithAds = true, bool isVisibleWithNoAds = true, 
                    int nbBlockingMinutes = 0, string iconName = null) {
    
        this.type = type;
        this.tag = tag;
        this.nbEarnedHexacoins = nbEarnedHexacoins;
        this.isRemovingAds = isRemovingAds;
        this.dynamicText = dynamicText;
        this.isVisibleWithAds = isVisibleWithAds;
        this.isVisibleWithNoAds = isVisibleWithNoAds;
        this.nbBlockingMinutes = nbBlockingMinutes;
        this.iconName = iconName;
    }

    public bool isVisible() {

        var gameManager = GameHelper.Instance.getGameManager();

        if (type == ShopItemType.IAP &&
            (iapProduct == null || !gameManager.purchasesManager.isIAPInitialized())) {
            //IAP products not loaded
            return false;
        }

        if (gameManager.hasRemovedAds) {
            if (!isVisibleWithNoAds) {
                return false;
            }
        } else {
            if (!isVisibleWithAds) {
                return false;
            }
        }

        return true;
    }

    public string getDynamicText() {

        if (dynamicText == null) {
            return "";
        }

        return dynamicText();
    }

    public void setIAPProduct(Product iapProduct) {

        if (iapProduct == null) {
            return;
        }

        this.iapProduct = iapProduct;
    }


    public static ShopItem LOADER = new ShopItem(
        ShopItemType.LOADER,
        "loader",
        0,
        false,
        () => Tr.get("Activity23.Text.Loading"),
        false,
        false
    );

    public static ShopItem REWARD_DAILY = new ShopItem(
        ShopItemType.REWARD,
        "dailyReward",
        5,
        false,
        () => Tr.get("Activity23.Text.Reward.Daily"),
        true,
        true,
        Constants.BLOCKING_TIME_MIN_REWARD_DAILY,
        "MenuButton.Rewards.Daily"
    );

    public static ShopItem REWARD_AD_VIDEO = new ShopItem(
        ShopItemType.REWARD,
        "adVideoReward",
        5,
        false,
        () => Tr.get(GameHelper.Instance.getGameManager().hasRemovedAds ? "Activity23.Text.Reward.NoAdsVideo" : "Activity23.Text.Reward.AdVideo"),
        true,
        true,
        Constants.BLOCKING_TIME_MIN_REWARD_AD_VIDEO,
        "MenuButton.Rewards.AdVideo"
    );


    public static ShopItem[] PURCHASES = {
        REWARD_DAILY,
        REWARD_AD_VIDEO,
        new ShopItem(
            ShopItemType.IAP,
            "com.hexasnap.add40",
            40,
            false,
            null
        ),
        new ShopItem(
            ShopItemType.IAP,
            "com.hexasnap.add100",
            100,
            false,
            () => Tr.get("Activity23.Text.Popular")
        ),
        new ShopItem(
            ShopItemType.IAP,
            "com.hexasnap.add200",
            200,
            false,
            null
        ),
        new ShopItem(
            ShopItemType.IAP,
            "com.hexasnap.add300",
            300,
            true,
            () => getPurchaseRemoveAdText(null)
        ),
        new ShopItem(
            ShopItemType.IAP,
            "com.hexasnap.add500",
            500,
            true,
            () => getPurchaseRemoveAdText(null)
        ),
        new ShopItem(
            ShopItemType.IAP,
            "com.hexasnap.add1000",
            1000,
            true,
            () => getPurchaseRemoveAdText(Tr.get("Activity23.Text.BestOffer"))
        )
    };

    private static string getPurchaseRemoveAdText(string purchaseText) {
        
        if (GameHelper.Instance.getGameManager().hasRemovedAds) {
            return purchaseText;
        }

        return Tr.get("Activity23.Text.AdFree");
    }

    public static int getNbFreeAvailableItems() {

        int res = 0;

        GameManager gameManager = GameHelper.Instance.getGameManager();

        foreach (var item in PURCHASES) {

            if (item.type != ShopItemType.REWARD) {
                continue;
            }

            if (!item.isVisible()) {
                continue;
            }

            if (!gameManager.rewardsManager.isShopItemBlocked(item)) {
                res++;
            }
        }

        return res;
    }

}

