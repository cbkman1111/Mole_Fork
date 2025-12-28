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
        StealCardAndPopDeck, // 카드 뺏고 턴 가져오기.

        //UpdateScore, // 점수 갱신.
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
        Next,
    }

    [System.Serializable] // 인스펙터에서 보려고 추가
    public class PlayInfo
    {
        public int turnIndex;           // 몇 번째 턴인가
        public Board.Player player;     // 행동 주체
        public Card popCard; // 덱에서 꺼낸 정보.
        public Card hit; // 최초 친 카드.
        public bool isHit = false; // 쳤는가.
        public float delta = 0.0f; // 시간.

        public PlayInfo(Board.Player player = Board.Player.None)
        {
            this.player = player;
            this.turnIndex = 0;
            this.isHit = false;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [System.Serializable] // 인스펙터에서 보려고 추가
    public class CommandInfo
    {
        public Command CommandType { get; private set; }
        public CommandStep Step { get; set; }
        public PlayInfo Info { get; private set; }

        public CommandInfo(Command type, Board.Player player)
        {
            CommandType = type;
            Step = CommandStep.None;
            Info = new PlayInfo(player);
        }

        /// <summary>
        /// 상태머신 처리: Start -> Progress(반복) -> Done
        /// </summary>
        /// <returns>명령이 완전히 끝났으면 true 반환</returns>
        public void Execute(Action onStart, Func<bool> onUpdate, Action onComplete)
        {
            switch (Step)
            {
                case CommandStep.None:
                    Step = CommandStep.Start;
                    break;

                case CommandStep.Start:
                    onStart?.Invoke(); // 시작 로직 실행
                    Step = CommandStep.Progress;
                    break;

                case CommandStep.Progress:
                    var done = onUpdate.Invoke();
                    if(done == true)
                        Step = CommandStep.Done;
                    break;

                case CommandStep.Done:
                    onComplete?.Invoke(); // 종료 로직 실행
                    Step = CommandStep.Next;
                    break;

                default:
                    break;
            }
        }
    }

    public class CommandProcedure
    {
        private Queue<CommandInfo> _commandQueue = new Queue<CommandInfo>();

        // 현재 실행 중인 명령 (디버깅용)
        public CommandInfo CurrentCommand { get; private set; }

        // [싱글턴 대신 프로퍼티 접근 권장] 
        // 외부에서 참조가 필요하다면 CurrentCommand.Info를 쓰도록 유도

        public CommandProcedure()
        {
            _commandQueue.Clear();
        }

        public void Clear()
        {
            _commandQueue.Clear();
            CurrentCommand = null;
        }

        public void Enqueue(Command type, Board.Player player = Board.Player.None)
        {
            var cmd = new CommandInfo(type, player);
            _commandQueue.Enqueue(cmd);
        }

        /// <summary>
        /// 다음 명령을 꺼내서 CurrentCommand로 설정
        /// </summary>
        public CommandInfo MoveNext()
        {
            if (_commandQueue.Count > 0)
            {
                CurrentCommand = _commandQueue.Dequeue();
                return CurrentCommand;
            }

            CurrentCommand = null;
            return null;
        }
    }
}
