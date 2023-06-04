using System;
using SweetSugar.Scripts.AdsEvents;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Integrations;
using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.Monetization
{
    public class VideoButtonMap : MonoBehaviour
    {
        public Animator anim;
        public Button button;


        private void OnEnable()
        {
            button.interactable = true;
            Invoke("Prepare",2);
        }

        private void Prepare()
        {
            // if (AdsManager.THIS.GetRewardedUnityAdsReady(RewardsType.GetCoinsMap))
            {
                ShowButton();
            }
        }

        public void ShowVideoAds()
        {
            //if (!AdsManager.THIS.GetRewardedUnityAdsReady()) return;
            InitScript.Instance.currentReward = RewardsType.GetGems;

            AdsManager.THIS.ShowRewardedAds();
        }

        private void ShowButton()
        {
            anim.SetTrigger("show");
        }

        public void Hide()
        {
            button.interactable = false;

        }
    }
}