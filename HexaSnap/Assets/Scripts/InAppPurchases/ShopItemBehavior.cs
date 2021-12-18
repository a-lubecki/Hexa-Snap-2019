/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class ShopItemBehavior : BaseModelBehavior {


    public ShopItem shopItem {
        get {
            return (ShopItem) model;
        }
    }


    private GameObject goLoadingAnim;
    private Text textNbHexacoins;
    private Text textDescription;
    private GameObject goImageButton;
    private Text textPrice;
    private RawImage imageIcon;

    private BadgeBehavior badge;

    private bool isLoading;

    private string coroutineTagUpdateRewardText;


    protected override void onAwake() {
        base.onAwake();

        coroutineTagUpdateRewardText = "updateRewardText_" + Constants.newRandomPositiveInt();

        Transform trImageBackground = transform.Find("ImageBackground");
        goLoadingAnim = trImageBackground.Find("LoadingAnim").gameObject;
        textNbHexacoins = trImageBackground.Find("TextNbHexacoins").GetComponent<Text>();

        textDescription = transform.Find("TextDescription").GetComponent<Text>();

        Transform trImageButton = transform.Find("ImageButton");
        goImageButton = trImageButton.gameObject;
        textPrice = trImageButton.Find("TextPrice").GetComponent<Text>();
        imageIcon = trImageButton.Find("ImageIcon").GetComponent<RawImage>();

        badge = trImageButton.Find("Badge").GetComponent<BadgeBehavior>();
    }

    protected override void onInit() {
        base.onInit();

        updateIsLoading();

        updateUI();

        if (shopItem.type == ShopItemType.REWARD) {
            Async.call(updateTextPeriodically(), coroutineTagUpdateRewardText);
        }
    }

    protected override void onDeinit() {
        base.onDeinit();

        //stop text updates for rewards
        Async.cancel(coroutineTagUpdateRewardText);
    }

    protected IEnumerator updateTextPeriodically() {

        while (true) {
            
            updateIsLoading();
            updateLoadingAnim();

            updateTextDescription();

            yield return new WaitForSeconds(1);
        }
    }

    private void updateIsLoading() {
        
        isLoading = (
            shopItem.type == ShopItemType.LOADER || 
            GameHelper.Instance.getRewardsManager().isShopItemBlocked(shopItem)
        );
    }

    public void forceLoading() {

        isLoading = true;

        updateUI();
    }

    private void updateUI() {

        updateLoadingAnim();

        textNbHexacoins.text = "+" + shopItem.nbEarnedHexacoins.ToString();
        
        updateTextDescription();

        goImageButton.SetActive(true);
        textPrice.enabled = false;
        imageIcon.enabled = false;

        if (shopItem.type == ShopItemType.REWARD) {

            imageIcon.enabled = true;
            imageIcon.texture = GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_MENUS + shopItem.iconName);

        } else if (shopItem.type == ShopItemType.IAP) {

            textPrice.enabled = true;
            textPrice.text = shopItem.iapProduct.metadata.localizedPriceString;

        } else if (shopItem.type == ShopItemType.LOADER) {

            goImageButton.SetActive(false);

        } else {

            throw new NotSupportedException();
        }
    }

    private void updateLoadingAnim() {

        goLoadingAnim.SetActive(isLoading);
        textNbHexacoins.gameObject.SetActive(!isLoading);
    }

    private void updateTextDescription() {
        
        badge.setText(null);

        long remainingTimeSec = GameHelper.Instance.getRewardsManager().getShopItemBlockedRemainingTimeSec(shopItem);

        if (remainingTimeSec <= 0) {

            //show a badge on the rewards that are availables
            if (shopItem.type == ShopItemType.REWARD && !isLoading) {
                badge.setText("!");
            }

            //show the item text
            textDescription.text = shopItem.getDynamicText();

            return;
        }

        //else show the remaining time before item unblocking
        string textRemainingTime;

        if (remainingTimeSec < 60) {
            textRemainingTime = string.Format(Tr.get("Time.SEC"), remainingTimeSec);
        } else if (remainingTimeSec < 6000) {
            textRemainingTime = string.Format(Tr.get("Time.MIN"), (int) Math.Round(remainingTimeSec / 60f));
        } else {
            textRemainingTime = string.Format(Tr.get("Time.HOUR"), (int) Math.Round(remainingTimeSec / 3600f));
        }

        textDescription.text = string.Format(Tr.get("Activity23.Text.Reward.Wait"), textRemainingTime);
    }

}
