using System.Collections;
using SweetSugar.Scripts.AdsEvents;
using SweetSugar.Scripts.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SweetSugar.Scripts.Integrations
{
    public class ShowAdsByButton : MonoBehaviour
    {
        public UnityEvent OnRewaredeShown;
        public bool checkRewardedAvailable;
        private CanvasGroup canvasGroup;
        public RewardsType placement;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

        }

        private void OnEnable()
        {
            if (canvasGroup != null)
            {
                
                canvasGroup.alpha = 1;
                canvasGroup.blocksRaycasts = true;
                if (checkRewardedAvailable && GetComponent<Button>().onClick.GetPersistentMethodName(0) == "ShowRewardedAd" /*&& !AdManager.Instance.IsRewardedAvailable()*/)
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.blocksRaycasts = false;
                    StartCoroutine(WaitForAds());
                }
            }

        }

        private IEnumerator WaitForAds()
        {
            yield return new WaitUntil(()=>AdsManager.THIS.GetRewardedUnityAdsReady());
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
 

        }

        private void OnDisable()
        {

        }

        private void OnRewardedShown()
        {
            OnRewaredeShown?.Invoke();

        }

        // public void ShowInterstitial(AdEvents placement)
        // {
        //     if (PlayerPrefs.GetInt("tutorialShown", 0) == 0) return;
        //     AdsManager.THIS.ShowVideo(placement);
        //     EventsListener.CustomEvent("Show ads " +placement);
        // }

        public void ShowRewardedAd()
        {
            AdsManager.OnRewardedShown += OnRewardedShown;
            AdsManager.THIS.ShowRewardedAds();
        }
    }
}