using System.Collections.Generic;
using System.Linq;
using Common.Global;
using UI.Popup;
using UnityEngine;

namespace Gostop
{
    /// <summary>
    /// 게임 보드.
    /// </summary>
    public partial class Board : MonoBehaviour
    {
        public CommandInfo CommandInfo = null;

        /// <summary>
        /// 내턴
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
            commandProcedure.Enqueue(Command.StartGame);
            CommandInfo = commandProcedure.Dequeue();
        }

        /// <summary>
        /// 움직이는 카드 존재 확인.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private int GetMoveCount(List<Card> list)
        {
            int count = list.Where(card => card.ListTween.Count != 0).ToList().Count;
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void LateUpdate()
        { 
            if(commandProcedure == null)
                return;
           
            if (CommandInfo == null)
                return;
            
            Command commandType = CommandInfo.type;
            switch (commandType)
            {
                // 게임 시작.
                case Command.StartGame:
                    CommandInfo.Process(
                         start: () => {
                             DestroyAllCards();

                             gameScore[0].Init();
                             gameScore[1].Init();
                             ScoreUpdate();
                         },
                         check: () => {
                             return true;
                         },
                         complete: () => {
                             commandProcedure.Enqueue(Command.CreateDeck);
                             
                         });
                    break;
          
                // 카드덱 생성.
                case Command.CreateDeck:
                    CommandInfo.Process(
                        start: () => {
                            CreateDeck();
                        },
                        check: () => {
                            int count = deck.Where(card => card.ListTween.Count != 0).ToList().Count;
                            return count == 0;
                        },
                        complete: () => {
                            commandProcedure.Enqueue(Command.Shuffle_8);
                            
                        });
                    break;

                // 바닥 8장 깔기.
                case Command.Shuffle_8:
                    CommandInfo.Process(
                        start: () => {
                            Shuffle8Card();
                        },
                        check: () => {
                            return GetMoveAllCount() == 0;
                        },
                        complete: () => {
                            commandProcedure.Enqueue(Command.Shuffle_10);
                            
                        });
                    break;
          
                // 열장씩 나누기.
                case Command.Shuffle_10:
                    CommandInfo.Process(
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
                            commandProcedure.Enqueue(Command.Open8);
                            
                        });
                    break;


                // 8장 뒤집기
                case Command.Open8:
                    CommandInfo.Process(
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
                            commandProcedure.Enqueue(Command.CheckJocker);
                            
                        });
                    break;
          

                // 바닥 조커 확인.
                case Command.CheckJocker:
                    CommandInfo.Process(
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
                                commandProcedure.Enqueue(Command.Open1More);
                                
                            }
                            else
                            {
                                commandProcedure.Enqueue(Command.HandUp);
                                
                            }
                        });
                    break;
       
                case Command.Open1More:
                    CommandInfo.Process(
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
                            commandProcedure.Enqueue(Command.CheckJocker);
                        });
                    break;


                case Command.HandUp:
                    CommandInfo.Process(
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
                            commandProcedure.Enqueue(Command.HandOpen);
                            
                        });
                    break;
            
 
                case Command.HandOpen: // 손패를 뒤집습니다.
                    CommandInfo.Process(
                        start: () => {
                            HandOpen();
                        },
                        check: () => {
                            int count = GetMoveCount(hands[0]);
                            return count == 0;
                        },
                        complete: () => {
                            commandProcedure.Enqueue(Command.HandSort);
                            
                        });
                    break;

                case Command.HandSort: // 손패를 정렬합니다.
                    CommandInfo.Process(
                        start: () => HandSort(),
                        check: () => {
                            return GetMoveCount(hands[0]) == 0;
                        },
                        complete: () => {
                            commandProcedure.Enqueue(Command.HitCard);
                            
                        });
                    break;
                
                case Command.HitCard:
                    CommandInfo.Process(
                        start: () => {
                            if (turnUser == Player.COMPUTER)
                            {
                                var list = GetSameMonthCard((int)Board.Player.COMPUTER, hands[(int)Player.COMPUTER][0]);
                                if (list.Count == 3) // 폭탄
                                {
                                    HitBomb((int)Player.COMPUTER, list, list[0]);
                                }
                                else if (list.Count == 4) // 총통
                                {
                                    HitChongtong((int)Player.COMPUTER, list, list[0]);
                                }
                                else
                                {
                                    HitCard((int)Player.COMPUTER, hands[(int)Player.COMPUTER][0]);
                                }
                            }
                        },
                        check: () => {
                            if (CommandInfo.info.hited == false)
                            {
                                return false;
                            }

                            if (CommandInfo.info.hit.Month == 13)
                            {
                                return false;
                            }

                            int count = GetMoveAllCount();
                            return count == 0;
                        },
                        complete: () => {
                            commandProcedure.Enqueue(Command.PopCardDeck);
                        });
                    break;

                case Command.PopCardDeckAndHit:
                    CommandInfo.Process(
                        start: () => {
                            CommandInfo.info.popCard = PopDeckCard();
                            if(CommandInfo.info.popCard == null)
                            {
                                Debug.LogError("PopDeckCard() return null.");
                            }
                        },
                        check: () => {
                            if (CommandInfo.info.popCard &&
                                CommandInfo.info.popCard.Month == 13) // 뒤집어서 조커가 나오면 다시 뽑습니다.
                            {
                                CommandInfo.step = CommandStep.Start;
                                return false;
                            }

                            return 0 == GetMoveAllCount();
                        },
                        complete: () => {
                            commandProcedure.Enqueue(Command.HitCard, turnUser);
                            
                        });
                    break;

                case Command.PopCardDeck:
                    CommandInfo.Process(
                        start: () => {
                            CommandInfo.info.popCard = PopDeckCard(turnUser);
                            if(CommandInfo.info.popCard == null)
                                Debug.LogError("PopDeckCard is null");
                        },
                        check: () => {
                            int count = GetMoveAllCount();
                            if (count == 0)
                            {
                                if (CommandInfo.info.popCard != null &&
                                    CommandInfo.info.popCard.Month == 13) // 뒤집어서 조커가 나오면 다시 뽑습니다.
                                {
                                    CommandInfo.step = CommandStep.Start;
                                    return false;
                                }
                            }

                            return count == 0;
                        },
                        complete: () => {
                            commandProcedure.Enqueue(Command.TakeCardCondition, turnUser);
                            
                        });

                    break;

                case Command.TakeCardCondition: // 카드 획득 처리.
                    CommandInfo.Process(
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
                                        var popup = UIManager.Instance.OpenPopup<UIPopupCardSelect>();
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
                                if(UIManager.Instance.FindPopup<UIPopupMessage>())
                                {
                                    return false;
                                }

                                return true;
                            }
                        },
                        complete: () => {
                            select.Clear();
                            commandProcedure.Enqueue(Command.TakeCard, turnUser);
                        });
                    break;

                case Command.TakeCard: // 카드 획득.
                    CommandInfo.Process(
                        start: () => TackeCardToScore(),
                        check: () => {
                            CommandInfo.info.delta += Time.deltaTime;
                            return CommandInfo.info.delta > 0.2f;
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

                            commandProcedure.Enqueue(Command.TakeToMe, turnUser);
                        });
                    break;

                case Command.TakeToMe:
                    CommandInfo.Process(
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
                                commandProcedure.Enqueue(Command.UpdateScore, turnUser);
                            }
                            else
                            {
                                commandProcedure.Enqueue(Command.StealCard, turnUser);
                            }
                        });

                    break;

                case Command.StealCard: // 카드 뺃기.
                    CommandInfo.Process(
                        start: () => StealCard(),
                        check: () => {
                            int count = GetMoveAllCount();
                            return count == 0;
                        },
                        complete: () => {
                            if (stealCount == 0)
                            {
                                commandProcedure.Enqueue(Command.UpdateScore, turnUser);
                            }
                            else
                            {
                                commandProcedure.Enqueue(Command.StealCard, turnUser);
                            }
                        });
                    break;

                case Command.UpdateScore: // 점수 체크.
                    CommandInfo.Process(
                        start: () => ScoreUpdate(),
                        check: () => {
                            return true;
                        },
                        complete: () => {
                            commandProcedure.Enqueue(Command.ChangeTurn, turnUser);
                        });

                    break;
                case Command.ChangeTurn: // 턴 바꾸기.
                    CommandInfo.Process(
                        start: () => HandSort(),
                        check: () => {
                            int count = GetMoveAllCount();
                            return count == 0;
                        },
                        complete: () => {
                            Command nextCommand = Command.HandSort;

                            // 현재 턴유저의 카드가 0개라면.
                            if (hands[(int)Player.COMPUTER].Count == 0 && hands[(int)Player.USER].Count == 0)
                            {
                                nextCommand = Command.GameOver_Tie;
                                nextCommand = Command.GameOver_Win;
                                nextCommand = Command.GameOver_Lose;
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
                            }

                            commandProcedure.Enqueue(nextCommand, turnUser);
                        });
                    break;

                case Command.GameOver_Win: // 승리 상태 처리.
                    CommandInfo.Process(
                        start: () => { },
                        check: () => {
                            return GetMoveAllCount() == 0;
                        },
                        complete: () => {
                            commandProcedure.Enqueue(Command.StartGame, turnUser);
                        });
                    break;

                case Command.GameOver_Lose: // 패배 상태 처리.
                    CommandInfo.Process(
                        start: () => { },
                        check: () => {
                            return GetMoveAllCount() == 0;
                        },
                        complete: () => {
                            commandProcedure.Enqueue(Command.StartGame, turnUser);
                        });
                    break;

                case Command.GameOver_Tie: // 무승부 상태 처리.
                    CommandInfo.Process(
                        start: () => { },
                        check: () => {
                            return GetMoveAllCount() == 0;
                        },
                        complete: () => {
                            commandProcedure.Enqueue(Command.StartGame, turnUser);
                        });
                    break;
            }

            // 처리 다된 커맨드이면 다음 커맨드 꺼냄.
            if (CommandInfo.step == CommandStep.Done)
            {
                if (commandProcedure.QueueCommand.Count > 0)
                {
                    CommandInfo = commandProcedure.Dequeue();
                }
            }
        }
    }
}
