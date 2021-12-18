/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections.Generic;
using UnityEngine.UI;


public class IOSDeviceBehavior : ISpecificDeviceBehavior {
    

    bool ISpecificDeviceBehavior.isMobile() {
        return true;
    }

    bool ISpecificDeviceBehavior.isAndroid() {
        return false;
    }

    bool ISpecificDeviceBehavior.isIOS() {
        return true;
    }

    bool ISpecificDeviceBehavior.isDesktop() {
        return false;
    }

    void ISpecificDeviceBehavior.adaptScroll(ScrollRect scrollRect) {

        scrollRect.movementType = ScrollRect.MovementType.Elastic;
        scrollRect.elasticity = 0.12f;
        scrollRect.decelerationRate = 0.005f;
        scrollRect.scrollSensitivity = 5;
    }

    string ISpecificDeviceBehavior.getButtonLoginSpecificTitle() {
        return Tr.get("Activity30.Button.GameCenter");
    }

    string ISpecificDeviceBehavior.getButtonLoginSpecificIcon() {
        return "MenuButton.Login.IOS";
    }

    string ISpecificDeviceBehavior.getUrlStoreHexaSnap() {
        return Constants.URL_STORE_APPLE;
    }

    string ISpecificDeviceBehavior.getSpecificStoreText() {
        return Tr.get("Specific.Store.IOS");
    }

    string ISpecificDeviceBehavior.getButtonShareIcon() {
        return "MenuButton.Share.IOS";
    }

    string ISpecificDeviceBehavior.getAdMobBottomBannerId() {
        return Constants.AD_MOB_BOTTOM_BANNER_IOS;
    }

    string ISpecificDeviceBehavior.getAdMobRewardedAdsId() {
        return Constants.AD_MOB_REWARDED_ADS_IOS;
    }

}
