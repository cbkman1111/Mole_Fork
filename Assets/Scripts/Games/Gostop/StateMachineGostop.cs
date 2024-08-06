using Skell;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gostop
{ 
    /// <summary>
    /// 각 턴의 정보.
    /// </summary>
    public enum State
    {
        None = -1,

        MakeGame, // 게임 생성.

        HitCard, // 카드 치기.
        PopCardDeck, // 카드 뒤집기.
        PopCardDeckAndHit, // 카드 뒤집고 치기 대기.
        SortHands,

        TakeCardCondition, // 먹을 수 있는지 확인.
        TakeCard, // 카드 가져오기.
        TakeToMe, // 내게 가져오기.
        StealCard, // 카드 뺏기.

        UpdateScore, // 점수 갱신.
        ChangeTurn, // 턴 바꾸기.

        GameOver_Tie, // 무승부.
        GameOver_Win, // 승.
        GameOver_Lose, // 패.
    }

    // 상태 처리 단계
    public enum StateEvent
    {
        INIT = 0,
        START,
        PROGRESS,
        DONE,
    }


    public class PlayInfo
    {
        public int index; // 턴 횟수.
        public Board.Player user; // 턴 유저.
        public Card popCard; // 덱에서 꺼낸 정보.
        public Card hit; // 최초 친 카드.
        public bool hited = false; // 쳤는가.
        public float delta = 0.0f; // 시간.
        public PlayInfo(int num = 0)
        {
            index = num;
            popCard = null;
            hit = null;
            hited = false; // 쳤는가.
            user = Board.Player.NONE;
            delta = 0.0f;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class StateInfo
    {
        public State state;
        public StateEvent evt;
        public PlayInfo info;

        public StateInfo()
        {
            state = State.None;
            evt = StateEvent.INIT;
            info = new PlayInfo();
        }

        /// <summary>
        /// 시작, 트리거 리턴 ture, 완료 순으로 호출되며 상태를 처리합니다.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="check"></param>
        /// <param name="complete"></param>
        public void Process(Action start, Func<bool> check, Action complete)
        {
            switch (evt)
            {
                case StateEvent.INIT:
                    evt = StateEvent.START;
                    break;

                case StateEvent.START:
                    evt = StateEvent.PROGRESS;
                    start();
                    break;

                case StateEvent.PROGRESS:
                    if (check() == true)
                        evt = StateEvent.DONE;
                    break;

                case StateEvent.DONE:
                    complete();
                    break;
            }
        }
        
    }

    public class StateMachineGostop
    {
        public Stack<StateInfo> Stack { get; set; }

        public static StateMachineGostop Create()
        {
            StateMachineGostop ret = new StateMachineGostop();
            if (ret != null && ret.Init() == true)
            {
                return ret;
            }

            return null;
        }

        public bool Init()
        {
            Stack = new ();
            return true;
        }

        public void Clear()
        {
            Stack.Clear();
        }

        /// <summary>
        /// 상태 변경.
        /// </summary>
        /// <param name="state"></param>
        public void Change(State state, Board.Player player)
        {
            StateInfo info = new StateInfo() {
                state = state,
                evt = StateEvent.INIT,
                info = new PlayInfo()
                {
                    index = 0,
                    user = player,
                    popCard = null,
                    hit = null,
                    hited = false,
                },
            };

            Stack.Push(info);
        }

        /// <summary>
        /// 상태 꺼내기.
        /// </summary>
        /// <returns></returns>
        public StateInfo PopState()
        {
            if (Stack.Count > 0)
            {
                var info = Stack.Pop();
                return info;
            }
            
            return null;
        }
    }

}
