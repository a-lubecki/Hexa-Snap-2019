/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using UnityEngine.UI;


public interface ISpecificDeviceBehavior {

    bool isMobile();
    bool isAndroid();
    bool isIOS();
    bool isDesktop();

    string getUrlStoreHexaSnap();
    string getSpecificStoreText();

    void adaptScroll(ScrollRect scrollRect);

    string getButtonLoginSpecificTitle();
    string getButtonLoginSpecificIcon();
    string getButtonShareIcon();

    string getAdMobBottomBannerId();
    string getAdMobRewardedAdsId();

}