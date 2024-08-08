using Common.Global;
using Common.Scene;
using Common.UIObject;
using UI.Popup;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuChat : MenuBase
    {
        public bool InitMenu()
        {
            return true;
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if (name == "Button - Exit")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
            else if (name == "Button - OpenScroll")
            {
                UIManager.Instance.OpenPopup<UIPopupScrollViewTest>();
            }
        }

        public override void OnInit() { }
    }
}
