using Common.Global;
using Common.Scene;
using Common.UIObject;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuAdMob : MenuBase
    {
        public Image Ready;
        public Image NotReady;

        public bool InitMenu()
        {
            SetState(AdMobManager.Instance.RewardAdReady);
            AdMobManager.Instance.OnChangeAdState += SetState;
            return true;
        }

        private void SetState(bool isReady)
        {
            Ready.gameObject.SetActive(isReady);
            NotReady.gameObject.SetActive(!isReady);
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if (name == "Button - Back")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
            else if(name == "Button - LoadAd")
            {
                AdMobManager.Instance.LoadRewardedAd();
            }
            else if(name == "Button - ShowAd")
            {
                AdMobManager.Instance.ShowRewardVideo();
            }
            else if(name == "Button - DestroyAd")
            {
                AdMobManager.Instance.DestroyAd();
            }
        }
    }
}
