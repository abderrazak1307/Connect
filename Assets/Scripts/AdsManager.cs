using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public const string APP_KEY = "1b03fbdbd";
    public bool showAds = false;
    public GameObject NoAdsButton;
    public static bool CanLoadNextScene = false;
    public static int AdsCounter = 0;
    void Start(){
        CanLoadNextScene = false;
        if(PlayerPrefs.GetInt("NoAds", 0) == 1 && NoAdsButton)
            NoAdsButton.SetActive(false);

        if(PlayerPrefs.GetInt("NoAds", 0) == 0){
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
            // Interstitials
            IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;

            // Rewarded ads
            //IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedOnAdClosedEvent;
            //IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;

            IronSource.Agent.init(APP_KEY);
            if(showAds)
                IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
            IronSource.Agent.loadInterstitial();
            //IronSource.Agent.loadRewardedVideo();
        }
    }
    void OnApplicationPause(bool isPaused){
        IronSource.Agent.onApplicationPause(isPaused);
    }
    private void SdkInitializationCompletedEvent() {
        Debug.Log("SDK Initialised");
        IronSource.Agent.validateIntegration();
    }
    private void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo){
        IronSource.Agent.loadInterstitial();
        CanLoadNextScene = true;
        AdsCounter = 0;
    }
    private void RewardedOnAdClosedEvent(IronSourceAdInfo adInfo){
        IronSource.Agent.loadRewardedVideo();
    }
    public static void ShowInterstitial(){
        Debug.Log("ShowInterstitial called, AdsCounter : "+AdsCounter);
        if(AdsCounter < 3)
            AdsCounter++;
        else
            if (IronSource.Agent.isInterstitialReady())
                IronSource.Agent.showInterstitial();
    }
    public static void ShowRewardedAd(){
        if(IronSource.Agent.isRewardedVideoAvailable()) IronSource.Agent.showRewardedVideo();
    }
    public void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        string placementName = placement.getPlacementName();
        string rewardName = placement.getRewardName();
        int rewardAmount = placement.getRewardAmount();
        Debug.Log(placementName + ": " + rewardAmount + " " +rewardName);
    }
    public void BuyNoAds(){
        PlayerPrefs.SetInt("NoAds", 1);
    }
}
