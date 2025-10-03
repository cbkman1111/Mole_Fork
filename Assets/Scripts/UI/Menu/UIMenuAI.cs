using Common.Global;
using Common.Scene;
using Common.UIObject;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuAI : MenuBase
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
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
            else if (name == "Button - 1")
            {
                // Å×½ºÆ® 
            }
            else if (name == "Button - 2")
            {

            }
            else if (name == "Button - 3")
            {

            }
            else if (name == "Button - 4")
            {

            }
            else if (name == "Button - 5")
            {

            }
        }
    }
}