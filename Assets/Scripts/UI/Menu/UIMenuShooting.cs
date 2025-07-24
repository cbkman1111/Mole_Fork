using Common.Global;
using Common.Scene;
using Common.UIObject;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuShooting : MenuBase
    {
        public Ant.Joystick Joystick = null;
        public System.Action<Vector3, float> Move { get; set; } = null;
        public System.Action Stop { get; set; } = null;

        public bool InitMenu()
        {
            Joystick.Init();
            Joystick.OnMove = null;
            Joystick.OnStop = null;
            return true;
        }
     
        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if (name == "Button - Back")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
        }
    }
}
