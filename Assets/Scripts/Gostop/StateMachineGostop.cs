using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineGostop
{
    public enum StateEvent
    {
        INIT = 0,
        START,
        PROGRESS,
        DONE,
    }

    public enum State
    {
        WAIT = 0,

        CREATE_DECK,

        SHUFFLE_8,
        SHUFFLE_10,

        OPEN_8,

        CHECK_JORKER,

        HANDS_UP,
        HANDS_OPEN,
        HANDS_SORT,

        CARD_HIT, // 카드 치기.
        CARD_POP, // 카드 뒤집기.

        EAT_CHECK, // 먹는 판정.
        SCORE_UPDATE, // 점수 갱신.
        TURN_CHECK, // 턴 바꾸기.

        GAME_OVER_TIE, // 무승부.
    }

    public Stack<TurnInfo> stack;

    /// <summary>
    /// 각 턴의 정보.
    /// </summary>
    public class TurnInfo
    {
        public int index; // 턴 횟수.
        public Board.Player userIndex; // 턴 유저.
        public Card pop; // 덱에서 꺼낸 정보.
        public Card hit; // 최초 친 카드.
        public bool hited = false; // 쳤는가.
        private Stack<StateInfo> stack = null;
        

        public TurnInfo(int num = 0)
        {
            index = num;
            pop = null;
            hit = null;
            hited = false; // 쳤는가.
            stack = new Stack<StateInfo>();
            userIndex = Board.Player.NONE;
            //queue = new Queue<StateInfo>();
        }

        public void AddState(StateInfo info)
        {
            stack.Push(info);
            //queue.Enqueue(info);
        }

        public StateInfo GetCurrentStateInfo()
        {
            return stack.Peek();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StateInfo {
        public State state;
        public StateEvent evt;

        public StateInfo()
        {
            state = State.WAIT;
            evt = StateEvent.INIT;
        }
    }

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
        stack = new Stack<TurnInfo>();
        stack.Push(new TurnInfo());

        return true;
    }

    public void Clear()
    {
        stack.Clear();
    }

    public void AddTurn(Board.Player userIndex)
    {
        TurnInfo info = new TurnInfo(stack.Count);
        info.userIndex = userIndex;
        stack.Push(info);
    }

    public void Change(State state)
    {
        StateInfo info = new StateInfo();
        info.state = state;
        info.evt = StateEvent.INIT;

        var turnInfo = GetCurrturnInfo();
        turnInfo.AddState(info);
    }

    public TurnInfo GetCurrturnInfo()
    {
        return stack.Peek();
    }

    public void Process(Action start, Func<bool> trigger, Action compete)
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
                    if (trigger() == true)
                    {
                        info.evt = StateEvent.DONE;
                    }
                    break;

                case StateEvent.DONE:
                    //queue.Dequeue();
                    compete();
                    break;
            }
        }
    }
}
