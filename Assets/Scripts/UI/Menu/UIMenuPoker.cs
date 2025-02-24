using Common.Global;
using Common.Scene;
using Common.UIObject;
using Pocker;
using Poker;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuPoker: MenuBase
    {
        [SerializeField]
        private UIPokerPlayer[] Players;

        public Action OnRestartGame = null;

        public bool InitMenu(Action restartGame)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                var playerInfo = GlobalGameManager.Instance.PockerData.Players[(PlayUser)i];
                bool ret = Players[i].InitCards(playerInfo);
            }

            OnRestartGame = restartGame;
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

            else if (name == "Button - Fold")
            {
            }
            else if (name == "Button - Check")
            {
            }
            else if (name == "Button - Bet")
            {
            }
            else if (name == "Button - Call")
            {
            }
            else if (name == "Button - Raise")
            {
            }
            else if (name == "Button - Allin")
            {
            }
            else if (name == "Button - ReStartGame")
            {
                if (GlobalGameManager.Instance.PockerData.GameOver == true)
                {
                    if (OnRestartGame != null)
                    {
                        OnRestartGame();
                    }
                }
            }
            else if (name == "Button - DealCard")
            {
                if (GlobalGameManager.Instance.PockerData.GameOver == true)
                {
                    return;
                }

                GlobalGameManager.Instance.PockerData.DealCard(1);

                // 있는 카드 뒤집기
                for (int i = 0; i < Players.Length; i++)
                {
                    Players[i].UpdateCard();
                    Players[i].OpenCards();
                }

                GlobalGameManager.Instance.PockerData.UpdateHandRank();
                GlobalGameManager.Instance.PockerData.UpdateRank();

                UpdateRank();
            }
            else if (name == "Button - OpenCard")
            {
                // 있는 카드 뒤집기
                for (int i = 0; i < Players.Length; i++)
                {
                    Players[i].OpenCards();
                }
            }
        }
    }
}
