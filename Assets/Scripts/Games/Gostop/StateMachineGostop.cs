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
        NONE = -1,
        //WAIT = 0,
        //START_GAME,
        //CREATE_DECK,
        //SHUFFLE_8,
        //SHUFFLE_10,
        //OPEN_8,
        //OPEN_1_MORE,
        //CHECK_JORKER,
        //HANDS_UP,
        //HANDS_OPEN,
        
        HANDS_SORT,

        CARD_HIT, // 카드 치기.
        CARD_POP, // 카드 뒤집기.

        EAT_CHECK, // 먹는 판정.
        EAT, // 먹기.
        STEAL, // 카드 뺏기.

        SCORE_UPDATE, // 점수 갱신.
        TURN_CHECK, // 턴 바꾸기.

        GAME_OVER_TIE, // 무승부.
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

        public PlayInfo(int num = 0)
        {
            index = num;
            popCard = null;
            hit = null;
            hited = false; // 쳤는가.
            user = Board.Player.NONE;
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
            state = State.NONE;
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
                    {
                        evt = StateEvent.DONE;
                    }
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

        public void Change(State state)
        {
            StateInfo info = new StateInfo() {
                state = state,
                evt = StateEvent.INIT,
                info = new PlayInfo()
            };

            Stack.Push(info);
        }

        /// <summary>
        /// 
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
