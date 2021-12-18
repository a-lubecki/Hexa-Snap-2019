/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using Facebook.Unity;


public class RewardsManager {


    private Dictionary<ShopItem, DateTime> shopItemsUnblockDateTime = new Dictionary<ShopItem, DateTime>();

    //diff between the device and the server
    public bool hasRetrievedTimeShift { get; private set; }

    //the time shift between the device and the server, used for rewards unblock time checking
    public long timeShiftSec { get; private set; }


    public void init(Dictionary<ShopItem, DateTime> shopItemsUnblockDateTime) {

        if (shopItemsUnblockDateTime == null) {
            this.shopItemsUnblockDateTime = new Dictionary<ShopItem, DateTime>();
        } else {
            this.shopItemsUnblockDateTime = shopItemsUnblockDateTime;
        }
    }

    public Dictionary<ShopItem, DateTime> getShopItemsUnblockDateTime() {
        //defensive copy
        return new Dictionary<ShopItem, DateTime>(shopItemsUnblockDateTime);
    }

    public void retrieveDateTimeShift(ShopItem item, Action<bool> onDone, Action onError) {

        if (hasRetrievedTimeShift) {
            //already retrieved
            endRetrieveDateTimeShift(item, onDone);
            return;
        }

        if (!FirebaseInitManager.instance.hasResolvedDependencies()) {
            onError();
            return;
        }

        //retrieve the datetime difference on the server to know if the player wants to cheat and has changed the time
        FirebaseFunctionsManager.instance.retrieveTimeShift(shiftSec => {

            if (Math.Abs(shiftSec) < 60) {
                //if diff is less than one min, we consider the player doesn't want to cheat
                shiftSec = 0;
            }

            //update timeshift in purchase manager will update times in the scrollview
            setRetrievedTimeshift(shiftSec);

            endRetrieveDateTimeShift(item, onDone);

        }, (error) => {

            onError();
        });
    }

    private void endRetrieveDateTimeShift(ShopItem item, Action<bool> completion) {
        
        //calculate the time is well elapsed to ensure the player has not cheated
        var canCollectReward = (hasRetrievedTimeShift && !isShopItemBlocked(item));
        completion(canCollectReward);
    }

    public void onRewardCollected(ShopItem item, string activityName) {

        //determine the tracking tag of the reward to track it in addHexacoins(...)
        string rewardTrackingTag = "unknown";
        if (item == ShopItem.REWARD_DAILY) {
            rewardTrackingTag = T.Value.EARN_REASON_REWARD_DAILY;
        } else if (item == ShopItem.REWARD_AD_VIDEO) {
            rewardTrackingTag = T.Value.EARN_REASON_REWARD_VIDEO;
        }

        GameHelper.Instance.getGameManager().addHexacoins(item.nbEarnedHexacoins, activityName, rewardTrackingTag);

        //block item for an amount of minutes/hours
        if (item.nbBlockingMinutes > 0) {
            setShopItemBlocked(item);
        }

        TrackingManager.instance.prepareEvent(T.Event.REWARD_COLLECTED)
                       .add(T.Param.ID, item.tag)
                       .add(T.Param.NB_HEXACOINS, item.nbEarnedHexacoins)
                       .track();
    }

    public void setShopItemBlocked(ShopItem shopItem) {

        //block for some time from now (remove time shift to have the real time)
        shopItemsUnblockDateTime[shopItem] = DateTime.Now.ToUniversalTime().AddSeconds(-timeShiftSec).AddMinutes(shopItem.nbBlockingMinutes);

        SaveManager.Instance.saveShopItems();
    }

    public bool isShopItemBlocked(ShopItem shopItem) {
        return getShopItemBlockedRemainingTimeSec(shopItem) > 0;
    }

    public long getShopItemBlockedRemainingTimeSec(ShopItem shopItem) {

        if (!shopItemsUnblockDateTime.ContainsKey(shopItem)) {
            return 0;
        }

        var timeSec = shopItemsUnblockDateTime[shopItem].TotalSeconds() - (DateTime.Now.ToUniversalTime().TotalSeconds() - timeShiftSec);

        //if elapsed, remove the item as it's not necessary any more
        if (hasRetrievedTimeShift && timeSec <= 0) {

            timeSec = 0;

            shopItemsUnblockDateTime.Remove(shopItem);

            SaveManager.Instance.saveShopItems();
        }

        return timeSec;
    }

    public void setRetrievedTimeshift(long timeShiftSec) {

        hasRetrievedTimeShift = true;

        this.timeShiftSec = timeShiftSec;
    }

    public void clearTimeshift() {

        hasRetrievedTimeShift = false;

        timeShiftSec = 0;
    }

}
