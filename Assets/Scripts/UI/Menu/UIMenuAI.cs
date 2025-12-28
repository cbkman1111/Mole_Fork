using System;
using Common.Global;
using Common.Scene;
using Common.UIObject;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuAI : MenuBase
    {
        public Action<string> clickAction = null;

        public bool InitMenu(Action<string> click)
        {
            clickAction = click;    
            return true;
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if (name == "Button - Back")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneMenu);
            }
            else if (name == "Button - 1")
            {
                clickAction.Invoke(name);
            }
            else if (name == "Button - 2")
            {
                clickAction.Invoke(name);
            }
            else if (name == "Button - 3")
            {
                clickAction.Invoke(name);
            }
            else if (name == "Button - 4")
            {
                clickAction.Invoke(name);
            }
            else if (name == "Button - 5")
            {
                clickAction.Invoke(name);
            }
        }
    }
}