using SweetSugar.Scripts.Core;
using UnityEngine;
using UnityEngine.Advertisements;

#if UNITY_ADS
namespace SweetSugar.Scripts.AdsEvents
{
    public class UnityAdsController : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener,
        IUnityAdsShowListener
    {
        #region UnityAdsController

        public static UnityAdsController Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void InitAds()
        {
#if UNITY_ANDROID
            Advertisement.Initialize(AdsManager.THIS.androidID, false, this);
#elif UNITY_IOS
            Advertisement.Initialize(AdsManager.THIS.iOSID, false, this);
#endif
        }

        public void ShowAds(string loadAdType)
        {
            Advertisement.Show(loadAdType, this);
        }

        public void LoadUnityRewardedAd()
        {
            string videoAdsZone = (Application.platform == RuntimePlatform.IPhonePlayer)
                ? AdsManager.THIS.unityRewardedIOS
                : AdsManager.THIS.unityRewardedAndroid;
            Advertisement.Load(videoAdsZone, this);
        }

        public void LoadUnityInterstitialAd()
        {
            string interstitialAdsZone =
                (Application.platform == RuntimePlatform.IPhonePlayer)
                    ? AdsManager.THIS.unityInterstitialIOS
                    : AdsManager.THIS.unityInterstitialAndroid;
            Advertisement.Load(interstitialAdsZone, this);
        }

        public void OnInitializationComplete()
        {
            Debug.Log("OnInitializationComplete!");
            LoadUnityRewardedAd();
            LoadUnityInterstitialAd();
        }

        public bool isLoaded;

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            isLoaded = false;
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            isLoaded = true;
            Debug.Log("OnUnityAdsAdLoaded!  placementId = " + placementId);
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            isLoaded = false;
            Debug.Log($"Error showing Ad Unit {placementId}: {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            isLoaded = false;
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            Debug.Log("OnUnityAdsShowStart!  placementId = " + placementId);
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Debug.Log("OnUnityAdsShowClick!  placementId = " + placementId);
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            isLoaded = false;
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                Debug.Log("OnUnityAdsShowComplete!  placementId = " + placementId);
                AdsManager._OnRewardedShown();
                InitScript.Instance.ShowReward();
            }
        }

        #endregion
    }
}
#endif