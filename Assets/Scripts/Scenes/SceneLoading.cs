using System;
using Common.Global;
using Common.Scene;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneLoading : SceneBase
    {
        private UILoadingMenu menu = null;

        public override bool Init(JSONObject param)
        {
            menu = UIManager.Instance.OpenMenu<UILoadingMenu>();
            if (menu != null)
            {
                menu.InitMenu();
            }

            return true;
        }

        public override async void Load(Action<float> update)
        {

            update(1f);
        }

    }
}
