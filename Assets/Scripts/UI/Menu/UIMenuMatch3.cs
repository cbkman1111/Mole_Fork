using System;
using Common.Global;
using Common.Scene;
using Common.UIObject;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuMatch3 : MenuBase
    {
        int level = 1;

        public Action<int> OnStartGame { get; set;}

        public override void OnInit()
        {

        }

        public bool InitMenu()
        {

            return true;
        }

        protected override void OnValueChanged(InputField input, string str) 
        {
            string name = input.name;
            if(name == "InputField - Level")
            {
                level = int.Parse(str);
            }
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if (name == "Button - Back")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
            else if (name == "Button - StartGame")
            {
                OnStartGame?.Invoke(level);

            }
        }
    }
}
