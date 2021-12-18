/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine;
using UnityEngine.UI;


public class SpecificDeviceManager : ISpecificDeviceBehavior {


    //singleton
    public static readonly SpecificDeviceManager Instance = new SpecificDeviceManager();


    private ISpecificDeviceBehavior behavior;


    private SpecificDeviceManager() {
        
        #if UNITY_ANDROID
        behavior = new AndroidDeviceBehavior();
        #elif UNITY_IPHONE
        behavior = new IOSDeviceBehavior();
        #else
        behavior = new DesktopDeviceBehavior();
        #endif
    }

    public bool isMobile() {
        return behavior.isMobile();
    }

    public bool isTablet() {
        return isMobile() && (Screen.width / Screen.dpi) >= 3.5f;
    }

    public bool isAndroid() {
        return behavior.isAndroid();
    }

    public bool isIOS() {
        return behavior.isIOS();
    }

    public bool isDesktop() {
        return behavior.isDesktop();
    }

    public void adaptScroll(ScrollRect scrollRect) {
        behavior.adaptScroll(scrollRect);
    }

    public string getButtonLoginSpecificTitle() {
        return behavior.getButtonLoginSpecificTitle();
    }

    public string getButtonLoginSpecificIcon() {
        return behavior.getButtonLoginSpecificIcon();
    }

    public string getUrlStoreHexaSnap() {
        return behavior.getUrlStoreHexaSnap();
    }

    public string getSpecificStoreText() {
        return behavior.getSpecificStoreText();
    }

    public string getButtonShareIcon() {
        return behavior.getButtonShareIcon();
    }

    public string getAdMobBottomBannerId() {
        return behavior.getAdMobBottomBannerId();
    }

    public string getAdMobRewardedAdsId() {
        return behavior.getAdMobRewardedAdsId();
    }

}
