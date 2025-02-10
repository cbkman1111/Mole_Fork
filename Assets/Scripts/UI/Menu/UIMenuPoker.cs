using Common.Global;
using Common.Scene;
using Common.UIObject;
using Common.Utils;
using Pocker;
using Scenes;
using UnityEngine;
using UnityEngine.UI;
using static Scenes.ScenePoker;

namespace UI.Menu
{
    public class UIMenuPoker: MenuBase
    {
        [SerializeField]
        private UIPokerPlayer[] Players;

        public bool InitMenu(ScenePoker scene)
        {
            for(int i = 0; i < Players.Length; i++)
            {
                var playerInfo = scene.Players[(PlayUser)i];
                bool ret = Players[i].InitCards(playerInfo);
                if(ret == false)
                {
                    GiantDebug.Log("ret is false.");
                }
             
                Players[i].OpenCards(7);
            }

            return true;
        }

        public void UpdateRank(ScenePoker scene)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                var playerInfo = scene.Players[(PlayUser)i];
                Players[i].SetHandRank(playerInfo);
            }
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
