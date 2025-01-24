using Common.Global;
using Common.Scene;
using Common.UIObject;
using Common.Utils.Pool;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuPoker: MenuBase
    {

        public bool createField = false;

        public bool InitMenu()
        {
            var prefab = ResourcesManager.Instance.LoadInBuild<UIJumpCoin>("UIJumpCoin");
            //pool = Common.Utils.Pool.Pool<UIJumpCoin>.Create(prefab, targetCube, 30);

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
