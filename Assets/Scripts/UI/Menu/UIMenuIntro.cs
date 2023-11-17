using Common.Global;
using Common.Scene;
using Common.UIObject;
using TMPro;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuIntro : MenuBase
    {
        public override void OnInit()
        {

        }

        public bool InitMenu()
        {
            return true;
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if(name == "Button - Start")
            {
                //SoundManager.Instance.StopAllSound();
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneLobby);
            }
        }
    }
}
