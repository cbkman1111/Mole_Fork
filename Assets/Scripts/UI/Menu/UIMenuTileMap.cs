using System;
using Common.Global;
using Common.Scene;
using Common.UIObject;
using Common.Utils.Pool;
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
        private Action<float> zoomCamera = null;
        
        public override void OnInit()
        {

        }

        public bool InitMenu(Action<Vector3> move, Action stop, Action save, Action<float> zoom)
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
            zoomCamera = zoom;
            return true;
        }

        public override void OnValueChanged(Slider slider, float f)
        {
            zoomCamera(f);
        }

        /*
        private void Update()
        {
            int countGround = PoolManager.Instance.GetObjectCount("TileGround");
            int countWater = PoolManager.Instance.GetObjectCount("TileWater");
            SetText("Text - MapInfo", $"{countGround}/{countWater}/{countGround + countWater}");
        }
        */

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
