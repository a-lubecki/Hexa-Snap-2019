/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections.Generic;
using UnityEngine.UI;


public class AndroidDeviceBehavior : ISpecificDeviceBehavior {


    bool ISpecificDeviceBehavior.isMobile() {
        return true;
    }

    bool ISpecificDeviceBehavior.isAndroid() {
        return true;
    }

    bool ISpecificDeviceBehavior.isIOS() {
        return false;
    }

    bool ISpecificDeviceBehavior.isDesktop() {
        return false;
    }

    void ISpecificDeviceBehavior.adaptScroll(ScrollRect scrollRect) {

        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.decelerationRate = 0.01f;
        scrollRect.scrollSensitivity = 1000;
    }

    string ISpecificDeviceBehavior.getButtonLoginSpecificTitle() {
        return Tr.get("Activity30.Button.GooglePlay");
    }

    string ISpecificDeviceBehavior.getButtonLoginSpecificIcon() {
        return "MenuButton.Login.ANDROID";
    }

    string ISpecificDeviceBehavior.getUrlStoreHexaSnap() {
        return Constants.URL_STORE_GOOGLE;
    }

    string ISpecificDeviceBehavior.getSpecificStoreText() {
        return Tr.get("Specific.Store.ANDROID");
    }

    string ISpecificDeviceBehavior.getButtonShareIcon() {
        return "MenuButton.Share.ANDROID";
    }

    string ISpecificDeviceBehavior.getAdMobBottomBannerId() {
        return Constants.AD_MOB_BOTTOM_BANNER_ANDROID;
    }

    string ISpecificDeviceBehavior.getAdMobRewardedAdsId() {
        return Constants.AD_MOB_REWARDED_ADS_ANDROID;
    }

}
