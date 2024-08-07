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
    public enum Command
    {
        None = -1,

        StartGame, // 게임 생성.

        CreateDeck, // 덱 생성.
        Shuffle_8,
        Shuffle_10,
        Open8,
        CheckJocker,
        Open1More,
        HandUp,
        HandOpen,
        HandSort,

        HitCard, // 카드 치기.
        PopCardDeck, // 카드 뒤집기.
        PopCardDeckAndHit, // 카드 뒤집고 치기 대기.
  

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
    public enum CommandStep
    {
        None = -1,
        Start,
        Progress,
        Done,
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
    public class CommandInfo
    {
        public Command type;
        public CommandStep step;
        public PlayInfo info;

        public CommandInfo()
        {
            type = Command.None;
            step = CommandStep.None;
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
            switch (step)
            {
                case CommandStep.None:
                    step = CommandStep.Start;
                    break;

                case CommandStep.Start:
                    step = CommandStep.Progress;
                    start();
                    break;

                case CommandStep.Progress:
                    if (check() == true)
                        step = CommandStep.Done;
                    break;

                case CommandStep.Done:
                    complete();
                    break;
            }
        }
    }

    public class CommandProcedure
    {
        public Queue<CommandInfo> QueueCommand { get; set; }

        public static CommandProcedure Create()
        {
            CommandProcedure ret = new CommandProcedure();
            if (ret != null && ret.Init() == true)
            {
                return ret;
            }

            return null;
        }

        public bool Init()
        {
            QueueCommand = new ();
            return true;
        }

        public void Clear()
        {
            QueueCommand.Clear();
        }

        /// <summary>
        /// 상태 변경.
        /// </summary>
        /// <param name="state"></param>
        public void Enqueue(Command command, Board.Player player = Board.Player.NONE)
        {
            CommandInfo info = new CommandInfo() {
                type = command,
                step = CommandStep.None,
                info = new PlayInfo()
                {
                    index = 0,
                    user = player,
                    popCard = null,
                    hit = null,
                    hited = false,
                },
            };

            QueueCommand.Enqueue(info);
        }

        /// <summary>
        /// 상태 꺼내기.
        /// </summary>
        /// <returns></returns>
        public CommandInfo Dequeue()
        {
            if (QueueCommand.Count > 0)
            {
                var command = QueueCommand.Dequeue();
                return command;
            }
            
            return null;
        }
    }

}
