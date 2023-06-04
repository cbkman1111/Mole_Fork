#if CHARTBOOST_ADS
using ChartboostSDK;
#endif
using System;
using System.Collections.Generic;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Integrations;
using UnityEngine;
using UnityEngine.Events;
#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
using SweetSugar.Scripts.AdsEvents.GoogleRewardedAds;
#endif
#if UNITY_ADS
using UnityEngine.Advertisements;

#endif

namespace SweetSugar.Scripts.AdsEvents
{
    /// <summary>
    /// Ads manager responsible for initialization and showing
    /// </summary>
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager THIS;

        //EDITOR: ads events
        public List<AdEvents> adsEvents = new List<AdEvents>();

        //is unity ads enabled
        public bool enableUnityAds;

        //is admob enabled
        public bool enableGoogleMobileAds;

        //is chartboost enabled
        public bool enableChartboostAds;

        //rewarded zone for Unity ads
        public string rewardedVideoZone;
        //admob stuff
#if GOOGLE_MOBILE_ADS
        public InterstitialAd interstitial;
        private AdRequest requestAdmob;
#endif
        public string admobUIDAndroid;
        public string admobRewardedUIDAndroid;
        public string admobUIDIOS;
        public string admobRewardedUIDIOS;

        [Space(20)] public string androidID;
        public string iOSID;
        public string unityRewardedAndroid;
        public string unityInterstitialAndroid;
        public string unityRewardedIOS;
        public string unityInterstitialIOS;

        private AdManagerScriptable adsSettings;
        private UnityAdsID unityAds;
        
        int npaValue = -1;
        
        public UnityEvent OnAdLoadedEvent;
        public UnityEvent OnAdFailedToLoadEvent;
        public UnityEvent OnAdOpeningEvent;
        public UnityEvent OnAdFailedToShowEvent;
        public UnityEvent OnUserEarnedRewardEvent;
        public UnityEvent OnAdClosedEvent;

        private void Awake()
        {
            if (THIS == null) THIS = this;
            else if (THIS != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(this);
            npaValue = PlayerPrefs.GetInt ("npa", 0);
            adsSettings = Resources.Load<AdManagerScriptable>("Scriptable/AdManagerScriptable");
            adsEvents = adsSettings.adsEvents;
            admobUIDAndroid = adsSettings.admobUIDAndroid;
            admobUIDIOS = adsSettings.admobUIDIOS;
            admobRewardedUIDAndroid = adsSettings.admobRewardedUIDAndroid;
            admobRewardedUIDIOS = adsSettings.admobRewardedUIDIOS;
#if UNITY_ADS //2.1.1
            gameObject.AddComponent<UnityAdsController>();
            enableUnityAds = true;
            unityAds = Resources.Load<UnityAdsID>("Scriptable/UnityAdsID");
            androidID = unityAds.androidID;
            iOSID = unityAds.iOSID;
            unityInterstitialAndroid = unityAds.unityInterstitialAndroid;
            unityInterstitialIOS = unityAds.unityInterstitialiOS;
            unityRewardedAndroid = unityAds.unityRewardedAndroid;
            unityRewardedIOS = unityAds.unityRewardediOS;
#if UNITY_ANDROID || UNITY_IOS
            UnityAdsController.Instance.InitAds();
#endif
#else
            enableUnityAds = false;
#endif

#if CHARTBOOST_ADS //1.6.1
		enableChartboostAds = true;
#else
            enableChartboostAds = false;
#endif
#if GOOGLE_MOBILE_ADS
            enableGoogleMobileAds = true; //1.6.1
#if UNITY_ANDROID
            MobileAds.Initialize(initStatus => { }); //2.1.6
            // When true all events raised by GoogleMobileAds will be raised
            // on the Unity main thread. The default value is false.
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            //interstitial = new InterstitialAd(admobUIDAndroid);
            LoadInterstitialAd(admobUIDAndroid);
#elif UNITY_IOS
        MobileAds.Initialize(admobUIDIOS);//2.1.6
        //interstitial = new InterstitialAd(admobUIDIOS);
             LoadInterstitialAd(admobUIDIOS);
#else
        MobileAds.Initialize(admobUIDAndroid);//2.1.6
		//interstitial = new InterstitialAd (admobUIDAndroid);
             LoadInterstitialAd(admobUIDAndroid);
#endif

            // Create an empty ad request.
            requestAdmob = new AdRequest.Builder().AddExtra ("npa", npaValue.ToString ()).Build();
            /*// Load the interstitial with the request.
            interstitial.LoadAd(requestAdmob);
            interstitial.OnAdLoaded += HandleInterstitialLoaded;
            interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;*/
#else
            enableGoogleMobileAds = false; //1.6.1
#endif
        }
#if GOOGLE_MOBILE_ADS

        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder().AddExtra ("npa", npaValue.ToString ()).Build();
        }
        
        public void LoadInterstitialAd(string _adUnitId)
        {
            // Clean up the old ad before loading a new one.
            if (interstitial != null)
            {
                interstitial.Destroy();
                interstitial = null;
            }

            Debug.Log("Loading the interstitial ad.");
            
            // send the request to load the ad.
            // Load an interstitial ad
        InterstitialAd.Load(_adUnitId, CreateAdRequest(),
            (InterstitialAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Debug.Log("Interstitial ad failed to load with error: " +
                              loadError.GetMessage());
                    return;
                }
                else if (ad == null)
                {
                    Debug.Log("Interstitial ad failed to load.");
                    return;
                }

                Debug.Log("Interstitial ad loaded.");
                interstitial = ad;

                ad.OnAdFullScreenContentOpened += () =>
                {
                    Debug.Log("Interstitial ad opening.");
                    OnAdOpeningEvent.Invoke();
                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("Interstitial ad closed.");
                    OnAdClosedEvent.Invoke();
                };
                ad.OnAdImpressionRecorded += () =>
                {
                    Debug.Log("Interstitial ad recorded an impression.");
                };
                ad.OnAdClicked += () =>
                {
                    Debug.Log("Interstitial ad recorded a click.");
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Debug.Log("Interstitial ad failed to show with error: " +
                              error.GetMessage());
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "Interstitial ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);
                    Debug.Log(msg);
                };
            });
        }
#endif
#if GOOGLE_MOBILE_ADS
        public void HandleInterstitialLoaded(object sender, EventArgs args)
        {
            print("HandleInterstitialLoaded event received.");
        }

        public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            print("HandleInterstitialFailedToLoad event received with message: " + args.LoadAdError);
        }
#endif

        public bool GetRewardedUnityAdsReady()
        {
#if APPODEAL
        return AppodealIntegration.THIS.IsRewardedLoaded();
#endif
#if UNITY_ADS

            return UnityAdsController.Instance.isLoaded;

            //return Advertisement.IsReady(rewardedVideoZone);
#endif
#if GOOGLE_MOBILE_ADS
            return RewAdmobManager.THIS.IsRewardedAdIsLoaded();
#endif
            return false;
        }

        private void OnDisable()
        {
#if GOOGLE_MOBILE_ADS
            if (interstitial != null)
            {
                interstitial.Destroy();
                interstitial = null;
            }

#endif
        }

        public delegate void RewardedShown();

        public static event RewardedShown OnRewardedShown;

        public void ShowRewardedAds()
        {
#if APPODEAL
        Debug.Log("show Rewarded ads video in " + LevelManager.THIS.gameStatus);

        if (GetRewardedUnityAdsReady())
        {
            AppodealIntegration.THIS.ShowRewardedAds();
        }
        else{
            #if UNITY_ADS
            Advertisement.Show(rewardedVideoZone, new ShowOptions
            {
                resultCallback = result =>
                {
                    if (result == ShowResult.Finished)
                    {
                        OnRewardedShown?.Invoke();
                        InitScript.Instance.ShowReward();
                    }
                }
            });
            #endif
        }
#elif UNITY_ADS
            Debug.Log("show Rewarded ads video in " + LevelManager.THIS.gameStatus);
            ShowVideo();

#elif GOOGLE_MOBILE_ADS //2.2
            bool stillShow = true;

#if UNITY_ADS
        stillShow = !GetRewardedUnityAdsReady ();
#endif
            if (stillShow)
                RewAdmobManager.THIS.ShowRewardedAd(InitScript.Instance.ShowReward);
#endif
        }

        public void CheckAdsEvents(GameState state)
        {
            foreach (var item in adsEvents)
            {
                if (item.gameEvent == state)
                {
                    item.calls++;
                    if (item.calls % item.everyLevel == 0)
                        ShowAdByType(item.adType);
                }
            }
        }

        void ShowAdByType(AdType adType)
        {
            if (adType == AdType.AdmobInterstitial && enableGoogleMobileAds)
                ShowAds(false);
            else if (adType == AdType.UnityAdsInterstitial && enableUnityAds)
                ShowVideo(true);
            else if (adType == AdType.UnityAdsVideo && enableUnityAds)
                ShowVideo(false);
            else if (adType == AdType.ChartboostInterstitial && enableChartboostAds)
                ShowAds(true);
            else if (adType == AdType.Appodeal)
                ShowAds(false);
        }

        public void ShowVideo(bool isInterstitial = false)
        {
#if UNITY_ADS
            Debug.Log("show Unity ads in " + LevelManager.THIS.gameStatus);

            if (!isInterstitial)
            {
#if UNITY_ANDROID
                UnityAdsController.Instance.ShowAds(unityRewardedAndroid);
#elif UNITY_IOS
                UnityAdsController.Instance.ShowAds(unityRewardedIOS);
#endif
            }
            else
            {
#if UNITY_ANDROID
                UnityAdsController.Instance.ShowAds(unityInterstitialAndroid);
#elif UNITY_IOS
                UnityAdsController.Instance.ShowAds(unityInterstitialIOS);
#endif
            }

#elif GOOGLE_MOBILE_ADS
            Debug.Log("show Admob rewarded video ads in " + LevelManager.THIS.gameStatus);
#endif
        }

        public void CacheRewarded()
        {
#if APPODEAL
        AppodealIntegration.THIS.RequestRewarded();
#endif
        }


        public void ShowAds(bool chartboost = true)
        {
#if APPODEAL
        if(AppodealIntegration.THIS.IsInterstitialLoaded())
        {
            Debug.Log("show  Interstitial in " + LevelManager.THIS.gameStatus);
            AppodealIntegration.THIS.ShowInterstitial();
        }
#endif
            if (chartboost)
            {
#if CHARTBOOST_ADS
            Debug.Log("show Chartboost Interstitial in " + LevelManager.THIS.gameStatus);

            Chartboost.showInterstitial(CBLocation.Default);
            Chartboost.cacheInterstitial(CBLocation.Default);
#endif
            }
            else
            {
#if GOOGLE_MOBILE_ADS
                Debug.Log("show admob Interstitial in " + LevelManager.THIS.gameStatus);
                if (interstitial != null && interstitial.CanShowAd())
                {
                    Debug.Log("Showing interstitial ad.");
                    interstitial.Show();
#if UNITY_ANDROID
                    //interstitial = new InterstitialAd(admobUIDAndroid);
                    LoadInterstitialAd(admobUIDAndroid);
#elif UNITY_IOS
               // interstitial = new InterstitialAd(admobUIDIOS);
                     LoadInterstitialAd(admobUIDIOS);
#else
			// interstitial = new InterstitialAd (admobUIDAndroid);
                    LoadInterstitialAd(admobUIDAndroid);
#endif
                }
                else
                {
                    Debug.LogError("Interstitial ad is not ready yet.");
                }
#endif
            }
        }

        public static void _OnRewardedShown()
        {
            OnRewardedShown?.Invoke();
        }
    }
}