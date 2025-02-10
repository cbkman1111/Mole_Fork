using Common.Global.Singleton;
using UnityEngine;
using GoogleMobileAds.Api;
using Common.Utils;
using System;
using System.Collections.Generic;

namespace Common.Global
{
    public class AdMobManager : MonoSingleton<AdMobManager>
    {
        /// 광고단위/지면 ID
        private const string aosUnit = "ca-app-pub-1994103802600464/6773347431";
        private const string iosUnit = "ca-app-pub-1994103802600464/8992215030";

        /// 테스트 지면 ID
        private const string aosUnitTest = "ca-app-pub-3940256099942544/5224354917";
        private const string iosUnitTest = "ca-app-pub-3940256099942544/1712485313";

        private MEC.CoroutineHandle LoadAndShow;

        private RewardedInterstitialAd _rewardedInterstitialAd;
        public bool RewardAdReady { get; set; }
    
        public Action<bool> OnChangeAdState = null;
        // Start is called before the first frame update
        protected override bool Init()
        {
            Debug.Log($"{Tag} - init.");
#if UNITY_IOS
            MobileAds.SetiOSAppPauseOnBackground(true);
#endif
            MobileAds.Initialize(initStatus => {
                // Partner Mediation SDK 설정이 정상적으로 설정되었는지 체크
                var adapterStatusMap = initStatus.getAdapterStatusMap();

                foreach (var status in adapterStatusMap)
                {
                    if (status.Value.InitializationState == AdapterState.Ready)
                    {
                        RequestConfiguration requestConfiguration = new RequestConfiguration();
                        requestConfiguration.TestDeviceIds.Add("11a4ba74-d1da-491e-aa01-965a7ea7155a");
                        MobileAds.SetRequestConfiguration(requestConfiguration);

                        LoadRewardedAd();
                    }
                    else if (status.Value.InitializationState == AdapterState.NotReady)
                    {
                        GiantDebug.Log($"{status.Key} initialize fail reason : {status.Value.Description}");
                    }
                }
            });
            return true;
        }

        private void Start()
        {

        }

        /// <summary>
        /// Loads the ad.
        /// </summary>
        public void LoadRewardedAd()
        {
            // Clean up the old ad before loading a new one.
            if (_rewardedInterstitialAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading rewarded interstitial ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();
            
            SetReady(false);

            var unitID = GetUnitUI();
            // Send the request to load the ad.
            RewardedInterstitialAd.Load(unitID, adRequest,
                (RewardedInterstitialAd ad, LoadAdError error) =>
                {
                    // If the operation failed with a reason.
                    if (error != null)
                    {
                        Debug.LogError("Rewarded interstitial ad failed to load an ad with error : "
                                        + error);
                        return;
                    }
                    // If the operation failed for unknown reasons.
                    // This is an unexpexted error, please report this bug if it happens.
                    if (ad == null)
                    {
                        Debug.LogError("Unexpected error: Rewarded interstitial load event fired with null ad and null error.");
                        return;
                    }

                    // The operation completed successfully.
                    Debug.Log("Rewarded interstitial ad loaded with response : "
                        + ad.GetResponseInfo());
                    _rewardedInterstitialAd = ad;

                    // Register to ad events to extend functionality.
                    RegisterEventHandlers(ad);

                    // Inform the UI that the ad is ready.
                    SetReady(true);
                });
        }
        /// <summary>
        /// 광고 지면 ID 반환.
        /// </summary>
        /// <returns></returns>
        public string GetUnitUI()
        {
            string unitId = "unused";

#if UNITY_EDITOR
            unitId = "unused";
#elif UNITY_ANDROID
            unitId = aosUnitTest;
#elif UNITY_IOS
            unitId = iosUnitTest;
#endif

            GiantDebug.Log($"{Tag} - GetUnitUI : {unitId}");
            return unitId;
        }


        private void SetReady(bool ready)
        {
            RewardAdReady = ready;
            OnChangeAdState?.Invoke(RewardAdReady);
        }

        /// <summary>
        /// Shows the ad.
        /// </summary>
        public void ShowRewardVideo(System.Action onComplete, System.Action onFailed, long adRewardGroupID, int adRewardStep, string placementName = "")
        {
            if (_rewardedInterstitialAd != null && _rewardedInterstitialAd.CanShowAd())
            {
                _rewardedInterstitialAd.Show((Reward reward) =>
                {
                    GiantDebug.Log("Rewarded interstitial ad rewarded : " + reward.Amount);
                });
            }
            else
            {
                GiantDebug.Log("준비되지 않아 2회차 재생 시도.");  
                if (LoadAndShow.IsRunning == false)
                {
                    LoadAndShow = MEC.Timing.RunCoroutine(LoadAndShowAd(onComplete, onFailed));
                }
            }

            // Inform the UI that the ad is not ready.
            SetReady(false);
        }

        private IEnumerator<float> LoadAndShowAd(System.Action onComplete, System.Action onFailed)
        {
            yield return MEC.Timing.WaitForOneFrame;

            DestroyAd();
            LoadRewardedAd();

            float time = 3.0f;
            while (true)
            {
                if (_rewardedInterstitialAd != null)
                {
                    break;
                }

                time -= Time.deltaTime;
                if(time <= 0)
                {
                    yield break;
                }

                yield return MEC.Timing.WaitForSeconds(0.5f);
            }

            if (_rewardedInterstitialAd != null)
            {
                _rewardedInterstitialAd.Show((Reward reward) =>
                {
                    GiantDebug.Log($"{Tag} - Rewarded ad rewarded the user. Type: {reward.Type}, amount: {reward.Amount}.");
                    onComplete?.Invoke();
                });
            }
            else
            {
                onFailed?.Invoke();
            }
        }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        public void DestroyAd()
        {
            LoadAndShow.IsRunning = false;

            if (_rewardedInterstitialAd != null)
            {
                Debug.Log("Destroying rewarded interstitial ad.");
                _rewardedInterstitialAd.Destroy();
                _rewardedInterstitialAd = null;
            }

            // Inform the UI that the ad is not ready.
            SetReady(false);
        }

        /// <summary>
        /// Logs the ResponseInfo.
        /// </summary>
        public void LogResponseInfo()
        {
            if (_rewardedInterstitialAd != null)
            {
                var responseInfo = _rewardedInterstitialAd.GetResponseInfo();
                UnityEngine.Debug.Log(responseInfo);
            }
        }

        protected void RegisterEventHandlers(RewardedInterstitialAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(string.Format("Rewarded interstitial ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Rewarded interstitial ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("Rewarded interstitial ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Rewarded interstitial ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded interstitial ad full screen content closed.");
                LoadRewardedAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Rewarded interstitial ad failed to open full screen content" +
                               " with error : " + error);
            };
        }
    }
}