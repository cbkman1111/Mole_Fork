using Common.Global;
using Common.Scene;
using Common.UIObject;
using Common.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuShooting : MenuBase
    {
        public Ant.Joystick Joystick = null;
        public event System.Action<Vector3, float> Move;
        public event System.Action Stop;
        public event System.Action<int> OnSpeedModify;

        public bool InitMenu()
        {
            Joystick.Init();
            return true;
        }

        protected override void OnValueChanged(TMP_InputField input, string str) 
        {
            if (input == null)
                return;

            GiantDebug.Log($"Input changed: {str}");
            string name = input.name;
            if(name == "InputField - MoveSpeed")
            {
                if(int.TryParse(str, out int speed) == true)
                    OnSpeedModify?.Invoke(speed);
            }
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
