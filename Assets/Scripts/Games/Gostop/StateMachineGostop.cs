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

        WAIT = 0,

        START_GAME,

        CREATE_DECK,

        SHUFFLE_8,
        SHUFFLE_10,

        OPEN_8,
        OPEN_1_MORE,

        CHECK_JORKER,

        HANDS_UP,
        HANDS_OPEN,
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

    public class TurnState
    {
        public int index; // �� Ƚ��.
        public Board.Player user; // �� ����.
        public Card pop; // ������ ���� ����.
        public Card hit; // ���� ģ ī��.
        public bool hited = false; // �ƴ°�.
        private Stack<StateInfo> stack = null;

        public TurnState(int num = 0)
        {
            index = num;
            pop = null;
            hit = null;
            hited = false; // �ƴ°�.
            stack = new Stack<StateInfo>();
            user = Board.Player.NONE;
        }

        public void AddState(StateInfo info)
        {
            stack.Push(info);
        }

        public StateInfo GetCurrentStateInfo()
        {
            return stack.Peek();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StateInfo
    {
        public State state;
        public StateEvent evt;

        public StateInfo()
        {
            state = State.WAIT;
            evt = StateEvent.INIT;
        }
    }

    public class StateMachineGostop
    {
        public Stack<TurnState> Stack { get; set; }

        public static StateMachineGostop Create()
        {
            StateMachineGostop ret = new StateMachineGostop();
            if (ret != null && ret.Init())
            {
                return ret;
            }

            return null;
        }

        public bool Init()
        {
            Stack = new Stack<TurnState>();
            Stack.Push(new TurnState());

            return true;
        }

        public void Clear()
        {
            Stack.Clear();
        }

        public void AddTurn(Board.Player userIndex)
        {
            TurnState turn = new TurnState(Stack.Count);
            turn.user = userIndex;
            Stack.Push(turn);
        }

        public void Change(State state)
        {
            StateInfo info = new StateInfo();
            info.state = state;
            info.evt = StateEvent.INIT;

            var turnInfo = GetCurrturnInfo();
            turnInfo.AddState(info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TurnState GetCurrturnInfo()
        {
            return Stack.Peek();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StateInfo GetCurrStateInfo()
        {
            var trun = GetCurrturnInfo();
            return trun.GetCurrentStateInfo();
        }

        /// <summary>
        /// ����, Ʈ���� ���� ture, �Ϸ� ������ ȣ��Ǹ� ���¸� ó���մϴ�.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="check"></param>
        /// <param name="complete"></param>
        public void Process(Action start, Func<bool> check, Action complete)
        {
            var turn = GetCurrturnInfo();
            var info = turn.GetCurrentStateInfo();
            if (info != null)
            {
                switch (info.evt)
                {
                    case StateEvent.INIT:
                        info.evt = StateEvent.START;
                        break;

                    case StateEvent.START:
                        info.evt = StateEvent.PROGRESS;
                        start();
                        break;

                    case StateEvent.PROGRESS:
                        if (check() == true)
                        {
                            info.evt = StateEvent.DONE;
                        }
                        break;

                    case StateEvent.DONE:
                        complete();
                        break;
                }
            }
        }
    }

}
