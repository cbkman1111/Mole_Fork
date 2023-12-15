using Common.Global;
using Common.Scene;
using Common.UIObject;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuBehaviorTree : MenuBase
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
            if(name == "Button - Back")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
        }
    }
}
