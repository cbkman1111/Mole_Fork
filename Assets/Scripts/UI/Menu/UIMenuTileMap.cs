using System;
using Common.Global;
using Common.Scene;
using Common.UIObject;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuTileMap : MenuBase
    {
        [FormerlySerializedAs("Joystick")] [SerializeField]
        public Ant.Joystick joystick = null;

        private Action saveGame = null;
        public override void OnInit()
        {

        }

        public bool InitMenu(Action<Vector3> move, Action stop, Action save)
        {
            joystick.Init((Vector3 direct, float angle) => {
                    SetText("Text - Debug", $"Angle : {angle}");

                    move?.Invoke(direct);
                },
                () =>
                {
                    stop?.Invoke();
                });

            saveGame = save;
            return true;
        }

        protected override void OnClick(Button btn)
        {
            string btnName = btn.name;
            if(btnName == "Button - Back")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
            else if (btnName == "Button - Save")
            {
                saveGame?.Invoke();
            }
        }
    }
}
