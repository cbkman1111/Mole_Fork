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
            
            hands = new CardList[(int)Player.Max];
            hands[0] = new CardList();
            hands[1] = new CardList();

            scores = new List<Card>[(int)Player.Max];
            scores[0] = new List<Card>();
            scores[1] = new List<Card>();

            select = new CardList();
            listEat = new CardList();
            bottoms = new Dictionary<int, CardList>
            {
                { 1, new CardList() },
                { 2, new CardList() },
                { 3, new CardList() },
                { 4, new CardList() },
                { 5, new CardList() },
                { 6, new CardList() },
                { 7, new CardList() },
                { 8, new CardList() },
                { 9, new CardList() },
                { 10, new CardList() },
                { 11, new CardList() },
                { 12, new CardList() },
                { 13, new CardList() }
            };

            gameScore = new Score[2];
            gameScore[0] = new Score();
            gameScore[1] = new Score();
            //behaviorTree = GetComponent<BehaviorTree>();
            return true;
        }
    }
}
