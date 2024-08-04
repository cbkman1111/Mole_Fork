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


namespace Gostop
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Board : MonoBehaviour
    {
        public struct DebugInfo
        {
            public int num;
            public string msg;
        }

        public enum Player
        {
            NONE = -1,
            USER = 0,
            COMPUTER,
            MAX,
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

        private StateMachineGostop stateMachine = null;
        
        [SerializeField]
        public Card prefabCard = null;
        public Sprite[] sprites = null;
        public Sprite spriteBomb = null;
        public Transform[] hitPosition = null;
        public Transform deckPosition = null;
        public List<Transform> cardPosition = null;
        public BoardPosition[] boardPositions = null;

        private Board.Player turnUser = 0;
        private int stealCount = 0; // 빼앗아올 패.

        private Queue<DebugInfo> listDebug = new Queue<DebugInfo>();
        private int debugNumber = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Board Create(UIMenuGostop menu)
        {
            Board prefab = ResourcesManager.Instance.LoadInBuild<Board>("Board");
            Board board = Instantiate<Board>(prefab);
            if (board != null && board.Init(menu))
            {
                return board;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Init(UIMenuGostop menu)
        {
            this.menu = menu;
            this.stateMachine = StateMachineGostop.Create();

            //stateMachine.Change(State.WAIT);
            turnUser = Player.USER;

            deck = new Stack<Card>();
            hands = new List<Card>[(int)Player.MAX];
            hands[0] = new List<Card>();
            hands[1] = new List<Card>();

            scores = new List<Card>[(int)Player.MAX];
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

            gameScore = new Scores[2];
            gameScore[0] = new Scores();
            gameScore[1] = new Scores();

            menu.SetPosition(this);

            var behavior = GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            return true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void DebugLog(string msg)
        {
            if (turnUser == Player.COMPUTER)
            {
                return;
            }

            debugNumber++;
            DebugInfo info;
            info.num = debugNumber;
            info.msg = msg;

            listDebug.Enqueue(info);

            string messages = string.Empty;
            foreach (var debug in listDebug)
            {
                messages += $"{debug.num}. {debug.msg}\n";
            }

            menu.SetDebug(messages);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        protected void EatLog(KeyValuePair<int, List<Card>> info)
        {
            if (info.Value.Where(c => c.Owner != Player.NONE).ToList().Count() == 0)
            {
                return;
            }

            foreach (var card in info.Value)
            {
                DebugLog($"   > {card.Month}월 {card.KindOfCard} / {card.Owner}");
            }
        }
    }
}
