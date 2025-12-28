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
        public CommandInfo CommnadLast = null;

        public const int MONTH_JOKER = 13;
        public const int MONTH_BOMB = 100;
        public const int MAX_MONTH = 12;

        /// <summary>
        /// 내턴
        /// </summary>
        /// <returns></returns>
        public bool MyTurn()
        {
            return turnUser == Player.Me;
        }

        /// <summary>
        /// 게임 시작.
        /// </summary>
        public void StartGame()
        {
            commandProcedure.Enqueue(Command.StartGame);
            CommandInfo = commandProcedure.MoveNext();
        }

        /// <summary>
        /// 움직이는 카드 존재 확인.
        /// </summary>
        /// <param name="deck"></param>
        /// <returns></returns>
        private int GetMoveCount(Stack<Card> stack)
        {
            int count = stack.Where(card => card.IsAnimating == true).ToList().Count;
            return count;
        }

        float updateDelta = 0.0f;

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

            updateDelta += Time.deltaTime;
            if (updateDelta < 0.1f)
                return;

            updateDelta = 0f;

            Command commandType = CommandInfo.CommandType;
            switch (commandType)
            {
                // 게임 시작.
                case Command.StartGame:
                    CommandInfo.Execute(
                         onStart: () => {
                             DestroyAllCards();

                             gameScore[0].Init();
                             gameScore[1].Init();
                             ScoreUpdate();
                         },
                         onUpdate: () => {
                             return true;
                         },
                         onComplete: () => {
                             commandProcedure.Enqueue(Command.CreateDeck);

                         });
                    break;
          
                // 카드덱 생성.
                case Command.CreateDeck:
                    CommandInfo.Execute(
                        onStart: () => {
                            CreateDeck();
                        },
                        onUpdate: () => {
                            int count = GetMoveCount(deck);
                            return count == 0;
                        },
                        onComplete: () => {
                            commandProcedure.Enqueue(Command.Shuffle_8);
                            
                        });
                    break;

                // 바닥 8장 깔기.
                case Command.Shuffle_8:
                    CommandInfo.Execute(
                        onStart: () => {
                            Shuffle8Card();
                        },
                        onUpdate: () => {
                            return GetMoveAllCount() == 0;
                        },
                        onComplete: () => {
                            commandProcedure.Enqueue(Command.Shuffle_10);
                            
                        });
                    break;
          
                // 열장씩 나누기.
                case Command.Shuffle_10:
                    CommandInfo.Execute(
                        onStart: () => {
                            Shuffle10Card();
                        },
                        onUpdate: () => {
                            int count = 0;
                            count += hands[0].MoveCount();//
                            count += hands[1].MoveCount();//
                            return count == 0;
                        },
                        onComplete: () => {
                            commandProcedure.Enqueue(Command.Open8);
                            
                        });
                    break;


                // 8장 뒤집기
                case Command.Open8:
                    CommandInfo.Execute(
                        onStart: () => {
                            FlipCard8();
                        },
                        onUpdate: () => {
                            int count = 0;
                            foreach (var slot in bottoms) {
                                count += slot.Value.MoveCount();
                            }

                            return count == 0;
                        },
                        onComplete: () => {
                            commandProcedure.Enqueue(Command.CheckJocker);
                            
                        });
                    break;
          

                // 바닥 조커 확인.
                case Command.CheckJocker:
                    CommandInfo.Execute(
                        onStart: () => {
                            CheckJoker();
                        },
                        onUpdate: () => {
                            int countMove = 0;
                            foreach (var slot in bottoms)
                            {
                                countMove += slot.Value.MoveCount(); 
                            }

                            return countMove == 0;
                        },
                        onComplete: () => {
                            int Count = 0;
                            int jockerCount = 0;
                            foreach (var slot in bottoms)
                            {
                                int n = slot.Value.GetList(13).Count;
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
                    CommandInfo.Execute(
                        onStart: () => {
                            PopDeckCard(Player.None);
                        },
                        onUpdate: () => {
                            int count = 0;

                            foreach (var slot in bottoms)
                            {
                                count += slot.Value.MoveCount();// GetMoveCount();
                            }

                            return count == 0;
                        },
                        onComplete: () => {
                            commandProcedure.Enqueue(Command.CheckJocker);
                        });
                    break;


                case Command.HandUp:
                    CommandInfo.Execute(
                        onStart: () => {
                            HandsUp();
                        },
                        onUpdate: () => {
                            int count = 0;
                            count += hands[0].MoveCount();
                            count += hands[1].MoveCount(); 
                            return count == 0;
                        },
                        onComplete: () => {
                            commandProcedure.Enqueue(Command.HandOpen);
                            
                        });
                    break;
            
 
                case Command.HandOpen: // 손패를 뒤집습니다.
                    CommandInfo.Execute(
                        onStart: () => {
                            HandOpen();
                        },
                        onUpdate: () => {
                            int count = hands[0].MoveCount();// GetMoveCount(hands[0].List);
                            return count == 0;
                        },
                        onComplete: () => {
                            commandProcedure.Enqueue(Command.HandSort);
                            
                        });
                    break;

                case Command.HandSort: // 손패를 정렬합니다.
                    CommandInfo.Execute(
                        onStart: () => HandSort(),
                        onUpdate: () => {
                            return hands[0].MoveCount() == 0;
                        },
                        onComplete: () => {
                            commandProcedure.Enqueue(Command.HitCard);
                            
                        });
                    break;
                
                case Command.HitCard:
                    CommandInfo.Execute(
                        onStart: () => {
                            if (turnUser == Player.Enemy)
                            {
                                int turnIndex = (int)turnUser;
                                var list = GetSameMonthCard(turnIndex, hands[turnIndex][0]);
                                if (list.Count == 3) // 폭탄
                                {
                                    HitBomb(turnIndex, list, list[0]);
                                }
                                else if (list.Count == 4) // 총통
                                {
                                    HitChongtong(turnIndex, list, list[0]);
                                }
                                else
                                {
                                    HitCard(turnIndex, hands[turnIndex][0]);
                                }
                            }
                        },
                        onUpdate: () => {
                            // 칠때까지 대기.
                            if (CommandInfo.Info.isHit == false)
                                return false;

                            // 조커를 낸 경우면 조금 기다렸다가 패 훔쳐오기 처리.
                            if(CommandInfo.Info.hit.Month == 13)
                            {
                                CommandInfo.Info.delta += Time.deltaTime;
                                if(CommandInfo.Info.delta < 0.1f)
                                {
                                    return false;
                                }
                            }
                            
                            int count = GetMoveAllCount();
                            return count == 0;
                        },
                        onComplete: () => {
                            if (CommandInfo.Info.hit.Month == 13)
                            {
                                commandProcedure.Enqueue(Command.StealCardAndPopDeck, turnUser);
                            }
                            else
                            {
                                commandProcedure.Enqueue(Command.PopCardDeck);
                            }
                        });
                    break;

                case Command.PopCardDeckAndHit:
                    CommandInfo.Execute(
                        onStart: () => {
                            CommandInfo.Info.popCard = PopDeckCard();
                            if(CommandInfo.Info.popCard == null)
                            {
                                Debug.LogError("PopDeckCard() return null.");
                            }
                        },
                        onUpdate: () => {
                            if (CommandInfo.Info.popCard &&
                                CommandInfo.Info.popCard.Month == 13) // 뒤집어서 조커가 나오면 다시 뽑습니다.
                            {
                                CommandInfo.Step = CommandStep.Start;
                                return false;
                            }

                            return 0 == GetMoveAllCount();
                        },
                        onComplete: () => {
                            commandProcedure.Enqueue(Command.HitCard, turnUser);
                            
                        });
                    break;

                case Command.PopCardDeck:
                    CommandInfo.Execute(
                        onStart: () => {
                            CommandInfo.Info.popCard = PopDeckCard(turnUser);
                            if(CommandInfo.Info.popCard == null)
                                Debug.LogError("PopDeckCard is null");
                        },
                        onUpdate: () => {
                            int count = GetMoveAllCount();
                            if (count == 0)
                            {
                                if (CommandInfo.Info.popCard != null &&
                                    CommandInfo.Info.popCard.Month == 13) // 뒤집어서 조커가 나오면 다시 뽑습니다.
                                {
                                    CommandInfo.Step = CommandStep.Start;
                                    return false;
                                }
                            }

                            return count == 0;
                        },
                        onComplete: () => {
                            commandProcedure.Enqueue(Command.TakeCardCondition, turnUser);
                            
                        });

                    break;

                case Command.TakeCardCondition: // 카드 획득 처리.
                    CommandInfo.Execute(
                        onStart: () => TakeCardCondition(),
                        onUpdate: () => {
                            if (select.Count == 2)
                            {
                                var list = select;
                                if (turnUser == Player.Enemy)
                                {
                                    list[0].Owner = Player.Enemy;
                                    listEat.Add(list[0]);
                                    select.Clear();
                                }
                                else
                                {
                                    if (list[0].KindOfCard != list[1].KindOfCard)
                                    {
                                        var popup = UIManager.Instance.OpenPopup<UIPopupCardSelect>();
                                        popup.Init(list[0], list[1], (Card selectCard) => {
                                            selectCard.Owner = turnUser;
                                            listEat.Add(selectCard);
                                            select.Clear();
                                        });
                                    }
                                    else
                                    {
                                        list[0].Owner = turnUser;
                                        listEat.Add(list[0]);
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
                        onComplete: () => {
                            select.Clear();
                            commandProcedure.Enqueue(Command.TakeCard, turnUser);
                        });
                    break;

                case Command.TakeCard: // 카드 획득.
                    CommandInfo.Execute(
                        onStart: () => TackeCardToScore(),
                        onUpdate: () => {
                            CommandInfo.Info.delta += Time.deltaTime;
                            return CommandInfo.Info.delta > 0.2f;
                        },
                        onComplete: () => {
                            // 주인 없는 카드로 설정.
                            foreach (var kindSlot in bottoms)
                            {
                                var list = kindSlot.Value;
                                foreach (var card in list) {
                                    card.Owner = Player.None;
                                }
                            }

                            commandProcedure.Enqueue(Command.TakeToMe, turnUser);
                        });
                    break;

                case Command.TakeToMe:
                    CommandInfo.Execute(
                        onStart: () => {
                            int count = 0;
                            int total = listEat.Count;

                            foreach (var card in listEat)
                            {
                                TackCard(card, total - count); // 카드 획득.
                                count++;
                            }

                            listEat.Clear();
                            ScoreUpdate();
                        },
                        onUpdate: () => {
                            return GetMoveAllCount() == 0;
                        },
                        onComplete: () => {
                            if (stealCount == 0)
                            {
                                commandProcedure.Enqueue(Command.ChangeTurn, turnUser);
                            }
                            else
                            {
                                commandProcedure.Enqueue(Command.StealCard, turnUser);
                            }
                        });

                    break;

                case Command.StealCardAndPopDeck: // 카드 뺃고 턴 가져오기.
                    CommandInfo.Execute(
                        onStart: () => {
                            StealCard();
                            ScoreUpdate();
                        },
                        onUpdate: () => {
                            int count = GetMoveAllCount();
                            return count == 0;
                        },
                        onComplete: () => {
                            if (stealCount == 0)
                            {
                                commandProcedure.Enqueue(Command.PopCardDeckAndHit, turnUser);
                            }
                            else
                            {
                                commandProcedure.Enqueue(Command.StealCardAndPopDeck, turnUser);
                            }
                        });
                    break;

                case Command.StealCard: // 카드 뺃기.
                    CommandInfo.Execute(
                        onStart: () =>
                        {
                            StealCard();
                            ScoreUpdate();
                        },
                        onUpdate: () => {
                            int count = GetMoveAllCount();
                            return count == 0;
                        },
                        onComplete: () => {
                            if (stealCount == 0)
                            {
                                commandProcedure.Enqueue(Command.ChangeTurn, turnUser);
                            }
                            else
                            {
                                commandProcedure.Enqueue(Command.StealCard, turnUser);
                            }
                        });
                    break;

                case Command.ChangeTurn: // 턴 바꾸기.
                    CommandInfo.Execute(
                        onStart: () => HandSort(),
                        onUpdate: () => {
                            int count = GetMoveAllCount();
                            return count == 0;
                        },
                        onComplete: () => {
                            Command nextCommand = Command.HandSort;

                            if (hands[(int)Player.Enemy].Count == 0 && hands[(int)Player.Me].Count == 0)
                            {
                                nextCommand = Command.GameOver_Tie;
                                nextCommand = Command.GameOver_Win;
                                nextCommand = Command.GameOver_Lose;
                            }
                            else 
                            {
                                if (turnUser == Player.Me)
                                {
                                    turnUser = Player.Enemy;
                                }
                                else
                                {
                                    turnUser = Player.Me;
                                }
                            }

                            commandProcedure.Enqueue(nextCommand, turnUser);
                        });
                    break;

                case Command.GameOver_Win: // 승리 상태 처리.
                    CommandInfo.Execute(
                        onStart: () => { },
                        onUpdate: () => {
                            return GetMoveAllCount() == 0;
                        },
                        onComplete: () => {
                            commandProcedure.Enqueue(Command.StartGame, turnUser);
                        });
                    break;

                case Command.GameOver_Lose: // 패배 상태 처리.
                    CommandInfo.Execute(
                        onStart: () => { },
                        onUpdate: () => {
                            return GetMoveAllCount() == 0;
                        },
                        onComplete: () => {
                            commandProcedure.Enqueue(Command.StartGame, turnUser);
                        });
                    break;

                case Command.GameOver_Tie: // 무승부 상태 처리.
                    CommandInfo.Execute(
                        onStart: () => { },
                        onUpdate: () => {
                            return GetMoveAllCount() == 0;
                        },
                        onComplete: () => {
                            commandProcedure.Enqueue(Command.StartGame, turnUser);
                        });
                    break;
            }

            // 처리 다된 커맨드이면 다음 커맨드 꺼냄.
            if (CommandInfo != null && CommandInfo.Step == CommandStep.Next)
            {
                CommnadLast = CommandInfo;
                CommandInfo = commandProcedure.MoveNext();
            }
        }
    }
}
