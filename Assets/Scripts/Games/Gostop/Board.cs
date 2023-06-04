using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static StateMachineGostop;

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class BoardPosition
{
    public Transform Gwang = null;
    public Transform Mung = null;
    public Transform Thee = null;
    public Transform Pee = null;

    public Transform GwangScore = null;
    public Transform MungScore = null;
    public Transform TheeScore = null;
    public Transform PeeScore = null;

    public Transform RecvieCard = null;
    public Transform Hand = null;
}

public class Scores
{
    public int shake; // ��� ����.
    public int go; // �� ����.
    public bool peebak; // �ǹ�.
    public bool gwangbak; // ����.
    public bool mungbak; // �۹�.
    public bool goback; // ����.

    public bool chungdan;
    public bool hongdan;
    public bool chodan;
    public bool godori;

    public int gawng;
    public int mung;
    public int pee;
    public int thee;

    public int total;
}
/// <summary>
/// 
/// </summary>
public class Board : MonoBehaviour
{
    private StateMachineGostop stateMachine = null;
    private State lastState = State.NONE;

    [SerializeField]
    public Card prefabCard = null;
    public Sprite[] sprites = null;
    public Sprite spriteBomb = null;

    public Transform deckPosition;
    public List<Transform> cardPosition;

    public BoardPosition[] boardPositions = null;

    private Dictionary<int, List<Card>> bottoms = null;
    private List<Card>[] hands = null;
    private List<Card>[] scores = null;
    private Stack<Card> deck = null;
    private List<Card> select = null; // �����ؾ� �ϴ� ī��.
    private List<Card> listEat = null; // �Դ���.
    
    private float cardWidth = 0;
    private float cardHeight = 0;

    private Board.Player turnUser = 0;
    private int stealCount = 0; // ���Ѿƿ� ��.

    public Scores[] gameScore = null;
    public UIMenuGostop menu = null;

    private float turnChangeStartTime = 0;

    public struct DebugInfo {
        public int num;
        public string msg;
    }

    private Queue<DebugInfo> listDebug = new Queue<DebugInfo>();
    private int debugNumber = 0;

    public enum Player { 
        NONE = -1,

        USER = 0,
        COMPUTER,

        MAX,
    };

    public enum BoardPositions { 
        GWANG = 0,
        MUNG,
        THEE,
        PEE,

        RECIVE,
        HAND,
    }

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

        stateMachine.Change(State.WAIT);

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
        return true;
    }

    public StateMachineGostop GetStateMachine()
    {
        return stateMachine;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool MyTurn()
    {
        return turnUser == Player.USER;
    }

    /// <summary>
    /// ���� ����.
    /// </summary>
    public void StartGame()
    {
        stateMachine.Change(State.START_GAME);
    }

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
    /// <returns></returns>
    private void Update()
    {
        var turnInfo = stateMachine.GetCurrturnInfo();
        var stateInfo = stateMachine.GetCurrStateInfo();

        if (lastState != stateInfo.state)
        {
            lastState = stateInfo.state;
            DebugLog(stateInfo.state.ToString());
        }

        switch (stateInfo.state)
        {
            case State.WAIT:
                stateMachine.Process(
                    start: () => {
                    },
                    check: () => {
                        return true;
                    },
                    complete:() => {});
                break;

            // ���� ����.
            case State.START_GAME:
                stateMachine.Process(
                     start: () => {
                         listDebug.Clear();
                     },
                     check: () => {
                         return true;
                     },
                     complete: () => {
                         stateMachine.Change(State.CREATE_DECK);
                     });
                break;

            // ī�嵦 ����.
            case State.CREATE_DECK:
                stateMachine.Process(
                    start: () => {
                        ScoreUpdate();
                        CreateDeck();
                    },
                    check: () => {
                        int count = deck.Where(card => card.CompleteMove == true).ToList().Count;
                        return count == 52;
                    },
                    complete: () => {
                        stateMachine.Change(State.SHUFFLE_8);
                    });

                break;

            // �ٴ� 8�� ���.
            case State.SHUFFLE_8:
                stateMachine.Process(
                    start: () => {
                        Pop8Cards();
                    },
                    check: () => {
                        int count = 0;
                        foreach (var slot in bottoms)
                        {
                            count += slot.Value.Where(card => card.CompleteMove == true).ToList().Count;
                        }

                        return count == 8;
                    },
                    complete:  () => {
                        stateMachine.Change(State.SHUFFLE_10);
                    });

                break;

            // ���徿 ������.
            case State.SHUFFLE_10:
                stateMachine.Process(
                    start: () => {
                        Pop10Cards();
                    },
                    check: () => {
                        int count = 0;
                        count += hands[0].Where(e => e.CompleteMove == true).ToList().Count;
                        count += hands[1].Where(e => e.CompleteMove == true).ToList().Count;
                        return count == 20;
                    },
                    complete: () => {
                        stateMachine.Change(State.OPEN_8);
                    });
                break;

            // 8�� ������
            case State.OPEN_8:
                stateMachine.Process(
                    start: () => {
                        FlipCard8();
                    },
                    check: () => {
                        int count = 0;
                        foreach (var slot in bottoms)
                        {
                            count += slot.Value.Where(e => e.Open == true).ToList().Count;
                        }

                        return count == 8;
                    },
                    complete: () => {
                        stateMachine.Change(State.CHECK_JORKER);
                    });
                break;

                // �ٴ� ��Ŀ Ȯ��.
            case State.CHECK_JORKER:
                stateMachine.Process(
                    start: () => {
                        foreach (var slot in bottoms)
                        {
                            var list = slot.Value.Where(e => e.Month == 13).ToList();
                            for (int i = list.Count - 1; i >= 0; --i)
                            {
                                var card = list[i];
            
                                EatScore(card, list.Count - i);
                                slot.Value.Remove(card);
                            }
                        }
                    },
                    check: () => {
                        int countMove = 0;
                        foreach (var slot in bottoms)
                        {
                            countMove += slot.Value.Where(e => e.CompleteMove == false).ToList().Count;
                        }

                        return countMove == 0;
                    },
                    complete: () => {
                        int Count = 0;
                        int jockerCount = 0;
                        foreach (var slot in bottoms)
                        {
                            int n = slot.Value.Where(e => e.Month == 13).ToList().Count;
                            if (n > 0)
                            {
                                jockerCount += n;
                            }

                            Count += slot.Value.Count;
                        }

                        if (jockerCount > 0 || Count < 8)
                        {
                            stateMachine.Change(State.OPEN_1_MORE);
                        }
                        else 
                        {
                            stateMachine.Change(State.HANDS_UP);
                        }
                    });
                break;

            case State.OPEN_1_MORE:
                stateMachine.Process(
                    start: () => {
                        Pop1Cards((int)Player.NONE);
                    },
                    check: () => {
                        int count = 0;
                        foreach (var slot in bottoms)
                        {
                            count += slot.Value.Where(e => e.CompleteMove == false).ToList().Count;
                        }

                        return count == 0;
                    },
                    complete: () => {
                        stateMachine.Change(State.CHECK_JORKER);
                    });
                break;

            case State.HANDS_UP:
                stateMachine.Process(
                    start: () => {
                        HandsUp();
                    },
                    check: () => {
                        int count = 0;
                        count += hands[0].Where(e => e.CompleteMove == true).ToList().Count;
                        count += hands[1].Where(e => e.CompleteMove == true).ToList().Count;

                        return count == 20;
                    },
                    complete: () => {
                        stateMachine.Change(State.HANDS_OPEN);
                    });
                break;

            case State.HANDS_OPEN: // ���и� �������ϴ�.
                stateMachine.Process(
                    start: () => {
                        HandOpen();
                    },
                    check: () => {
                        int count = hands[0].Where(e => e.Open == false).ToList().Count;
                        return count == 0;
                    },
                    complete: () => {
                        stateMachine.Change(State.HANDS_SORT);
                    });

                break;

            case State.HANDS_SORT: // ���и� �����մϴ�.
                stateMachine.Process(
                    start: () => {
                        SortHand();
                    },
                    check: () => {
                        int count = hands[0].Where(e => e.Open == false).ToList().Count;
                        return count == 0;
                    },
                    complete: () => {
                        stateMachine.Change(State.CARD_HIT);
                    });
                break;

            case State.CARD_HIT:
                stateMachine.Process(
                    start: () => {
                        menu.ShowScoreMenu(true);
                        if (turnUser == Player.COMPUTER)
                        {
                            var list = GetSameMonthCard((int)Board.Player.USER, hands[(int)Player.COMPUTER][0]);
                            if (list.Count == 3) // ��ź
                            {
                                HitBomb((int)Player.COMPUTER, list, list[0]);
                                stateInfo.evt = StateEvent.PROGRESS;
                            }
                            else if (list.Count == 4) // ����
                            {
                                HitChongtong((int)Player.COMPUTER, list, list[0]);
                                stateInfo.evt = StateEvent.PROGRESS; 
                            }
                            else
                            {
                                HitCard((int)Player.COMPUTER, hands[(int)Player.COMPUTER][0]);
                            }
                        }
                    },
                    check: () => {
                        if (turnInfo.hited == false)
                        { 
                            return false;
                        }

                        if (turnInfo.hit.Month == 13)
                        {
                            return false;
                        }

                        int count = GetMoveAllCount();
                        return count == 0;
                    },
                    complete: () => {
                        stateMachine.Change(State.CARD_POP);
                    });
                break;

            case State.CARD_POP:
                stateMachine.Process(
                    start: () => {
                        turnInfo.pop = Pop1Cards((int)turnUser); 
                    },
                    check: () => {
                        
                        int count = GetMoveAllCount();
                        if (count == 0)
                        {
                            if (turnInfo.pop.Month == 13) // ����� ��Ŀ�� ������ �ٽ� �̽��ϴ�.
                            {
                                stateInfo.evt = StateEvent.INIT;
                                return false;
                            }
                        }

                        return count == 0;
                    },
                    complete: () => {
                        stateMachine.Change(State.EAT_CHECK);
                    });

                break;

            case State.EAT_CHECK: // ī�� ȹ�� ó��.
                stateMachine.Process(
                    start: () => {
                        EatCheck();

                    },
                    check: () => {
                        if (select.Count == 2)
                        {
                            if (turnUser == Player.COMPUTER)
                            {
                                select[0].Owner = turnUser;
                                listEat.Add(select[0]);
                                select.Clear();
                            }
                            else 
                            {
                                if (UIManager.Instance.FindPopup("PopupCardSelect") == false)
                                {
                                    PopupCardSelect popup = UIManager.Instance.OpenPopup<PopupCardSelect>("PopupCardSelect");
                                    popup.Init(select[0], select[1], (Card selectCard) => {

                                        selectCard.Owner = turnUser;
                                        listEat.Add(selectCard);
                                        select.Clear();
                                    });
                                }
                            }
                            
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    },
                    complete: () => {


                        select.Clear();
                        stateMachine.Change(State.EAT);
                    });
                break;

            case State.EAT: // ī�� ȹ��.
                stateMachine.Process(
                    start: () => {
                        Eat();
                    },
                    check: () => {
                        return true;
                    },
                    complete: () => {
                        // ���� ���� ī��� ����.
                        foreach (var kindSlot in bottoms)
                        {
                            var list = kindSlot.Value;
                            foreach (var card in list)
                            {
                                card.Owner = Player.NONE;
                            }
                        }

                        if (stealCount == 0)
                        {
                            stateMachine.Change(State.SCORE_UPDATE);
                        }
                        else
                        {
                            stateMachine.Change(State.STEAL);
                        }
                    });
                break;
            case State.STEAL: // ī�� �ӱ�.
                stateMachine.Process(
                    start: () => {
                        StealCard();
                    },
                    check: () => {
                        return true;
                    },
                    complete: () => {
                        if (stealCount == 0)
                        {
                            stateMachine.Change(State.SCORE_UPDATE);
                        }
                        else
                        {
                            stateMachine.Change(State.STEAL);
                        }
                    });
                break;

            case State.SCORE_UPDATE: // ���� üũ.
                stateMachine.Process(
                    start: () => {
                        ScoreUpdate();
                    },
                    check: () => {
                        return true;
                    },
                    complete: () => {
                        stateMachine.Change(State.TURN_CHECK);
                    });

                break;
            case State.TURN_CHECK: // �� �ٲٱ�.
                stateMachine.Process(
                    start: () => {
                        if (turnUser == Player.USER)
                        {
                            turnUser = Player.COMPUTER;
                        }
                        else
                        {
                            turnUser = Player.USER;
                        }

                        turnChangeStartTime = 0;
                    },
                    check: () => {

                        turnChangeStartTime += Time.deltaTime;
                        if (turnChangeStartTime < 0.1f) // �ణ ��ٸ��� �� �ٲߴϴ�.
                        {
                            return false; 
                        }
                        
                        int count = GetMoveAllCount();
                        return count == 0;
                    },
                    complete: () => {
                        State nextState = State.HANDS_SORT;
                        if (hands[(int)Player.COMPUTER].Count > 0 && turnUser == Player.COMPUTER)
                        {
                            nextState = State.HANDS_SORT;
                        }
                        else if (hands[(int)Player.USER].Count > 0 && turnUser == Player.USER)
                        {
                            nextState = State.HANDS_SORT;
                        }
                        else
                        {
                            nextState = State.GAME_OVER_TIE;
                        }

                        stateMachine.AddTurn(turnUser); // ���� ����.
                        stateMachine.Change(nextState);
                    });
                break;

            case State.GAME_OVER_TIE: // ���º� ���� ó��.
                stateMachine.Process(
                    start: () => {

                    },
                    check: () => {
                        int count = GetMoveAllCount();
                        return count == 0;
                    },
                    complete: () => {
                        DestroyAllCards();

                        stateMachine.Init();
                        stateMachine.Change(State.START_GAME);
                    });
                break;
        }
    }

    /// <summary>
    /// ����� �и� ��Ĩ�ϴ�.
    /// </summary>
    private void StealCard()
    {
        int index = (int)Player.NONE;
        if (turnUser == Player.COMPUTER)
        {
            index = (int)Player.USER;
        }
        else if (turnUser == Player.USER)
        {
            index = (int)Player.COMPUTER;
        }

        var listAll = scores[index].Where(e =>
                     e.KindOfCard == Card.KindOf.P ||
                     e.KindOfCard == Card.KindOf.PP ||
                     e.KindOfCard == Card.KindOf.PPP).
                     OrderBy(e => e.KindOfCard).ToList();
        var list1 = listAll.Where(e => e.KindOfCard == Card.KindOf.P).ToList();
        var list2 = listAll.Where(e => e.KindOfCard == Card.KindOf.PP).ToList();
        var list3 = listAll.Where(e => e.KindOfCard == Card.KindOf.PPP).ToList();

        if (listAll.Count == 0)
        {
            stealCount = 0;
            return;
        }

        Card card = null;
        if (stealCount >= 3 && list3.Count > 0)
        {
            card = list3[0];
        }
        else if (stealCount >= 2 && list2.Count > 0)
        {
            card = list2[0];
        }
        else
        {
            card = listAll[0];
        }

        if (listAll[0].KindOfCard == Card.KindOf.P)
            stealCount -= 1;
        else if (listAll[0].KindOfCard == Card.KindOf.PP)
            stealCount -= 2;
        else if (listAll[0].KindOfCard == Card.KindOf.PPP)
            stealCount -= 3;

        if (stealCount < 0)
            stealCount = 0;

        scores[index].Remove(card);
        EatScore(card);
    }
    /// <summary>
    /// ���� ī�带 �����մϴ�.
    /// </summary>
    private void DestroyAllCards()
    {
        // ��ü �����.
        for(int i = 0; i < deck.Count(); i++)
        {
            GameObject.Destroy(deck.Pop().gameObject);
        }

        foreach (var kindSlot in bottoms)
        {
            foreach (var card in kindSlot.Value)
            {
                GameObject.Destroy(card.gameObject);
            }
        }

        for (int i = 0; i < (int)Player.MAX; i++)
        {
            foreach (var card in hands[i])
            {
                GameObject.Destroy(card.gameObject);
            }
        }

        for (int i = 0; i < (int)Player.MAX; i++)
        {
            foreach (var card in scores[i])
            {
                GameObject.Destroy(card.gameObject);
            }
        }
        
        // �迭 �����.
        deck.Clear();
        stateMachine.Clear();

        foreach (var kindSlot in bottoms)
        {
            kindSlot.Value.Clear();
        }

        for (int i = 0; i < 2; i++)
        {
            hands[i].Clear();
        }

        for (int i = 0; i < (int)Player.MAX; i++)
        {
            scores[i].Clear();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    private List<T> ShuffleList<T>(List<T> list)
    {
        int random1, random2;
        T temp;

        for (int i = 0; i < list.Count; ++i)
        {
            random1 = UnityEngine.Random.Range(0, list.Count);
            random2 = UnityEngine.Random.Range(0, list.Count);

            temp = list[random1];
            list[random1] = list[random2];
            list[random2] = temp;
        }

        return list;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool SortHand()
    {
        for (int i = 0; i < (int)Player.MAX; i++)
        {
            hands[i] = hands[i].OrderBy(card => card.Num).ToList();
        }

        for (int user = 0; user < (int)Player.MAX; user++)
        {
            float startX = cardWidth * 10f * -0.5f;
            for (int index = 0; index < hands[user].Count; index++)
            {
                var card = hands[user][index];

                float x = startX + index * cardWidth + cardWidth * 0.5f;
                var handPosition = boardPositions[user].Hand.position;
                Vector3 position = handPosition + new Vector3(x, 0, 0);
                card.SetPhysicDiable(true);
                card.MoveTo(
                    position, 
                    time: 0.1f, 
                    delay: index * 0.0f);
            }
        }


        return true;
    }

    /// <summary>
    /// ���ھ� ó��.
    /// </summary>
    private void ScoreUpdate()
    {
        for (int user = 0; user < (int)Player.MAX; user++)
        {
            int gwang = scores[user].Where(card => card.KindOfCard == Card.KindOf.GWANG || card.KindOfCard == Card.KindOf.GWANG_B).ToList().Count();
            int mung = scores[user].Where(card => card.KindOfCard == Card.KindOf.MUNG || card.KindOfCard == Card.KindOf.MUNG_GODORI || card.KindOfCard == Card.KindOf.MUNG_KOO).ToList().Count();
            int thee = scores[user].Where(card => card.KindOfCard == Card.KindOf.CHO || card.KindOfCard == Card.KindOf.CHUNG || card.KindOfCard == Card.KindOf.HONG).ToList().Count();
          
            // ������
            if (gwang == 5)
            {
                gameScore[user].gawng = 15;
            }
            else if (gwang == 4)
            {
                gameScore[user].gawng = 4;
            }
            else if (gwang == 3)
            {
                int bgwang = scores[user].Where(card => card.KindOfCard == Card.KindOf.GWANG_B).ToList().Count();
                gameScore[user].gawng = bgwang == 1 ? 2 : 3;
            }

            // ������
            if (thee >= 5)
            {
                gameScore[user].thee = thee - 4;

                if (scores[user].Where(card => card.KindOfCard == Card.KindOf.CHO).ToList().Count == 3)
                {
                    gameScore[user].thee += 3;
                    gameScore[user].chodan = true;
                }

                if (scores[user].Where(card => card.KindOfCard == Card.KindOf.CHUNG).ToList().Count == 3)
                {
                    gameScore[user].thee += 3;
                    gameScore[user].chungdan = true;
                }

                if (scores[user].Where(card => card.KindOfCard == Card.KindOf.HONG).ToList().Count == 3)
                {
                    gameScore[user].thee += 3;
                    gameScore[user].hongdan = true;
                }
            }

            // ������
            if (mung >= 5)
            {
                gameScore[user].mung = mung - 4;
                if (scores[user].Where(card => card.KindOfCard == Card.KindOf.MUNG_GODORI).ToList().Count == 3)
                {
                    gameScore[user].mung += 5;
                    gameScore[user].godori = true;
                }
            }

            // ������
            var list = scores[user].Where(card => card.KindOfCard == Card.KindOf.P || card.KindOfCard == Card.KindOf.PP || card.KindOfCard == Card.KindOf.PPP).ToList();
            int pee = 0;
            foreach (var card in list)
            {
                switch (card.KindOfCard)
                {
                    case Card.KindOf.P:
                        pee += 1;
                        break;
                    case Card.KindOf.PP:
                        pee += 2;
                        break;
                    case Card.KindOf.PPP:
                        pee += 3;
                        break;
                }
            }
            
            if (pee >= 10)
            {
                gameScore[user].pee = pee - 9;
            }
        }

        // �� ���
        for (int user = 0; user < (int)Player.MAX; user++)
        {
            int player = (int)user;
            int enemy = (int)Player.COMPUTER;
            if (player == (int)Player.USER)
            {
                enemy = (int)Player.COMPUTER;
            }
            else
            {
                enemy = (int)Player.USER;
            }

            if (gameScore[player].gawng > 0)
            {
                int gwang = scores[enemy].Where(card => card.KindOfCard == Card.KindOf.GWANG || card.KindOfCard == Card.KindOf.GWANG_B).ToList().Count();
                if (gwang == 0)
                {
                    gameScore[player].gwangbak = true;
                }
                else
                {
                    gameScore[player].gwangbak = false;
                }
            }

            if (gameScore[player].pee > 0)
            {
                int pee = 0;
                var list = scores[enemy].Where(card => card.KindOfCard == Card.KindOf.P || card.KindOfCard == Card.KindOf.PP || card.KindOfCard == Card.KindOf.PPP).ToList();
                foreach (var card in list)
                {
                    switch (card.KindOfCard)
                    {
                        case Card.KindOf.P:
                            pee += 1;
                            break;
                        case Card.KindOf.PP:
                            pee += 2;
                            break;
                        case Card.KindOf.PPP:
                            pee += 3;
                            break;
                    }
                }

                if (pee < 6)
                {
                    gameScore[player].peebak = true;
                }
                else 
                {
                    gameScore[player].peebak = false;
                }
            }


            if (gameScore[player].mung >= 7)
            {
                gameScore[player].mungbak = true;
            }
            else
            {
                gameScore[player].mungbak = false;
            }

            gameScore[player].total = gameScore[player].gawng + gameScore[player].mung + gameScore[player].thee + gameScore[player].pee + gameScore[player].go;
            int multiCount = 0;
            if (gameScore[player].peebak == true)
            {
                multiCount += 1;
            }
            if (gameScore[player].gwangbak == true)
            {
                multiCount += 1;
            }
            if (gameScore[player].mungbak == true)
            {
                multiCount += 1;
            }
            if (gameScore[player].go >= 3)
            {
                multiCount += gameScore[player].go - 3;
            }
            
            float muti = Mathf.Pow(2, multiCount);
            gameScore[player].total *= (int)muti;

            menu.ScoreUpdate((Player)user, gameScore[user]);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool CreateDeck()
    {
        List<int> nums = new List<int>();
        for (int i = 0; i < 52; i++)
        {
            nums.Add(i + 1);
        }

        nums = ShuffleList(nums);

        for (int i = 0; i < nums.Count; i++)
        {
            int n = nums[i];
            Card card = Instantiate<Card>(prefabCard);
            if (card != null)
            {
                Sprite sprite = sprites[n - 1];

                card.Init(n, sprite);
                deck.Push(card);

                float height = card.Height * 0.5f;
                card.transform.position = deckPosition.position;
                card.MoveTo(new Vector3(deckPosition.position.x, height * i, deckPosition.position.z), delay: i * 0.01f);

                // ī�� ũ�⸦ �����صӴϴ�.
                cardWidth = card.Width;
                cardHeight = card.Height;
            }
        }

        return true;
    }

    /// <summary>
    /// �ٴ� 8�� �Ѹ���.
    /// </summary>
    /// <returns></returns>
    private bool Pop8Cards()
    {
        for (int i = 0; i < 8; i++)
        {
            Card card = deck.Pop();
            KeyValuePair<int, List<Card>> slot = GetSlot(card);

            float randX = UnityEngine.Random.Range(0.2f, 0.3f);
            float randZ = UnityEngine.Random.Range(0.1f, 0.15f);

            Vector3 position = cardPosition[slot.Key - 1].position + new Vector3(i * randX, 0, i * randZ);
            card.SetPhysicDiable(true);
            card.MoveTo(
                position,
                time: 0.08f,
                delay: i * 0.025f);

            slot.Value.Add(card);
        }

        return true;
    }

    /// <summary>
    /// 10�徿 �����ֱ�
    /// </summary>
    /// <returns></returns>
    private bool Pop10Cards()
    {
        for (int user = 0; user < 2; user++)
        {
            for(int i = 0; i < 10; i++)
            {
                Card card = deck.Pop();
                
                float randX = UnityEngine.Random.Range(-1.00f, 1.0f);
                float randY = i * card.Height;
                float randZ = UnityEngine.Random.Range(1.00f, 1.0f);

                var recivePosition = boardPositions[user].RecvieCard.position;
                Vector3 position = recivePosition + new Vector3(randX, randY, randZ);
                card.SetPhysicDiable(false);
                card.MoveTo(
                    position,
                    time: 0.1f,
                    delay: user * 0.2f + i * 0.1f);

                card.Owner = (Player)user;
                hands[user].Add(card);
            }
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool FlipCard8()
    {
        foreach (var slot in bottoms)
        {
            foreach (var card in slot.Value)
            {
                card.SetPhysicDiable(false);
                card.CardOpen();
            }
        }
        
        return true;
    }

    /// <summary>
    /// ���и� �޾� ��ϴ�.
    /// </summary>
    /// <returns></returns>
    private bool HandsUp()
    {
        float startX = cardWidth * 10f * -0.5f;
        for (int user = 0; user < 2; user++)
        {
            var list = hands[user];
            list.Reverse();

            int index = 0;
            foreach (var card in list)
            {
                float x = startX + index * cardWidth + cardWidth * 0.5f;
                var handPosition = boardPositions[user].Hand.position;
                Vector3 position = handPosition + new Vector3(x, 0, 0);
                card.SetPhysicDiable(true);
                card.MoveTo(
                    position, 
                    time: 0.1f, 
                    delay: index * 0.05f);

                card.SetOpen(false);
                index++;
            }
        }

        return true;
    }

    /// <summary>
    /// ���и� �������ϴ�.
    /// </summary>
    /// <returns></returns>
    private bool HandOpen()
    {
        for (int index = 0; index < hands[0].Count; index++)
        {
            Card card = hands[0][index];
            card.ShowMe(delay:index * 0.05f);
            card.SetShadow(false);

        }
        
        for (int index = 0; index < hands[1].Count; index++)
        {
            Card card = hands[1][index];
            card.SetOpen(true);
            card.SetShadow(false);
        }

        return true;
    }

    /// <summary>
    /// �տ� ������ ī�� ������ �����մϴ�.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="card"></param>
    /// <returns></returns>
    public List<Card> GetSameMonthCard(int user, Card card)
    {
        return hands[user].Where(c => c.Month == card.Month && c.Month != 13).ToList();
    }

    /// <summary>
    /// ���� ó��.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="list"></param>
    public void HitChongtong(int user, List<Card> list, Card selected)
    {
        HitCard(user, selected, 0.2f);
        gameScore[user].shake += 1;
    }

    /// <summary>
    /// ��ź.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="list"></param>
    public void HitBomb(int user, List<Card> list, Card select)
    {
        bool eatPossible = false;
        foreach (KeyValuePair<int, List<Card>> kindSlot in bottoms)
        {
            if (kindSlot.Value.Count > 0)
            {
                if (kindSlot.Value[0].Month == list[0].Month)
                {
                    eatPossible = true;
                    break;
                }
            }
        }

        if (eatPossible == true)
        {
            for (int i = 0; i < 3; i++)
            {
                HitCard(user, list[i], i * 0.2f);

                // ��ź ī�带 �տ� ����ݴϴ�.
                if (i > 0)
                {
                    Card card = Instantiate<Card>(prefabCard);
                    if (card != null)
                    {
                        card.Init(-1, spriteBomb);
                        card.Month = 100;
                        card.transform.position = list[i].transform.position;
                        card.transform.rotation = list[i].transform.rotation;
                        card.Open = true;
                        hands[user].Add(card);
                    }
                }
            }

            stealCount += 1;
            gameScore[user].shake += 1;
        }
        else
        {
            HitCard(user, select,  0.2f);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="card"></param>
    public void HitCard(int user, Card card, float delay = 0)
    {
        var turnInfo = stateMachine.GetCurrturnInfo();
        var info = turnInfo.GetCurrentStateInfo();

        KeyValuePair<int, List<Card>> slot = GetSlot(card);
        bool success = hands[user].Remove(card);
        if (success == true)
        {
            turnInfo.hit = card;
            turnInfo.hited = true;

            if (slot.Key != -1)
            {
                float randX = UnityEngine.Random.Range(-0.5f, 0.5f);
                float randZ = UnityEngine.Random.Range(-0.5f, 0.5f);
                int stackCount = slot.Value.Count;

                Vector3 destination1 = card.transform.position + new Vector3(0, 3, 3);
                Vector3 destination2 = cardPosition[slot.Key - 1].position + 
                    new Vector3(randX, stackCount * card.Height, randZ);

                card.SetPhysicDiable(true);
                card.SetShadow(true);
                card.Owner = (Player)user;

                if ((Player)user == Player.USER)
                {
                    if (card.Month == 13) // ��Ŀ ī��.
                    {
                        EatScore(card, complete: () =>
                        {
                            StealCard();
                        });

                        var deckCard = deck.Pop();
                        deckCard.MoveTo(card.transform.position, time: 0.1f);
                        deckCard.ShowMe();
                        hands[user].Add(deckCard);
                    }
                    else if (card.Month == 100) // ��ź ��¥ ī��.
                    {
                        GameObject.Destroy(card.gameObject);
                        stateMachine.Change(State.CARD_POP);
                    }
                    else // �Ϲ� ī��.
                    {
                        card.MoveTo( // ī�带 ���� �̾Ƽ�.
                            destination1,
                            time: 0.2f,
                            ease: DG.Tweening.Ease.Linear,
                            complete: () =>
                            {
                                card.SetPhysicDiable(false); // ����ģ��.
                                card.MoveTo(
                                    destination2,
                                    time: 0.1f,
                                    delay: delay,
                                    complete: () =>
                                    {

                                    });
                            });

                        slot.Value.Add(card);
                    }
                }
                else
                {
                    if (card.Month == 13)
                    {
                        EatScore(card, complete: () => {
                            StealCard();
                        });
           
                        var deckCard = deck.Pop();
                        deckCard.MoveTo(card.transform.position);
                        deckCard.CardOpen();
                        hands[user].Add(deckCard);

                        info.evt = StateEvent.INIT;
                    }
                    else
                    {
                        Vector3 destination2Up = cardPosition[slot.Key - 1].position + new Vector3(0, 10, 0);

                        card.MoveTo(
                            destination2Up,
                            time: 0.2f,
                            complete: () =>
                            {
                                card.SetPhysicDiable(false);
                                card.MoveTo(
                                    destination2,
                                    time: 0.2f,
                                    delay: delay,
                                    complete: () =>
                                    {

                                    });
                            });

                        slot.Value.Add(card);
                    }
                }
            }
        }    
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="card"></param>
    private void EatScore(Card card, int count = 0, Action complete = null)
    {
        int index = (int)turnUser;
        int stack = 0;
        Vector3 targetPosition = Vector3.zero;

        switch (card.KindOfCard)
        {
            case Card.KindOf.GWANG:
            case Card.KindOf.GWANG_B:
                targetPosition = boardPositions[index].Gwang.position;
                stack = scores[index].
                    Where(e => e.KindOfCard == Card.KindOf.GWANG || 
                               e.KindOfCard == Card.KindOf.GWANG_B).
                                ToList().
                                Count();

                break;
            case Card.KindOf.MUNG:
            case Card.KindOf.MUNG_GODORI:
            case Card.KindOf.MUNG_KOO:
                targetPosition = boardPositions[index].Mung.position;
                stack = scores[index].
                    Where(e => e.KindOfCard == Card.KindOf.MUNG || 
                               e.KindOfCard == Card.KindOf.MUNG_GODORI ||
                               e.KindOfCard == Card.KindOf.MUNG_KOO).
                               ToList().
                               Count();
                break;
            case Card.KindOf.CHO:
            case Card.KindOf.CHUNG:
            case Card.KindOf.HONG:
            case Card.KindOf.CHO_B:
                targetPosition = boardPositions[index].Thee.position;
                stack = scores[index].
                    Where(e => e.KindOfCard == Card.KindOf.CHO ||
                               e.KindOfCard == Card.KindOf.CHUNG ||
                               e.KindOfCard == Card.KindOf.HONG ||
                               e.KindOfCard == Card.KindOf.CHO_B).
                               ToList().
                               Count();
                break;
            case Card.KindOf.P:
            case Card.KindOf.PP:
            case Card.KindOf.PPP:
                targetPosition = boardPositions[index].Pee.position;
                stack = scores[index].
                    Where(e => e.KindOfCard == Card.KindOf.P ||
                               e.KindOfCard == Card.KindOf.PP ||
                               e.KindOfCard == Card.KindOf.PPP).
                               ToList().
                               Count();
                break;
        }

        float interval = 0.5f;
        var position = targetPosition + new Vector3(stack * 0.6f, stack * card.Height, 0);
        card.MoveTo(position, time: count * 0.05f, delay: interval, complete: complete);
        card.SetPhysicDiable(false);
        scores[(int)turnUser].Add(card);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    private void EatLog(KeyValuePair<int, List<Card>> info)
    {
        if (info.Value.Where(c => c.Owner != Player.NONE).ToList().Count() == 0)
        {
            return;
        }

        foreach (var card in info.Value)
        {  
            DebugLog($"   > {card.Month}�� {card.KindOfCard} / {card.Owner}");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void Eat()
    {
        Debug.Log($"�Դ� ���� �� : {listEat.Count}");

        int total = listEat.Count;
        if (total == 0)
        {
            return;
        }


        for (int i = listEat.Count - 1; i >= 0; --i)
        {
            var card = listEat[i];
            EatScore(card, total - i); // ī�� ȹ��.

            var slot = GetSlot(card);
            slot.Value.Remove(card); // ���� ���Կ��� ����.           
        }

        listEat.Clear();
    }

    /// <summary>
    /// ȹ�� ī�� üũ.
    /// </summary>
    /// <returns></returns>
    private bool EatCheck()
    {
        bool possibleEat = false; 
        foreach (KeyValuePair<int, List<Card>> kindSlot in bottoms)
        {
            var list = kindSlot.Value.Where(c => c.Month != 13).ToList();
            var listJocker = kindSlot.Value.Where(c => c.Month == 13).ToList();

            switch (list.Count)
            {
                case 1:
                    if (listJocker.Count > 0)
                    {
                        stealCount += listJocker.Count;
                        listEat.AddRange(listJocker);
                    }
                    break;
                case 2:
                    if (list[0].Owner == turnUser &&
                        list[1].Owner == turnUser)
                    {
                        foreach (var card in list)
                        {
                            listEat.Add(card);
                        }

                        possibleEat = true;
                        stealCount++;
                        DebugLog($"{list[0].Month} - �ͽ�.");
                    }
                    else if (list[0].Owner == Player.NONE &&
                             list[1].Owner == turnUser)
                    {
                        foreach (var card in list)
                        {
                            listEat.Add(card);
                        }

                        possibleEat = true;
                        DebugLog($"{list[0].Month} - ����.");
                    }
                    else
                    {
                        EatLog(kindSlot);
                    }
                    break;
                case 3:
                    if (list[0].Owner == Player.NONE &&
                        list[1].Owner == turnUser &&
                        list[2].Owner == Player.NONE)
                    {
                        DebugLog($"{list[0].Month} - ����. 1");
                    }
                    else if (list[0].Owner == Player.NONE &&
                              list[1].Owner == turnUser &&
                              list[2].Owner == turnUser)
                    {
                        DebugLog($"{list[0].Month} - ����. 2");
                    }
                    else if (list[0].Owner == Player.NONE &&
                            list[1].Owner == Player.NONE &&
                            list[2].Owner == turnUser)
                    {
                        foreach (var card in list)
                        {
                            if (card.Owner == Player.NONE)
                            {
                                select.Add(card);
                            }
                            else if (card.Owner == Player.USER)
                            {
                                listEat.Add(card);
                            }
                        }

                        // ���� ������ �׳� �ڿ��� ��.
                        if (select.Count == 2)
                        {
                            listEat.Add(select[1]);
                            if (select[0].KindOfCard == select[1].KindOfCard)
                            {
                                select.Clear();
                            }
                        }

                        possibleEat = true;
                        DebugLog($"{list[0].Month} - �񸣱�. {select.Count}");
                    }
                    else
                    {
                        EatLog(kindSlot);
                    }

                    break;
                case 4:
                    if (list[0].Owner == Player.NONE &&
                        list[1].Owner == Player.NONE &&
                        list[2].Owner == Player.NONE &&
                        list[3].Owner == turnUser)
                    {
                        foreach (var card in list)
                        {
                            listEat.Add(card);
                        }

                        possibleEat = true;
                        stealCount++;
                        DebugLog($"{list[0].Month} - �ƽ�.");
                    }
                    else if (list[0].Owner == Player.NONE &&
                            list[1].Owner == Player.NONE &&
                            list[2].Owner == turnUser &&
                            list[3].Owner == turnUser)
                    {
                        foreach (var card in list)
                        {
                            listEat.Add(card);
                        }

                        possibleEat = true;
                        stealCount++;
                        select.Clear();
                        DebugLog($"{list[0].Month} - ����. 1");
                    }
                    else if (list[0].Owner == Player.NONE &&
                            list[1].Owner == turnUser &&
                            list[2].Owner == turnUser &&
                            list[3].Owner == turnUser)
                    {
                        foreach (var card in list)
                        {
                            listEat.Add(card);
                        }

                        possibleEat = true;
                        stealCount++;
                        select.Clear();
                        DebugLog($"{list[0].Month} - ����. 2");
                    }
                    else
                    {
                        EatLog(kindSlot);
                    }
                    break;
                default:
                    {
                        EatLog(kindSlot);
                    }
                    break;
            }

            if (possibleEat == true)
            {
                stealCount += listJocker.Count;
                listEat.AddRange(listJocker);
            }
        }

        return true;
    }

    /// <summary>
    /// 10�徿 �����ֱ�
    /// </summary>
    /// <returns></returns>
    private Card Pop1Cards(int user)
    {
        var turnInfo = stateMachine.GetCurrturnInfo();
        var info = turnInfo.GetCurrentStateInfo();
        Card card = deck.Pop();

        KeyValuePair<int, List<Card>> slot;
        if (card.Month == 13 && turnInfo.hit != null) // ������ ī�尡 ��Ŀ�� ���� ī�� ���� �ø��ϴ�.
        {
            slot = GetSlot(turnInfo.hit);
        }
        else 
        {
            slot = GetSlot(card);
        }
        
        if (slot.Key != -1)
        {
            float randX = UnityEngine.Random.Range(-0.5f, 0.5f);
            float randZ = UnityEngine.Random.Range(-0.5f, 0.5f);
            int stackCount = slot.Value.Count;

            Vector3 position = card.transform.position + new Vector3(0, 5, 0);
            Vector3 destination = cardPosition[slot.Key - 1].position +
                        new Vector3(randX, stackCount * card.Height, randZ);

            card.CardOpen();
            card.Owner = (Player)user;
            card.MoveTo(
              position,
              time: 0.2f,
              complete: () => {
                  card.MoveTo(
                    destination,
                    time: 0.2f,
                    complete: () => {

                    });
              });

            slot.Value.Add(card);
        }
         
        return card;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private KeyValuePair<int, List<Card>> GetSlot(Card card)
    {
        int key = -1;
        foreach (var kindSlot in bottoms)
        {
            var exist = kindSlot.Value.Where(e => e.Month == card.Month).FirstOrDefault();
            if (exist != null)
            {
                key = kindSlot.Key;
                break;
            }
        }

        KeyValuePair<int, List<Card>> slot;
        if (key == -1)
        {
            var emptyList = bottoms.Where(c => c.Value.Count == 0).ToList();
            slot = emptyList[UnityEngine.Random.Range(0, emptyList.Count)];
        }
        else
        {
            slot = bottoms.Where(c => c.Key == key).FirstOrDefault();
        }

        Debug.Log($"slot : {slot.Key}");
        return slot;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private int GetMoveAllCount()
    {
        int count = 0;
        foreach (var slot in bottoms)
        {
            count += slot.Value.Where(card => card.CompleteMove == false).ToList().Count;
        }

        return count;
    }
}