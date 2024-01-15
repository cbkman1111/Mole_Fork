using Common.Global;
using Common.Scene;
using Common.UIObject;
using Scenes;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuTetris : MenuBase
    {
        public override void OnInit()
        {

        }

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
            else if (name == "Button - Rotate")
            {
                var scene = AppManager.Instance.CurrScene as SceneTetris;
                if (scene == null)
                    return;

                scene.OnRotateShape();
            }
            else if (name == "Button - Right")
            {
                var scene = AppManager.Instance.CurrScene as SceneTetris;
                if (scene == null)
                    return;

                scene.OnRight();
            }
            else if (name == "Button - Left")
            {
                var scene = AppManager.Instance.CurrScene as SceneTetris;
                if (scene == null)
                    return;

                scene.OnLeft();
            }
            else if (name == "Button - Up")
            {
                var scene = AppManager.Instance.CurrScene as SceneTetris;
                if (scene == null)
                    return;

                scene.OnUp();
            }
            else if (name == "Button - Down")
            {
                var scene = AppManager.Instance.CurrScene as SceneTetris;
                if (scene == null)
                    return;

                scene.OnDown();
            }
            else if (name == "Button - Insert")
            {
                var scene = AppManager.Instance.CurrScene as SceneTetris;
                if (scene == null)
                    return;

                scene.OnInsert();
            }
        }
    }
}
