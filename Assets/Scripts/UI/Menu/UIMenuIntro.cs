using Common.Global;
using Common.Scene;
using Common.UIObject;
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
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
        }
    }
}
