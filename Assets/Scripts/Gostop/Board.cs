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

/// <summary>
/// 
/// </summary>
public class Board : MonoBehaviour
{
    private StateMachineGostop stateMachine = null;

    [SerializeField]
    public Card prefabCard = null;
    public Sprite[] sprites = null;

    public Transform deckPosition;
    public List<Transform> cardPosition;

    public BoardPosition[] boardPositions = null;

    private Dictionary<int, List<Card>> bottoms = null;
    private List<Card>[] hands = null;
    private List<Card>[] scores = null;
    private Stack<Card> deck = null;
    private List<Card> select = null; // 선택해야 하는 카드.

    private float cardWidth = 0;
    private float cardHeight = 0;

    private Board.Player turnUser = 0;

    public UIMenuGostop menu = null;

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
    /// 게임 시작.
    /// </summary>
    public void StartGame()
    {
        stateMachine.Change(State.CREATE_DECK);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private void Update()
    {
        var turnInfo = stateMachine.GetCurrturnInfo();
        var stateInfo = turnInfo.GetCurrentStateInfo();
        switch (stateInfo.state)
        {
            case State.WAIT:
                stateMachine.Process(
                    () => {},
                    () => {
                        return true;
                    },
                    () => {});
                break;

            // 카드덱 생성.
            case State.CREATE_DECK:
                stateMachine.Process(
                    () => {
                        SetCardCount();
                        CreateDeck();
                    },
                    () => {
                        int count = deck.Where(card => card.CompleteMove == true).ToList().Count;
                        return count == 52;
                    },
                    () => {
                        stateMachine.Change(State.SHUFFLE_8);
                    });

                break;

            // 바닥 8장 깔기.
            case State.SHUFFLE_8:
                stateMachine.Process(
                    () => { 
                        Pop8Cards(); 
                    },
                    () => {
                        int count = 0;
                        foreach (var slot in bottoms)
                        {
                            count += slot.Value.Where(card => card.CompleteMove == true).ToList().Count;
                        }

                        return count == 8;
                    },
                    () => {
                        stateMachine.Change(State.SHUFFLE_10);
                    });

                break;

            // 열장씩 나누기.
            case State.SHUFFLE_10:
                stateMachine.Process(
                    () => {
                        Pop10Cards();
                    },
                    () => {
                        int count = 0;
                        count += hands[0].Where(e => e.CompleteMove == true).ToList().Count;
                        count += hands[1].Where(e => e.CompleteMove == true).ToList().Count;
                        return count == 20;
                    },
                    () => {
                        stateMachine.Change(State.OPEN_8);
                    });
                break;

            // 8장 뒤집기
            case State.OPEN_8:
                stateMachine.Process(
                    () => {
                        FlipCard8();
                    },
                    () => {
                        int count = 0;
                        foreach (var slot in bottoms)
                        {
                            count += slot.Value.Where(e => e.Open == true).ToList().Count;
                        }

                        return count == 8;
                    },
                    () => {
                        stateMachine.Change(State.CHECK_JORKER);
                    });
                break;

            case State.CHECK_JORKER:
                stateMachine.Process(
                    () => {
                        foreach (var slot in bottoms)
                        {
                            var list = slot.Value.Where(e => e.Month == 13).ToList();
                            for (int i = list.Count - 1; i >= 0; --i)
                            {
                                var card = list[i];

                                turnInfo.pop = Pop1Cards((int)turnUser);
                                EatScore(card);
                                list.Remove(card);
                            }
                        }
                    },
                    () => {

                        int count = 0;
                        foreach (var slot in bottoms)
                        {
                            count += slot.Value.Where(e => e.Open == true).ToList().Count;
                        }

                        // 수량이 부족하면 다시 깔도록 INIT 상태로 되돌립니다.
                        if (count < 8)
                        {
                            stateInfo.evt = StateEvent.INIT;
                            return false;
                        }
                        else
                        {
                            int countMove = 0;
                            foreach (var slot in bottoms)
                            {
                                countMove += slot.Value.Where(e => e.CompleteMove == false).ToList().Count;
                            }

                            return countMove == 0;
                        }
                    },
                    () => {
                        stateMachine.Change(State.HANDS_UP);
                    });
                break;

            case State.HANDS_UP:
                stateMachine.Process(
                    () => {
                        HandsUp();
                    },
                    () => {
                        int count = 0;
                        count += hands[0].Where(e => e.CompleteMove == true).ToList().Count;
                        count += hands[1].Where(e => e.CompleteMove == true).ToList().Count;

                        return count == 20;
                    },
                    () => {
                        stateMachine.Change(State.HANDS_OPEN);
                    });
                break;

            case State.HANDS_OPEN: // 손패를 뒤집습니다.
                stateMachine.Process(
                    () => {
                        HandOpen();
                    },
                    () => {
                        int count = hands[0].Where(e => e.Open == false).ToList().Count;
                        return count == 0;
                    },
                    () => {
                        stateMachine.Change(State.HANDS_SORT);
                    });

                break;

            case State.HANDS_SORT: // 손패를 정렬합니다.
                stateMachine.Process(
                    () => {
                        SortHand();
                    },
                    () => {
                        int count = hands[0].Where(e => e.Open == false).ToList().Count;
                        return count == 0;
                    },
                    () => {
                        stateMachine.Change(State.CARD_HIT);
                    });
                break;

            case State.CARD_HIT:
                stateMachine.Process(
                    () => {
                        menu.ShowScoreMenu(true);
                        if (turnUser == Player.COMPUTER)
                        {
                            HitCard((int)Player.COMPUTER, hands[(int)Player.COMPUTER][0]);
                        }
                    },
                    () => {
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
                    () => {
                        stateMachine.Change(State.CARD_POP);
                    });
                break;

            case State.CARD_POP:
                stateMachine.Process(
                    () => {
                        turnInfo.pop = Pop1Cards((int)turnUser); 
                    },
                    () => {
                        
                        int count = GetMoveAllCount();
                        if (count == 0)
                        {
                            if (turnInfo.pop.Month == 13) // 뒤집어서 조커가 나오면 다시 뽑습니다.
                            {
                                stateInfo.evt = StateEvent.INIT;
                                return false;
                            }
                        }

                        return count == 0;
                    },
                    () => {
                        stateMachine.Change(State.EAT_CHECK);
                    });

                break;

            case State.EAT_CHECK: // 카드 획득 처리.
                stateMachine.Process(
                    () => {
                        EatCheck();
                    },
                    () => {
                        if (select.Count == 2)
                        {
                            if (turnUser == Player.COMPUTER)
                            {
                                select[0].Owner = turnUser;

                                var slot = GetSlot(select[0]);
                                for (int i = slot.Value.Count - 1; i >= 0; --i)
                                {
                                    var slotCard = slot.Value[i];
                                    if (slotCard.Owner == turnUser)
                                    {
                                        slot.Value.Remove(slotCard);
                                        EatScore(slotCard);
                                    }
                                }

                                select.Clear();
                            }
                            else 
                            {
                                if (UIManager.Instance.FindPopup("PopupCardSelect") == false)
                                {
                                    PopupCardSelect popup = UIManager.Instance.OpenPopup<PopupCardSelect>("PopupCardSelect");
                                    popup.Init(select[0], select[1], (Card selectCard) => {

                                        selectCard.Owner = turnUser;

                                        var slot = GetSlot(selectCard);
                                        for (int i = slot.Value.Count - 1; i >= 0; --i)
                                        {
                                            var slotCard = slot.Value[i];
                                            if (slotCard.Owner == turnUser)
                                            {
                                                slot.Value.Remove(slotCard);
                                                EatScore(slotCard);
                                            }
                                        }

                                        select.Clear();
                                    });
                                }
                            }
                            

                            return false;
                        }
                        else
                        {
                            int count = GetMoveAllCount();
                            count += scores[0].Where(card => card.CompleteMove == false).ToList().Count();
                            count += scores[1].Where(card => card.CompleteMove == false).ToList().Count();
                            return count == 0;
                        }
                    },
                    () => {
                        // 주인 없는 카드로 설정.
                        foreach (var kindSlot in bottoms)
                        {
                            var list = kindSlot.Value;
                            foreach (var card in list)
                            {
                                card.Owner = Player.NONE;
                            }
                        }

                        select.Clear();
                        stateMachine.Change(State.SCORE_UPDATE);
                    });
                break;

            case State.SCORE_UPDATE: // 점수 체크.
                stateMachine.Process(
                    () => {
                        SetCardCount();
                    },
                    () => {
                        return true;
                    },
                    () => {
                        stateMachine.Change(State.TURN_CHECK);
                    });

                break;
            case State.TURN_CHECK: // 턴 바꾸기.
                stateMachine.Process(
                    () => {
                        if (turnUser == Player.USER)
                        {
                            turnUser = Player.COMPUTER;
                        }
                        else
                        {
                            turnUser = Player.USER;
                        }
                    },
                    () => {
                        int count = GetMoveAllCount();
                        return count == 0;
                    },
                    () => {
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

                        stateMachine.AddTurn(turnUser); // 턴을 증가.
                        stateMachine.Change(nextState);
                    });
                break;

            case State.GAME_OVER_TIE: // 무승부 상태 처리.
                stateMachine.Process(
                    () => {
                    },
                    () => {
                        int count = GetMoveAllCount();
                        return count == 0;
                    },
                    () => {
                        DestroyAllCards();

                        stateMachine.Init();
                        stateMachine.Change(State.CREATE_DECK);
                    });
                break;
        }
    }

    /// <summary>
    /// 기존 카드를 제거합니다.
    /// </summary>
    private void DestroyAllCards()
    {
        // 객체 지우기.
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
        
        // 배열 지우기.
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

    private void SetCardCount()
    {
        for (int user = 0; user < (int)Player.MAX; user++)
        {
            int gwang = scores[user].Where(card => card.KindOfCard == Card.KindOf.GWANG || card.KindOfCard == Card.KindOf.GWANG_B).ToList().Count();
            int mung = scores[user].Where(card => card.KindOfCard == Card.KindOf.MUNG || card.KindOfCard == Card.KindOf.MUNG_GODORI || card.KindOfCard == Card.KindOf.MUNG_KOO).ToList().Count();
            int thee = scores[user].Where(card => card.KindOfCard == Card.KindOf.CHO || card.KindOfCard == Card.KindOf.CHUNG || card.KindOfCard == Card.KindOf.HONG).ToList().Count();
            int pee = scores[user].Where(card => card.KindOfCard == Card.KindOf.P || card.KindOfCard == Card.KindOf.PP || card.KindOfCard == Card.KindOf.PPP).ToList().Count();

            menu.ScoreUpdate((Player)user, gwang, mung, thee, pee);
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
            Sprite sp = sprites[n - 1];
            Card card = Instantiate<Card>(prefabCard);
            card.Init(n, sp);
            deck.Push(card);
            
            float height = card.Height * 0.5f;
            card.transform.position = deckPosition.position;
            card.MoveTo( new Vector3(deckPosition.position.x, height * i, deckPosition.position.z), delay: i * 0.01f);

            // 카드 크기를 저장해둡니다.
            cardWidth = card.Width;
            cardHeight = card.Height;
        }

        return true;
    }

    /// <summary>
    /// 바닥 8장 뿌리기.
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
    /// 10장씩 나눠주기
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
    /// 손패를 받아 듭니다.
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
    /// 손패를 뒤집습니다.
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
    /// 
    /// </summary>
    /// <param name="card"></param>
    public void HitCard(int user, Card card)
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
                
                if (user == 0)
                {
                    if (card.Month == 13)
                    {
                        EatScore(card);

                        var deckCard = deck.Pop();
                        deckCard.MoveTo(card.transform.position, time: 0.1f);
                        deckCard.ShowMe();
                        hands[user].Add(deckCard);
                    }
                    else
                    {
                        card.MoveTo(
                            destination1,
                            time: 0.2f,
                            ease: iTween.EaseType.easeInBack,
                            complete: () =>
                            {
                                card.SetPhysicDiable(false);
                                card.MoveTo(
                                    destination2,
                                    time: 0.2f,
                                    ease: iTween.EaseType.easeInQuint,
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
                        EatScore(card);

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
                            ease: iTween.EaseType.easeInBack,
                            complete: () =>
                            {
                                card.SetPhysicDiable(false);
                                card.MoveTo(
                                    destination2,
                                    time: 0.2f,
                                    ease: iTween.EaseType.easeInQuint,
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
    private void EatScore(Card card, int count = 0)
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
        
        var position = targetPosition + new Vector3(stack * 0.6f, stack * card.Height, 0);
        card.MoveTo(position, ease: iTween.EaseType.easeInQuad, time: 0.2f, delay: count * 0.1f);
        card.SetPhysicDiable(false);

        scores[(int)turnUser].Add(card);
    }

    /// <summary>
    /// 바닥 8장 뿌리기.
    /// </summary>
    /// <returns></returns>
    private bool EatCheck()
    {
        foreach (var kindSlot in bottoms)
        {
            var list = kindSlot.Value;
            int count = list.Where(card => card.Owner == turnUser).ToList().Count();
            if (count > 0) // 판정이 가능.
            {
                int total = list.Count;
                switch (total)
                {
                    case 2:
                        if (list[0].Owner == turnUser && 
                            list[1].Owner == turnUser)
                        {
                            for (int i = list.Count - 1; i >= 0; --i)
                            {
                                var card = list[i]; 
                                EatScore(card, total - i);
                                list.Remove(card);
                            }

                            Debug.LogWarning("귀신."); 
                        }
                        else if (list[0].Owner == Player.NONE && 
                                 list[1].Owner == turnUser )
                        {
                            for (int i = list.Count - 1; i >= 0; --i)
                            {
                                var card = list[i];
                                EatScore(card, total - i);
                                list.Remove(card);
                            }
                            
                            Debug.LogWarning("먹음.");
                        }
                        break;
                    case 3:
                        if (list[0].Owner == Player.NONE &&
                            list[1].Owner == turnUser &&
                            list[2].Owner == Player.NONE)
                        {

                            Debug.LogWarning("쌈.");
                        }
                        else if (list[0].Owner == Player.NONE &&
                                 list[1].Owner == Player.NONE &&
                                 list[2].Owner == turnUser)
                        {
                            for (int i = list.Count - 1; i >= 0; --i)
                            {
                                var card = list[i];
                                if (card.Owner == Player.NONE)
                                {
                                    select.Add(card);
                                }
                            }

                            Debug.LogWarning("골라 먹기.");
                        }
                        break;
                    case 4:
                        if (list[0].Owner == Player.NONE &&
                            list[1].Owner == Player.NONE &&
                            list[2].Owner == turnUser &&
                            list[3].Owner == Player.NONE)
                        {

                            Debug.LogWarning("쌈.");
                        }
                        else if (list[0].Owner == Player.NONE &&
                                 list[1].Owner == Player.NONE &&
                                 list[2].Owner == Player.NONE &&
                                 list[3].Owner == turnUser)
                        {
                            for (int i = list.Count - 1; i >= 0; --i)
                            {
                                var card = list[i];
                                EatScore(card, total - i);
                                list.Remove(card);
                            }

                            Debug.LogWarning("싼걸 먹는다.");
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 10장씩 나눠주기
    /// </summary>
    /// <returns></returns>
    private Card Pop1Cards(int user)
    {
        var turnInfo = stateMachine.GetCurrturnInfo();
        var info = turnInfo.GetCurrentStateInfo();
        Card card = deck.Pop();

        KeyValuePair<int, List<Card>> slot;
        if (card.Month == 13 && turnInfo.hit != null) // 뒤집은 카드가 조커면 때린 카드 위로 올립니다.
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
            card.Owner = turnUser;
            card.MoveTo(
              position,
              time: 0.2f,
              ease: iTween.EaseType.easeInQuint,
              complete: () => {
                  card.MoveTo(
                    destination,
                    time: 0.2f,
                    ease: iTween.EaseType.easeInQuint,
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
