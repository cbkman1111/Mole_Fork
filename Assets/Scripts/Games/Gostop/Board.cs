using Common.Global;
using System;
using System.Collections.Generic;
using UnityEngine;


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
            Me = 0,
            Enemy,
            Max,
        };

        public enum BoardArea
        {
            GWANG = 0,
            MUNG,
            THEE,
            PEE,
            RECIVE,
            HAND,
        }

        [Header("Settings")]
        [SerializeField] public BoardSetting setting;
        [SerializeField] public Card prefabCard;

        [Header("Resources")]
        [SerializeField] public Sprite[] sprites = null;
        [SerializeField] public Sprite spriteBomb = null;

        [SerializeField]
        private CommandProcedure commandProcedure = null;

        [Header("Positions")]
        [SerializeField] public Transform[] hitPosition = null;
        [SerializeField] public List<Transform> cardPosition = null;
        [SerializeField] public Transform deckPosition = null;
        
        public Vector3 Deck => deckPosition.position;
        public BoardPosition[] boardPositions = null;

        private Player turnUser = Player.Me;
        private int stealCount = 0; // 빼앗아올 패.

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Board Create(Action<Player, Score> updateScore)
        {
            Board prefab = ResourcesManager.Instance.LoadInBuild<Board>("Board");
            if (prefab == null)
            {
                Debug.LogError("[Board] Failed to load Board prefab.");
                return null;
            }

            Board board = Instantiate<Board>(prefab);
            if (board != null && board.Init(updateScore))
            {
                return board;
            }

            // 초기화 실패 시 생성된 객체 파괴
            Destroy(board.gameObject);
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Init(Action<Player, Score> score)
        {
            setting = ResourcesManager.Instance.LoadInBuild<BoardSetting>("BoardSetting");
            if (setting == null)
            {
                Debug.LogWarning("[Board] BoardSetting is missing.");
                // return false; // 필수라면 false 리턴
            }

            updateScore = score;
            commandProcedure = new();
            turnUser = Player.Me;
            deck = new Stack<Card>();
            
            hands = new CardList[(int)Player.Max];
            hands[0] = new CardList();
            hands[1] = new CardList();

            scores = new CardList[(int)Player.Max];
            scores[0] = new CardList();
            scores[1] = new CardList();

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

        public void ReplaceCard(int index, int cardnum)
        {
            hands[(int)Player.Me][index].ReplaceCard(cardnum, sprites[cardnum]);
        }
    }
}
