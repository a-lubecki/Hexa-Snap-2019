/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections.Generic;
using UnityEngine.UI;


public class DesktopDeviceBehavior : ISpecificDeviceBehavior {


    bool ISpecificDeviceBehavior.isMobile() {
        return false;
    }

    bool ISpecificDeviceBehavior.isAndroid() {
        return false;
    }

    bool ISpecificDeviceBehavior.isIOS() {
        return false;
    }

    bool ISpecificDeviceBehavior.isDesktop() {
        return true;
    }

    void ISpecificDeviceBehavior.adaptScroll(ScrollRect scrollRect) {

        scrollRect.movementType = ScrollRect.MovementType.Elastic;
        scrollRect.elasticity = 0.12f;
        scrollRect.decelerationRate = 0.005f;
        scrollRect.scrollSensitivity = 5;
    }

    string ISpecificDeviceBehavior.getButtonLoginSpecificTitle() {
        throw new ArgumentException("Button not managed");
    }

    string ISpecificDeviceBehavior.getButtonLoginSpecificIcon() {
        throw new ArgumentException("Button not managed");
    }

    string ISpecificDeviceBehavior.getUrlStoreHexaSnap() {
        return Constants.URL_STORE_STEAM;
    }

    string ISpecificDeviceBehavior.getSpecificStoreText() {
        throw new NotSupportedException("Text not managed");
    }

    string ISpecificDeviceBehavior.getButtonShareIcon() {
        throw new ArgumentException("Not managed");
    }

    string ISpecificDeviceBehavior.getAdMobBottomBannerId() {
        return null;
    }

    string ISpecificDeviceBehavior.getAdMobRewardedAdsId() {
        return null;
    }

}
