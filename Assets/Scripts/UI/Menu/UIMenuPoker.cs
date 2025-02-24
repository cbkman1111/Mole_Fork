using Common.Global;
using Common.Scene;
using Common.UIObject;
using Pocker;
using Poker;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuPoker: MenuBase
    {
        [SerializeField]
        private UIPokerPlayer[] Players;

        public bool InitMenu()
        {
            for (int i = 0; i < Players.Length; i++)
            {
                var playerInfo = GlobalGameManager.Instance.PockerData.Players[(PlayUser)i];
                bool ret = Players[i].InitCards(playerInfo);
            }

            return true;
        }

        public void UpdateRank()
        {
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i].UpdateHandRank();
            }
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if (name == "Button - Back")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
            else if (name == "Button - Process")
            {
                // 있는 카드 뒤집기
                for (int i = 0; i < Players.Length; i++)
                {
                    Players[i].OpenCards(2);
                }
            }
        }
    }
}
