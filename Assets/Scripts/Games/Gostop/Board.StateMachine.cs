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
            stateMachine.Change(State.CARD_HIT, turnUser);
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
#if false
                /*
                case State.WAIT:
                    stateMachine.Process(
                        start: () => {
                        },
                        check: () => {
                            return true;
                        },
                        complete: () => { });
                    break;
                */

                // 게임 시작.
                /*
                case State.START_GAME:
                    stateMachine.Process(
                         start: () => {
                             listDebug.Clear();
                         },
                         check: () => {
                             return true;
                         },
                         complete: () => {
                             stateMachine.Change(State.CREATE_DECK);
                         });
                    break;
                */
                // 카드덱 생성.
                /*
                case State.CREATE_DECK:
                    stateMachine.Process(
                        start: () => {
                            ScoreUpdate();
                            CreateDeck();
                        },
                        check: () => {
                            int count = GetMoveCount(deck); //deck.Where(card => card.ListTween.Count == 0).ToList().Count;
                            return count == 0;
                        },
                        complete: () => {
                            stateMachine.Change(State.SHUFFLE_8);
                        });

                    break;
                */

                // 바닥 8장 깔기.
                /*
                case State.SHUFFLE_8:
                    stateMachine.Process(
                        start: () => {
                            Shuffle8Card();
                        },
                        check: () => {
                            int count = 0;
                            foreach (var slot in bottoms){
                                count += GetMoveCount(slot.Value);// slot.Value.Where(card => card.ListTween.Count == 0).ToList().Count;
                            }

                            return count == 0;
                        },
                        complete: () => {
                            stateMachine.Change(State.SHUFFLE_10);
                        });

                    break;
                */
                // 열장씩 나누기.
                /*
                case State.SHUFFLE_10:
                    stateMachine.Process(
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
                            stateMachine.Change(State.OPEN_8);
                        });
                    break;
                */

                // 8장 뒤집기
                /*
                case State.OPEN_8:
                    stateMachine.Process(
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
                            stateMachine.Change(State.CHECK_JORKER);
                        });
                    break;
                */

                // 바닥 조커 확인.
                /*
                case State.CHECK_JORKER:
                    stateMachine.Process(
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
                                stateMachine.Change(State.OPEN_1_MORE);
                            }
                            else
                            {
                                stateMachine.Change(State.HANDS_UP);
                            }
                        });
                    break;
                */

                /*
                case State.OPEN_1_MORE:
                    stateMachine.Process(
                        start: () => {
                            //Pop1Cards((int)Player.NONE);
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
                            stateMachine.Change(State.CHECK_JORKER);
                        });
                    break;
                */

                /*
                case State.HANDS_UP:
                    stateMachine.Process(
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
                            stateMachine.Change(State.HANDS_OPEN);
                        });
                    break;
                */

                /*
                case State.HANDS_OPEN: // 손패를 뒤집습니다.
                    stateMachine.Process(
                        start: () => {
                            HandOpen();
                        },
                        check: () => {
                            int count = GetMoveCount(hands[0]);//.Where(e => e.Open == false).ToList().Count;
                            return count == 0;
                        },
                        complete: () => {
                            stateMachine.Change(State.HANDS_SORT);
                        });

                    break;
                */
#endif
                case State.HANDS_SORT: // 손패를 정렬합니다.
                    StateInfo.Process(
                        start: () => SortHand(),
                        check: () => {
                            return GetMoveCount(hands[0]) == 0;
                        },
                        complete: () => {
                            stateMachine.Change(State.CARD_HIT, turnUser);
                            StateInfo = null;
                        });
                    break;
                
                case State.CARD_HIT:
                    StateInfo.Process(
                        start: () => {
                            menu.ShowScoreMenu(true);
                            if (turnUser == Player.COMPUTER)
                            {
                                var list = GetSameMonthCard((int)Board.Player.USER, hands[(int)Player.COMPUTER][0]);
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
                            stateMachine.Change(State.CARD_POP, turnUser);
                            StateInfo = null;
                        });
                    break;

                case State.CARD_POP_AND_HIT:
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
                            stateMachine.Change(State.CARD_HIT, turnUser);
                            StateInfo = null;
                        });
                    break;

                case State.CARD_POP:
                    StateInfo.Process(
                        start: () => {
                            StateInfo.info.popCard = Pop1Cards(turnUser);
                        },
                        check: () => {
                            int count = GetMoveAllCount();
                            if (count == 0)
                            {
                                if (StateInfo.info.popCard.Month == 13) // 뒤집어서 조커가 나오면 다시 뽑습니다.
                                {
                                    StateInfo.evt = StateEvent.INIT;
                                    return false;
                                }
                            }

                            return count == 0;
                        },
                        complete: () => {
                            stateMachine.Change(State.EAT_CHECK, turnUser);
                            StateInfo = null;
                        });

                    break;

                case State.EAT_CHECK: // 카드 획득 처리.
                    StateInfo.Process(
                        start: () => EatCheck(),
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
                                    var popup = UIManager.Instance.OpenPopup<PopupCardSelect>("PopupCardSelect");
                                    popup.Init(select[0], select[1], (Card selectCard) => {
                                        selectCard.Owner = turnUser;
                                        listEat.Add(selectCard);
                                        select.Clear();
                                    });
                         
                                }

                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        },
                        complete: () => {

                            select.Clear();
                            stateMachine.Change(State.EAT, turnUser);
                            StateInfo = null;
                        });
                    break;

                case State.EAT: // 카드 획득.
                    StateInfo.Process(
                        start: () => Eat(),
                        check: () => {
                            return true;
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

                            if (stealCount == 0)
                            {
                                stateMachine.Change(State.SCORE_UPDATE, turnUser);
                                StateInfo = null;
                            }
                            else
                            {
                                stateMachine.Change(State.STEAL, turnUser);
                                StateInfo = null;
                            }
                        });
                    break;
                case State.STEAL: // 카드 뺃기.
                    StateInfo.Process(
                        start: () => StealCard(),
                        check: () => {
                            int count = GetMoveAllCount();
                            return count == 0;
                        },
                        complete: () => {
                            if (stealCount == 0)
                            {
                                stateMachine.Change(State.SCORE_UPDATE, turnUser);
                                StateInfo = null;
                            }
                            else
                            {
                                stateMachine.Change(State.STEAL, turnUser);
                                StateInfo = null;
                            }
                        });
                    break;

                case State.SCORE_UPDATE: // 점수 체크.
                    StateInfo.Process(
                        start: () => ScoreUpdate(),
                        check: () => {
                            return true;
                        },
                        complete: () => {
                            stateMachine.Change(State.TURN_CHECK, turnUser);
                            StateInfo = null;
                        });

                    break;
                case State.TURN_CHECK: // 턴 바꾸기.
                    StateInfo.Process(
                        start: () => SortHand(),
                        check: () => {
                            int count = GetMoveAllCount();
                            return count == 0;
                        },
                        complete: () => {
                            State nextState = State.HANDS_SORT;

                            // 현재 턴유저의 카드가 0개라면.
                            if (hands[(int)Player.COMPUTER].Count == 0 && hands[(int)Player.USER].Count == 0)
                            {
                                nextState = State.GAME_OVER_TIE;
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

                case State.GAME_OVER_TIE: // 무승부 상태 처리.
                    StateInfo.Process(
                        start: () => { },
                        check: () => {
                            return GetMoveAllCount() == 0;
                        },
                        complete: () => {
                            DestroyAllCards();
                            //stateMachine.Init();
                            //stateMachine.Change(State.START_GAME);
                            StateInfo = null;
                        });
                    break;
            }
        }
    }

}
