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
            SetGDPRConsent(true);
            SetCCPADoNotSell(true);

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

        /// <summary>
        /// GDPR 동의 상태 설정.
        /// GDPR(General Data Protection Regulation)은 유럽연합(EU)의 개인정보 보호법
        /// https://www.cloudflare.com/ko-kr/learning/privacy/what-is-the-gdpr/
        /// 
        /// Google EU 사용자 동의 정책에 따라 개발자는 기기 식별자 및 개인 정보 사용과 관련하여 유럽 경제 지역 
        /// (EEA) 사용자에게 특정 정보를 공개하고 동의를 얻어야 합니다. 
        /// 이 정책에는 EU 온라인 개인 정보 보호 지침 및 개인 정보 보호법 (GDPR)의 요구사항이 반영되어 있습니다. 
        /// 동의를 얻으려면 개인 정보를 수집, 수신 또는 사용할 수 있는 미디에이션 체인의 각 광고 네트워크를 식별하고 
        /// 각 네트워크의 사용에 관한 정보를 제공해야 합니다. 
        /// 
        /// 현재 Google은 이러한 네트워크에 사용자의 동의 여부를 자동으로 전달할 수 없습니다.
        /// </summary>
        /// <param name="consent"></param>
        private void SetGDPRConsent(bool consent)
        {
            GoogleMobileAds.Mediation.IronSource.Api.IronSource.SetConsent(consent);
            GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetHasUserConsent(consent);
            GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetIsAgeRestrictedUser(consent); // 사용자가 연령 제한 카테고리에 속하는 것으로 알려진 경우 아래 플래그를 true로 설정할 수도 있습니다.

#if UNITY_IOS
            GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetize.SetGDPRStatus(true, "v1.0.0");
#else
            GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetize.SetGDPRMessageVersion("v1.0.0");
#endif

            GoogleMobileAds.Mediation.DTExchange.Api.DTExchange.SetGDPRConsent(consent);
            GoogleMobileAds.Mediation.DTExchange.Api.DTExchange.SetGDPRConsentString("myGDPRConsentString");

            Debug.Log($"GDPR consent set to: {consent}");
        }

        /// <summary>
        /// CCPA "Do Not Sell" 상태 설정.
        /// CCPA(캘리포니아 소비자 개인정보 보호법)
        /// https://www.cloudflare.com/ko-kr/learning/privacy/what-is-the-ccpa/
        /// 
        /// 미국 주 개인 정보 보호법 은 사용자에게 법률에 정의된 바에 따라 '개인 정보'의 '판매'를 거부할 권리를 부여합니다. 
        /// 개인 정보 판매 거부 권리는 '판매'하는 회사의 홈페이지에 명시된 'Do Not Sell My Personal Information(내 개인 정보 판매 거부)' 
        /// 링크를 통해 행사할 수 있습니다. 미국 주 개인 정보 보호법 준수 가이드에서는 Google 광고 게재에 제한적인 
        /// 데이터 처리를 사용 설정하는 기능을 제공하지만 Google은 미디에이션 체인의 각 광고 네트워크에 이 설정을 적용할 수 없습니다. 
        /// 따라서 개인 정보 판매에 참여할 수 있는 미디에이션 체인의 각 광고 네트워크를 파악하고 
        /// 각 네트워크의 안내에 따라 규정을 준수해야 합니다.
        /// </summary>
        /// <param name="doNotSell"></param>
        private void SetCCPADoNotSell(bool doNotSell)
        {
            // Set CCPA "Do Not Sell" status
            GoogleMobileAds.Mediation.IronSource.Api.IronSource.SetMetaData("do_not_sell", doNotSell ? "true" : "false");
            GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetDoNotSell(doNotSell);

            GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetize.SetCCPAStatus(doNotSell);
            GoogleMobileAds.Mediation.DTExchange.Api.DTExchange.ClearCCPAString();
            GoogleMobileAds.Mediation.DTExchange.Api.DTExchange.SetCCPAString("do not sell");

            Debug.Log($"CCPA 'Do Not Sell' set to: {doNotSell}");
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