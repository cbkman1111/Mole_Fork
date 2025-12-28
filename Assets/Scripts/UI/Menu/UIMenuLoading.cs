using Common.UIObject;
using UnityEngine.UI;
using Common.Global;
using Common.Utils;

namespace UI.Menu
{
    public class UILoadingMenu : MenuBase
    {

        public bool InitMenu()
        {
            return true;
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if (name == "Button - Back")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneMenu);
            }
        }
    }
}
