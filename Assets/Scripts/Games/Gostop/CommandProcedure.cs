using Skell;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gostop
{ 
    /// <summary>
    /// �� ���� ����.
    /// </summary>
    public enum Command
    {
        None = -1,

        StartGame, // ���� ����.

        CreateDeck, // �� ����.
        Shuffle_8,
        Shuffle_10,
        Open8,
        CheckJocker,
        Open1More,
        HandUp,
        HandOpen,
        HandSort,

        HitCard, // ī�� ġ��.
        PopCardDeck, // ī�� ������.
        PopCardDeckAndHit, // ī�� ������ ġ�� ���.
  

        TakeCardCondition, // ���� �� �ִ��� Ȯ��.
        TakeCard, // ī�� ��������.
        TakeToMe, // ���� ��������.
        StealCard, // ī�� ����.

        UpdateScore, // ���� ����.
        ChangeTurn, // �� �ٲٱ�.

        GameOver_Tie, // ���º�.
        GameOver_Win, // ��.
        GameOver_Lose, // ��.
    }

    // ���� ó�� �ܰ�
    public enum CommandStep
    {
        None = -1,
        Start,
        Progress,
        Done,
    }

    public class PlayInfo
    {
        public int index; // �� Ƚ��.
        public Board.Player user; // �� ����.
        public Card popCard; // ������ ���� ����.
        public Card hit; // ���� ģ ī��.
        public bool hited = false; // �ƴ°�.
        public float delta = 0.0f; // �ð�.
        public PlayInfo(int num = 0)
        {
            index = num;
            popCard = null;
            hit = null;
            hited = false; // �ƴ°�.
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
        /// ����, Ʈ���� ���� ture, �Ϸ� ������ ȣ��Ǹ� ���¸� ó���մϴ�.
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
        /// ���� ����.
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
        /// ���� ������.
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
