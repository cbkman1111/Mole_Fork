using Common.Global;
using Common.Scene;
using Common.UIObject;
using Pocker;
using Poker;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuPoker: MenuBase
    {
        [SerializeField]
        private UIPokerPlayer[] PlayerCell;
        public Action OnRestartGame = null;

        public bool InitMenu(Action restartGame)
        {
            for (int i = 0; i < PlayerCell.Length; i++)
            {
                var playerInfo = GlobalGameManager.Instance.PockerGameData.Players[(PlayUser)i];
                bool ret = PlayerCell[i].InitCards(playerInfo);
            }

            UpdateUI();
            OnRestartGame = restartGame;
            return true;
        }

        public void UpdateUI()
        {
            for (int i = 0; i < PlayerCell.Length; i++)
            {
                PlayerCell[i].UpdateCard();
                PlayerCell[i].OpenCards();
                PlayerCell[i].UpdateHandRank();
            }

            SetText("Text - State", GlobalGameManager.Instance.PockerGameData.State.ToString());
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
            else if (name == "Button - NextStep")
            {
                if (GlobalGameManager.Instance.PockerGameData.GameOver == true)
                {
                    if (OnRestartGame != null)
                    {
                        OnRestartGame();
                    }
                }
                else 
                {
                    // 상태.
                    switch (GlobalGameManager.Instance.PockerGameData.State)
                    {
                        case SevenPokerState.Start: 
                            GlobalGameManager.Instance.PockerGameData.State = SevenPokerState.CardDeal3;
                            break;

                        case SevenPokerState.CardDeal3:
                            GlobalGameManager.Instance.PockerGameData.DealCard(3, true);
                            GlobalGameManager.Instance.PockerGameData.UpdateHandRank();
                            GlobalGameManager.Instance.PockerGameData.UpdateRank();
                            GlobalGameManager.Instance.PockerGameData.State = SevenPokerState.OpenFirstCard;
                            break;

                        case SevenPokerState.OpenFirstCard:
                            GlobalGameManager.Instance.PockerGameData.OpenFirstCard();
                            GlobalGameManager.Instance.PockerGameData.State = SevenPokerState.Bet1;
                            break;

                        case SevenPokerState.Bet1:
                            GlobalGameManager.Instance.PockerGameData.BetEnemy();
                            GlobalGameManager.Instance.PockerGameData.State = SevenPokerState.CardDeal4;
                            // 턴이 나면 나의 배팅.
                            break;

                        case SevenPokerState.CardDeal4:
                            GlobalGameManager.Instance.PockerGameData.DealCard(1);
                            GlobalGameManager.Instance.PockerGameData.State = SevenPokerState.Bet2;
                            break;

                        case SevenPokerState.Bet2: // 2차 베팅(4구)
                            GlobalGameManager.Instance.PockerGameData.BetEnemy();
                            GlobalGameManager.Instance.PockerGameData.State = SevenPokerState.CardDeal5;
                            break;

                        case SevenPokerState.CardDeal5: // 5번째 카드 받기
                            GlobalGameManager.Instance.PockerGameData.DealCard(1);
                            GlobalGameManager.Instance.PockerGameData.State = SevenPokerState.Bet3;
                            break;

                        case SevenPokerState.Bet3: // 3차 베팅(5구)
                            GlobalGameManager.Instance.PockerGameData.BetEnemy();
                            GlobalGameManager.Instance.PockerGameData.State = SevenPokerState.CardDeal6;
                            break;

                        case SevenPokerState.CardDeal6: // 6번째 카드 받기
                            GlobalGameManager.Instance.PockerGameData.DealCard(1);
                            GlobalGameManager.Instance.PockerGameData.State = SevenPokerState.Bet4;
                            break;

                        case SevenPokerState.Bet4: // 4차 베팅(6구)
                            GlobalGameManager.Instance.PockerGameData.BetEnemy();
                            GlobalGameManager.Instance.PockerGameData.State = SevenPokerState.CardDealHidden;
                            break;

                        case SevenPokerState.CardDealHidden: // 7번째 히든 카드 받기
                            GlobalGameManager.Instance.PockerGameData.DealCard(1);
                            GlobalGameManager.Instance.PockerGameData.State = SevenPokerState.Bet5Last;
                            break;

                        case SevenPokerState.Bet5Last: // 5차 베팅(7구)
                            GlobalGameManager.Instance.PockerGameData.BetEnemy();
                            GlobalGameManager.Instance.PockerGameData.State = SevenPokerState.CheckWinner;
                            break;

                        case SevenPokerState.CheckWinner: // 승자 확인
                            GlobalGameManager.Instance.PockerGameData.OpenAllCards();
                            GlobalGameManager.Instance.PockerGameData.State = SevenPokerState.GameOver;
                            break;

                        case SevenPokerState.GameOver: // 게임 오버.
                            if (OnRestartGame != null)
                            {
                                OnRestartGame();
                            }
                            break;
                    }


                    // 족보 갱신 및 순위 갱신.
                    GlobalGameManager.Instance.PockerGameData.UpdateHandRank();
                    GlobalGameManager.Instance.PockerGameData.UpdateRank();

                    // UI 갱신.
                    UpdateUI();
                }
            }
        }
    }
}
