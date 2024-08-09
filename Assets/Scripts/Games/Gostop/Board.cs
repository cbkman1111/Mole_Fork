using DG.Tweening;
using System;

using System.Collections.Generic;
using System.Linq;
using Common.Global;
using UI.Menu;
using UI.Popup;
using UnityEngine;
using static UnityEditor.Rendering.InspectorCurveEditor;
using UnityEditor;
using BehaviorDesigner.Runtime;
using Scenes;


namespace Gostop
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Board : MonoBehaviour
    {
        public enum Player
        {
            None = -1,
            Player = 0,
            Enemy,
            Max,
        };

        public enum BoardPositions
        {
            GWANG = 0,
            MUNG,
            THEE,
            PEE,
            RECIVE,
            HAND,
        }

        private CommandProcedure commandProcedure = null;
        //private BehaviorTree behaviorTree = null;

        public BoardSetting setting;

        [SerializeField]
        public Card prefabCard = null;
        public Sprite[] sprites = null;
        public Sprite spriteBomb = null;
        public Transform[] hitPosition = null;
        public Transform deckPosition = null;
        public Vector3 Deck => deckPosition.position;

        public List<Transform> cardPosition = null;
        public BoardPosition[] boardPositions = null;

        private Board.Player turnUser = 0;
        private int stealCount = 0; // 빼앗아올 패.

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Board Create(Action<Player, Score> updateScore)
        {
            Board prefab = ResourcesManager.Instance.LoadInBuild<Board>("Board");
            Board board = Instantiate<Board>(prefab);
            if (board != null && board.Init(updateScore))
            {
                return board;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Init(Action<Player, Score> score)
        {
            BoardSettingContainer container = ResourcesManager.Instance.LoadInBuild<BoardSettingContainer>("BoardSetting");
            setting = container.setting.DeepClone();

            updateScore = score;
            commandProcedure = CommandProcedure.Create();
            turnUser = Player.Player;
            deck = new Stack<Card>();
            
            hands = new List<Card>[(int)Player.Max];
            hands[0] = new List<Card>();
            hands[1] = new List<Card>();

            scores = new List<Card>[(int)Player.Max];
            scores[0] = new List<Card>();
            scores[1] = new List<Card>();

            select = new List<Card>();
            listEat = new List<Card>();
            bottoms = new Dictionary<int, List<Card>>();
            bottoms.Add(1, new List<Card>());
            bottoms.Add(2, new List<Card>());
            bottoms.Add(3, new List<Card>());
            bottoms.Add(4, new List<Card>());
            bottoms.Add(5, new List<Card>());
            bottoms.Add(6, new List<Card>());
            bottoms.Add(7, new List<Card>());
            bottoms.Add(8, new List<Card>());
            bottoms.Add(9, new List<Card>());
            bottoms.Add(10, new List<Card>());
            bottoms.Add(11, new List<Card>());
            bottoms.Add(12, new List<Card>());
            bottoms.Add(13, new List<Card>());

            gameScore = new Score[2];
            gameScore[0] = new Score();
            gameScore[1] = new Score();
            //behaviorTree = GetComponent<BehaviorTree>();
            return true;
        }
    }
}
