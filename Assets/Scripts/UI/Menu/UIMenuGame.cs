using Common.Global;
using Common.Scene;
using Common.UIObject;
using UI.Popup;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuGame : MenuBase
    {
        public override void OnInit()
        {

        }

        public bool InitMenu()
        {
            
            return true;
        }

        public void OnAngleChange(float angle)
        {
            SetText("Text - title", $"{angle}");
        }

        public override void OnValueChanged(Slider slider, float f) 
        {
            string name = slider.name;
            if(name.CompareTo("Slider - Camera") == 0)
            {
                SceneBase scene = AppManager.Instance.CurrScene;
                const int FOV_MIN = 10;
                const int FOV_AMOUNT = 50;

                scene.MainCamera.fieldOfView = FOV_MIN + FOV_AMOUNT * f;
            }
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if (name == "Button - A")
            {
                UIManager.Instance.OpenPopup<PopupSample>();
                SetText("Text - title", "a clicked");
            }
            else if (name == "Button - B")
            {
                SoundManager.Instance.PlayEffect("EFF_shoot");
                SetText("Text - title", "b clicked");

            }
            else if (name == "Button - C")
            {
                SoundManager.Instance.PlayEffect("single_coin_fall_on_wood");
                SetText("Text - title", "c clicked");

                UIManager.Instance.OpenPopup<PopupScrollViewTest>();
            }
            else if (name == "Button - Exit")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
        }
    }
}
