using DG.Tweening;
using System;

using System.Collections.Generic;
using System.Linq;
using Common.Global;
using UI.Menu;
using UI.Popup;
using UnityEngine;
using static UnityEditor.Rendering.InspectorCurveEditor;
using UnityEditor;


namespace Gostop
{
 
    public partial class Board : MonoBehaviour
    {
        public StateInfo StateInfo = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool MyTurn()
        {
            return turnUser == Player.USER;
        }

        /// <summary>
        /// 게임 시작.
        /// </summary>
        public void StartGame()
        {
            stateMachine.Change(State.StartGame, turnUser);
        }

        private int GetMoveCount(List<Card> list)
        {
            int count = list.Where(card => card.ListTween.Count != 0).ToList().Count;
            return count;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void Update()
        { 
            if(stateMachine == null)
                return;
                
            if(StateInfo == null)
                StateInfo = stateMachine.PopState();
            
            if (StateInfo == null)
                return;
            
            State state = StateInfo.state;
            switch (state)
            {
                // 게임 시작.
                case State.StartGame:
                    StateInfo.Process(
                         start: () => {
                             DestroyAllCards();
                         },
                         check: () => {
                             return true;
                         },
                         complete: () => {
                             stateMachine.Change(State.CreateDeck, Player.NONE);
                             StateInfo = null;
                         });
                    break;
          
                // 카드덱 생성.
                case State.CreateDeck:
                    StateInfo.Process(
                        start: () => {
                            //ScoreUpdate();
                            CreateDeck();
                        },
                        check: () => {
                            int count = deck.Where(card => card.ListTween.Count != 0).ToList().Count;
                            return count == 0;
                        },
                        complete: () => {
                            stateMachine.Change(State.Shuffle_8, Player.NONE);
                            StateInfo = null;
                        });
                    break;

                // 바닥 8장 깔기.
                case State.Shuffle_8:
                    StateInfo.Process(
                        start: () => {
                            Shuffle8Card();
                        },
                        check: () => {
                            return GetMoveAllCount() == 0;
                        },
                        complete: () => {
                            stateMachine.Change(State.Shuffle_10, Player.NONE);
                            StateInfo = null;
                        });
                    break;
          
                // 열장씩 나누기.
                case State.Shuffle_10:
                    StateInfo.Process(
                        start: () => {
                            //Pop10Cards();
                            Shuffle10Card();
                        },
                        check: () => {
                            int count = 0;
                            count += GetMoveCount(hands[0]);// hands[0].Where(e => e.ListTween.Count == 0).ToList().Count;
                            count += GetMoveCount(hands[1]);// hands[1].Where(e => e.ListTween.Count == 0).ToList().Count;
                            return count == 0;
                        },
                        complete: () => {
                            stateMachine.Change(State.Open8, Player.NONE);
                            StateInfo = null;
                        });
                    break;


                // 8장 뒤집기
                case State.Open8:
                    StateInfo.Process(
                        start: () => {
                            FlipCard8();
                        },
                        check: () => {
                            int count = 0;
                            foreach (var slot in bottoms) {
                                count += GetMoveCount(slot.Value);// slot.Value.Where(e => e.ListTween.Count == 0).ToList().Count;
                            }

                            return count == 0;
                        },
                        complete: () => {
                            stateMachine.Change(State.CheckJocker, Player.NONE);
                            StateInfo = null;
                        });
                    break;
          

                // 바닥 조커 확인.
                case State.CheckJocker:
                    StateInfo.Process(
                        start: () => {
                            CheckJoker();
                        },
                        check: () => {
                            int countMove = 0;
                            foreach (var slot in bottoms)
                            {
                                countMove += GetMoveCount(slot.Value);
                            }

                            return countMove == 0;
                        },
                        complete: () => {
                            int Count = 0;
                            int jockerCount = 0;
                            foreach (var slot in bottoms)
                            {
                                int n = slot.Value.Where(e => e.Month == 13).ToList().Count;
                                if (n > 0)
                                {
                                    jockerCount += n;
                                }

                                Count += slot.Value.Count;
                            }

                            if (jockerCount > 0 || Count < 8)
                            {
                                stateMachine.Change(State.Open1More, Player.NONE);
                                StateInfo = null;
                            }
                            else
                            {
                                stateMachine.Change(State.HandUp, Player.NONE);
                                StateInfo = null;
                            }
                        });
                    break;
       
                case State.Open1More:
                    StateInfo.Process(
                        start: () => {
                            PopDeckCard(Player.NONE);
                        },
                        check: () => {
                            int count = 0;

                            foreach (var slot in bottoms)
                            {
                                count += GetMoveCount(slot.Value);
                            }

                            return count == 0;
                        },
                        complete: () => {
                            stateMachine.Change(State.CheckJocker, Player.NONE);
                            StateInfo = null;
                        });
                    break;


                case State.HandUp:
                    StateInfo.Process(
                        start: () => {
                            HandsUp();
                        },
                        check: () => {
                            int count = 0;
                            count += GetMoveCount(hands[0]);
                            count += GetMoveCount(hands[1]);

                            return count == 0;
                        },
                        complete: () => {
                            stateMachine.Change(State.HandOpen, Player.NONE);
                            StateInfo = null;
                        });
                    break;
            
 
                case State.HandOpen: // 손패를 뒤집습니다.
                    StateInfo.Process(
                        start: () => {
                            HandOpen();
                        },
                        check: () => {
                            int count = GetMoveCount(hands[0]);//.Where(e => e.Open == false).ToList().Count;
                            return count == 0;
                        },
                        complete: () => {
                            stateMachine.Change(State.HandSort, Player.NONE);
                            StateInfo = null;
                        });
                    break;

                case State.HandSort: // 손패를 정렬합니다.
                    StateInfo.Process(
                        start: () => HandSort(),
                        check: () => {
                            return GetMoveCount(hands[0]) == 0;
                        },
                        complete: () => {
                            stateMachine.Change(State.HitCard, turnUser);
                            StateInfo = null;
                        });
                    break;
                
                case State.HitCard:
                    StateInfo.Process(
                        start: () => {
                            menu.ShowScoreMenu(true);
                            if (turnUser == Player.COMPUTER)
                            {
                                var list = GetSameMonthCard((int)Board.Player.COMPUTER, hands[(int)Player.COMPUTER][0]);
                                if (list.Count == 3) // 폭탄
                                {
                                    HitBomb((int)Player.COMPUTER, list, list[0]);
                                    StateInfo.evt = StateEvent.PROGRESS;
                                }
                                else if (list.Count == 4) // 총통
                                {
                                    HitChongtong((int)Player.COMPUTER, list, list[0]);
                                    StateInfo.evt = StateEvent.PROGRESS;
                                }
                                else
                                {
                                    HitCard((int)Player.COMPUTER, hands[(int)Player.COMPUTER][0]);
                                }
                            }
                        },
                        check: () => {
                            if (StateInfo.info.hited == false)
                            {
                                return false;
                            }

                            if (StateInfo.info.hit.Month == 13)
                            {
                                return false;
                            }

                            int count = GetMoveAllCount();
                            return count == 0;
                        },
                        complete: () => {
                            stateMachine.Change(State.PopCardDeck, turnUser);
                            StateInfo = null;
                        });
                    break;

                case State.PopCardDeckAndHit:
                    StateInfo.Process(
                        start: () => {
                            StateInfo.info.popCard = PopDeckCard();
                        },
                        check: () => {
                            if (StateInfo.info.popCard.Month == 13) // 뒤집어서 조커가 나오면 다시 뽑습니다.
                            {
                                StateInfo.evt = StateEvent.INIT;
                                return false;
                            }

                            return 0 == GetMoveAllCount();
                        },
                        complete: () => {
                            stateMachine.Change(State.HitCard, turnUser);
                            StateInfo = null;
                        });
                    break;

                case State.PopCardDeck:
                    StateInfo.Process(
                        start: () => {
                            StateInfo.info.popCard = PopDeckCard(turnUser);
                            if(StateInfo.info.popCard == null)
                                Debug.LogError("PopDeckCard is null");
                        },
                        check: () => {
                            int count = GetMoveAllCount();
                            if (count == 0)
                            {
                                if (StateInfo.info.popCard != null &&
                                    StateInfo.info.popCard.Month == 13) // 뒤집어서 조커가 나오면 다시 뽑습니다.
                                {
                                    StateInfo.evt = StateEvent.INIT;
                                    return false;
                                }
                            }

                            return count == 0;
                        },
                        complete: () => {
                            stateMachine.Change(State.TakeCardCondition, turnUser);
                            StateInfo = null;
                        });

                    break;

                case State.TakeCardCondition: // 카드 획득 처리.
                    StateInfo.Process(
                        start: () => TakeCardCondition(),
                        check: () => {
                            if (select.Count == 2)
                            {
                                if (turnUser == Player.COMPUTER)
                                {
                                    select[0].Owner = Player.COMPUTER;
                                    listEat.Add(select[0]);
                                    select.Clear();
                                }
                                else
                                {
                                    if (select[0].KindOfCard != select[1].KindOfCard)
                                    {
                                        var popup = UIManager.Instance.OpenPopup<UIPopupCardSelect>("UIPopupCardSelect");
                                        popup.Init(select[0], select[1], (Card selectCard) => {
                                            selectCard.Owner = turnUser;
                                            listEat.Add(selectCard);
                                            select.Clear();
                                        });
                                    }
                                    else
                                    {
                                        select[0].Owner = turnUser;
                                        listEat.Add(select[0]);
                                        select.Clear();
                                    }
                                }

                                return false;
                            }
                            else
                            {
                                if(UIManager.Instance.FindPopup("UIPopupMessage"))
                                {
                                    return false;
                                }

                                return true;
                            }
                        },
                        complete: () => {
                            select.Clear();
                            stateMachine.Change(State.TakeCard, turnUser);
                            StateInfo = null;
                        });
                    break;

                case State.TakeCard: // 카드 획득.
                    StateInfo.Process(
                        start: () => TackeCardToScore(),
                        check: () => {
                            StateInfo.info.delta += Time.deltaTime;
                            return StateInfo.info.delta > 0.2f;
                        },
                        complete: () => {
                            // 주인 없는 카드로 설정.
                            foreach (var kindSlot in bottoms)
                            {
                                var list = kindSlot.Value;
                                foreach (var card in list) {
                                    card.Owner = Player.NONE;
                                }
                            }

                            stateMachine.Change(State.TakeToMe, turnUser);
                            StateInfo = null;

                        });
                    break;

                case State.TakeToMe:
                    StateInfo.Process(
                        start: () => {
                            int count = 0;
                            int total = listEat.Count;
                            foreach (var card in listEat)
                            {
                                TackCard(card, total - count); // 카드 획득.
                                count++;
                            }

                            listEat.Clear();
                        },
                        check: () => {
                            return GetMoveAllCount() == 0;
                        },
                        complete: () => {
                            if (stealCount == 0)
                            {
                                stateMachine.Change(State.UpdateScore, turnUser);
                                StateInfo = null;
                            }
                            else
                            {
                                stateMachine.Change(State.StealCard, turnUser);
                                StateInfo = null;
                            }
                        });


                    break;

                case State.StealCard: // 카드 뺃기.
                    StateInfo.Process(
                        start: () => StealCard(),
                        check: () => {
                            int count = GetMoveAllCount();
                            return count == 0;
                        },
                        complete: () => {
                            if (stealCount == 0)
                            {
                                stateMachine.Change(State.UpdateScore, turnUser);
                                StateInfo = null;
                            }
                            else
                            {
                                stateMachine.Change(State.StealCard, turnUser);
                                StateInfo = null;
                            }
                        });
                    break;

                case State.UpdateScore: // 점수 체크.
                    StateInfo.Process(
                        start: () => ScoreUpdate(),
                        check: () => {
                            return true;
                        },
                        complete: () => {
                            stateMachine.Change(State.ChangeTurn, turnUser);
                            StateInfo = null;
                        });

                    break;
                case State.ChangeTurn: // 턴 바꾸기.
                    StateInfo.Process(
                        start: () => HandSort(),
                        check: () => {
                            int count = GetMoveAllCount();
                            return count == 0;
                        },
                        complete: () => {
                            State nextState = State.HandSort;

                            // 현재 턴유저의 카드가 0개라면.
                            if (hands[(int)Player.COMPUTER].Count == 0 && hands[(int)Player.USER].Count == 0)
                            {
                                nextState = State.GameOver_Tie;
                                nextState = State.GameOver_Win;
                                nextState = State.GameOver_Lose;
                            }
                            else 
                            {
                                if (turnUser == Player.USER)
                                {
                                    turnUser = Player.COMPUTER;
                                }
                                else
                                {
                                    turnUser = Player.USER;
                                }

                                //stateMachine.AddTurn(turnUser); // 턴을 증가.
                            }

                            stateMachine.Change(nextState, turnUser);
                            StateInfo = null;
                        });
                    break;

                case State.GameOver_Win: // 승리 상태 처리.
                    StateInfo.Process(
                        start: () => { },
                        check: () => {
                            return GetMoveAllCount() == 0;
                        },
                        complete: () => {
                            DestroyAllCards();
                            stateMachine.Change(State.StartGame, turnUser);
                            StateInfo = null;
                        });
                    break;

                case State.GameOver_Lose: // 패배 상태 처리.
                    StateInfo.Process(
                        start: () => { },
                        check: () => {
                            return GetMoveAllCount() == 0;
                        },
                        complete: () => {
                            DestroyAllCards();
                            stateMachine.Change(State.StartGame, turnUser);
                            StateInfo = null;
                        });
                    break;

                case State.GameOver_Tie: // 무승부 상태 처리.
                    StateInfo.Process(
                        start: () => { },
                        check: () => {
                            return GetMoveAllCount() == 0;
                        },
                        complete: () => {
                            DestroyAllCards();
                            stateMachine.Change(State.StartGame, turnUser);
                            StateInfo = null;
                        });
                    break;
            }
        }
    }
}
