using System;
using UnityEngine;
using UnityEngine.Events;
#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
using SweetSugar.Scripts.AdsEvents;
#endif

//2.2
namespace SweetSugar.Scripts.AdsEvents.GoogleRewardedAds
{
    public class RewAdmobManager : MonoBehaviour
    {
        public static RewAdmobManager THIS;
        #if GOOGLE_MOBILE_ADS
    private RewardedAd rewardBasedVideo;
    private Action resultCallback;

    public UnityEvent OnAdFailedToLoadEvent;
    public UnityEvent OnAdOpeningEvent;
    public UnityEvent OnAdFailedToShowEvent;
    public UnityEvent OnUserEarnedRewardEvent;
    public UnityEvent OnAdClosedEvent;
    
    int npaValue = -1;

    private void Awake()
    {
        if (THIS == null)
            THIS = this;
        else if (THIS != this)
            Destroy(gameObject);
        DontDestroyOnLoad(this);
    }

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder().AddExtra ("npa", npaValue.ToString ()).Build();
    }
    
    public void Start()
    {
        npaValue = PlayerPrefs.GetInt ("npa", 0);
        RequestRewardBasedVideo();
    }

    private void RequestRewardBasedVideo()
    {
#if UNITY_ANDROID
        string adUnitId = AdsManager.THIS.admobRewardedUIDAndroid;
#elif UNITY_IPHONE
        string adUnitId = AdsManager.THIS.admobRewardedUIDIOS;
#else
            string adUnitId = "unexpected_platform";
#endif
        
          // create new rewarded ad instance
        RewardedAd.Load(adUnitId, CreateAdRequest(),
            (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Debug.Log("Rewarded ad failed to load with error: " +
                                loadError.GetMessage());
                    return;
                }
                else if (ad == null)
                {
                    Debug.Log("Rewarded ad failed to load.");
                    return;
                }

                Debug.Log("Rewarded ad loaded.");
                rewardBasedVideo = ad;

                ad.OnAdFullScreenContentOpened += () =>
                {
                    Debug.Log("Rewarded ad opening.");
                    OnAdOpeningEvent?.Invoke();
                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("Rewarded ad closed.");
                    OnAdClosedEvent?.Invoke();
                };
                ad.OnAdImpressionRecorded += () =>
                {
                    Debug.Log("Rewarded ad recorded an impression.");
                };
                ad.OnAdClicked += () =>
                {
                    Debug.Log("Rewarded ad recorded a click.");
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Debug.Log("Rewarded ad failed to show with error: " +
                              error.GetMessage());
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "Rewarded ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);
                    Debug.Log(msg);
                };
            });
    }

    public bool IsRewardedAdIsLoaded()
    {
        return rewardBasedVideo.CanShowAd();
    }

    public void ShowRewardedAd(Action resultCallback)
    {
        this.resultCallback = resultCallback;
        if (rewardBasedVideo == null)
        {
            Debug.Log("Rewarded ad is not ready yet.");
            RequestRewardBasedVideo();
        }
        
        if (rewardBasedVideo != null)
        {
            rewardBasedVideo.Show((Reward reward) =>
            {
                Debug.Log("Rewarded ad granted a reward: " + reward.Amount);
            });
        }
    }

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardBasedVideoFailedToLoad event received with message: "
                             + args.LoadAdError);
        this.RequestRewardBasedVideo();

    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoClosed event received");
        this.RequestRewardBasedVideo();
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardBasedVideoRewarded event received for "
                        + amount.ToString() + " " + type);
        if (resultCallback != null) resultCallback();
        this.RequestRewardBasedVideo();

    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
    }
        #endif
    }
}
