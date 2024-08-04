using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gostop
{ 
    /// <summary>
    /// �� ���� ����.
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

        CARD_HIT, // ī�� ġ��.
        CARD_POP, // ī�� ������.

        EAT_CHECK, // �Դ� ����.
        EAT, // �Ա�.
        STEAL, // ī�� ����.

        SCORE_UPDATE, // ���� ����.
        TURN_CHECK, // �� �ٲٱ�.

        GAME_OVER_TIE, // ���º�.
    }

    // ���� ó�� �ܰ�
    public enum StateEvent
    {
        INIT = 0,
        START,
        PROGRESS,
        DONE,
    }


    public class PlayInfo
    {
        public int index; // �� Ƚ��.
        public Board.Player user; // �� ����.
        public Card popCard; // ������ ���� ����.
        public Card hit; // ���� ģ ī��.
        public bool hited = false; // �ƴ°�.

        public PlayInfo(int num = 0)
        {
            index = num;
            popCard = null;
            hit = null;
            hited = false; // �ƴ°�.
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
        /// ����, Ʈ���� ���� ture, �Ϸ� ������ ȣ��Ǹ� ���¸� ó���մϴ�.
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
