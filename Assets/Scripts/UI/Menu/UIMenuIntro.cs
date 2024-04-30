using Common.Global;
using Common.Scene;
using Common.UIObject;
using Common.Utils;
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
            GiantDebug.Log($"UIMenuIntro - InitMenu.");
            return true;
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if(name == "Button - Start")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
        }
    }
}
