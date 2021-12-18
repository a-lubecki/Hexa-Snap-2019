/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using UnityEngine;
using GoogleMobileAds.Api;


public class AdsManager : MonoBehaviour {


    private BannerView bannerView;
    private RewardBasedVideoAd rewardBasedVideo;

    private Action onRewardFailed;
    private Action onRewardClosed;
    private Action onRewardDone;

    public string lastRewardFailMessage { get; private set; }


    protected void Start() {

        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.Initialize(initStatus => { });
    }

    protected void OnApplicationQuit() {

        //free memory
        if (bannerView != null) {
            bannerView.Destroy();
            bannerView = null;
        }
    }

    public void showAdBanner() {

        if (GameHelper.Instance.getGameManager().hasRemovedAds) {
            //can't show ads if the user paid to remove them
            return;
        }

        if (bannerView == null) {

            //create the banner
            string adUnitId;

            if (Debug.isDebugBuild) {
                adUnitId = Constants.AD_MOB_BOTTOM_BANNER_TEST;
            } else {
                adUnitId = SpecificDeviceManager.Instance.getAdMobBottomBannerId();
            }

            if (adUnitId == null) {
                return;
            }

            bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
            bannerView.LoadAd(createAdRequest());
        }

        bannerView.Show();
    }

    public void hideAdBanner() {

        if (bannerView == null) {
            //not created yet
            return;
        }

        bannerView.Hide();
    }

    public bool loadRewardVideoOnce() {

        if (GameHelper.Instance.getGameManager().hasRemovedAds) {
            //can't show ads if the user paid to remove them
            return false;
        }

        if (rewardBasedVideo != null) {
            //already loaded once
            return true;
        }

        string adUnitId;

        if (Debug.isDebugBuild) {
            adUnitId = Constants.AD_MOB_REWARDED_ADS_TEST;
        } else {
            adUnitId = SpecificDeviceManager.Instance.getAdMobRewardedAdsId();
        }

        if (adUnitId == null) {
            return false;
        }

        lastRewardFailMessage = "";

        rewardBasedVideo = RewardBasedVideoAd.Instance;
        rewardBasedVideo.LoadAd(createAdRequest(), adUnitId);

        rewardBasedVideo.OnAdLoaded += onRewardedAdLoaded;
        rewardBasedVideo.OnAdFailedToLoad += onRewardedAdFailedToLoad;
        rewardBasedVideo.OnAdClosed += onRewardedAdClosed;
        rewardBasedVideo.OnAdRewarded += onRewardedAdDone;

        return true;
    }

    public void showRewardedAd(Action onDone, Action onFailed, Action onClosed) {

        if (GameHelper.Instance.getGameManager().hasRemovedAds) {
            //can't show ads if the user paid to remove them
            onDone?.Invoke();
            return;
        }

        if (!loadRewardVideoOnce()) {
            onFailed?.Invoke();
            return;
        }

        onRewardFailed = onFailed;
        onRewardClosed = onClosed;
        onRewardDone = onDone;

        //show video only if loaded
        tryShowVideo();
    }

    protected void onRewardedAdLoaded(object sender, EventArgs args) {

        //show video if loaded after showing requested
        tryShowVideo();
    }

    private void tryShowVideo() {

        if (rewardBasedVideo == null) {
            //no video to show
            return;
        }

        if (!rewardBasedVideo.IsLoaded()) {
            //video to show but pending, will be shown the next time
            return;
        }

        if (onRewardDone == null) {
            //no listener to manage the reward of the player
            return;
        }

        rewardBasedVideo.Show();
    }

    protected void onRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {

        //cancel load and prepare for a new retry later
        rewardBasedVideo = null;

        lastRewardFailMessage = args.Message;

        callListener(onRewardFailed);
    }

    protected void onRewardedAdClosed(object sender, EventArgs args) {

        //cancel load and prepare for a new retry later
        rewardBasedVideo = null;

        callListener(onRewardClosed);
    }

    protected void onRewardedAdDone(object sender, Reward args) {

        callListener(onRewardDone);
    }

    private void callListener(Action listener) {

        clearListeners();

        //invoke listener on main thread to avoid crash
        if (listener != null) {
            DispatchQueue.Instance.Invoke(listener);
        }
    }

    private void clearListeners() {

        onRewardFailed = null;
        onRewardClosed = null;
        onRewardDone = null;
    }


    private AdRequest createAdRequest() {

        return new AdRequest.Builder()
                            .AddExtra("npa", "1") //remove ads personalization (GDPR)
                            .AddTestDevice(AdRequest.TestDeviceSimulator)
                            .AddTestDevice("XXXXXX")
                            .AddTestDevice("XXXXXX")
                            .AddTestDevice("XXXXXX")
                            .AddTestDevice("XXXXXX")
                            .AddTestDevice("XXXXXX")
                            .Build();
    }

}
