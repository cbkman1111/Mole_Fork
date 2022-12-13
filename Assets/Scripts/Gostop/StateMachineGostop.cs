using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각 턴의 정보.
/// </summary>
/// 
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

// 상태 처리 단계
public enum StateEvent
{
    INIT = 0,
    START,
    PROGRESS,
    DONE,
}

public class TurnState
{
    public int index; // 턴 횟수.
    public Board.Player user; // 턴 유저.
    public Card pop; // 덱에서 꺼낸 정보.
    public Card hit; // 최초 친 카드.
    public bool hited = false; // 쳤는가.
    private Stack<StateInfo> stack = null;

    public TurnState(int num = 0)
    {
        index = num;
        pop = null;
        hit = null;
        hited = false; // 쳤는가.
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
    /// 시작, 트리거 리턴 ture, 완료 순으로 호출되며 상태를 처리합니다.
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
