using System;
using Common.Global;
using Common.Scene;
using Common.UIObject;
using Scenes;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuAntHouse : MenuBase
    {
        [SerializeField]
        public Ant.Joystick Joystick = null;
        public Camera miniMapCamera = null;

        public override void OnInit()
        {

        }

        public bool InitMenu(Action<Vector3> move, Action stop)
        {
            Joystick.Init((Vector3 direct, float angle) => {
                    SetText("Text - Debug", $"Angle : {angle}");

                    move?.Invoke(direct);
                },
                () =>
                {
                    stop?.Invoke();
                });


            var mainCamera = AppManager.Instance.CurrScene.MainCamera;
            miniMapCamera.transform.SetParent(mainCamera.transform);
            miniMapCamera.transform.position = new Vector3(0,0,-1);

            return true;
        }


        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if (name.CompareTo("Button - Back") == 0)
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
            else if (name.CompareTo("Button - Ok") == 0)
            {
                var scene = AppManager.Instance.CurrScene as SceneAntHouse;
                scene.RemoveTile();
            }
            else if (name.CompareTo("Button - Create") == 0)
            {
                var scene = AppManager.Instance.CurrScene as SceneAntHouse;
                scene.CreateMonster();
            }
            else if (name.CompareTo("Button - SaveGame") == 0)
            {
                var scene = AppManager.Instance.CurrScene as SceneAntHouse;
                scene.SaveGame();
            }
            else if (name.CompareTo("Button - CreateObject") == 0)
            {
                var scene = AppManager.Instance.CurrScene as SceneAntHouse;
                scene.CreateObject();
            }
            else if (name.CompareTo("Button - Map 4") == 0)
            {
                JSONObject jsonParam = new JSONObject();
                jsonParam.SetField("map_no", 4);
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneAntHouse, param: jsonParam);
            }
            else if (name.CompareTo("Button - Map 3") == 0)
            {
                JSONObject jsonParam = new JSONObject();
                jsonParam.SetField("map_no", 3);
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneAntHouse, param: jsonParam);
            }
        }
    }
}
