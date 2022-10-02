using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    private float cardWidth = 0;
    private float cardHeight = 0;

    private State state = State.WAIT;
    private StateEvent evt = StateEvent.INIT;

    private Player turnUser = 0;

    public Action OnGameStart = null;

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

    public enum StateEvent { 
        INIT = 0,
        PROGRESS,
        DONE,
    }

    public enum State { 
        WAIT = 0,

        CREATE_DECK,
        
        SHUFFLE_8,
        SHUFFLE_10,

        SHUFFLE_OPEN_8,

        HANDS_UP,
        HANDS_OPEN,
        HANDS_SORT,

        CARD_HIT, // 카드 치기.
        CARD_POP, // 카드 뒤집기.
        
        //HIT_USER_SECOND,
        //DECK_CARD_POP_USER_SECOND, // 패 뒤집기.

        EAT_CHECK, // 먹는 판정.

        TURN_CHECK, // 턴 바꾸기.

        GAME_OVER_TIE, // 무승부.
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Board Create()
    {
        Board prefab = ResourcesManager.Instance.LoadInBuild<Board>("Board");
        Board board = Instantiate<Board>(prefab);
        if (board != null && board.Init())
        {
            return board;
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool Init()
    {
        state = State.WAIT;
        evt = StateEvent.INIT;

        deck = new Stack<Card>();
        hands = new List<Card>[(int)Player.MAX];
        hands[0] = new List<Card>();
        hands[1] = new List<Card>();

        scores = new List<Card>[(int)Player.MAX];
        scores[0] = new List<Card>();
        scores[1] = new List<Card>();

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

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public State GetState()
    {
        return state;
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
        state = State.CREATE_DECK;
        evt = StateEvent.INIT;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private void Update()
    {
        switch (state)
        {
            case State.WAIT:
                break;

            // 카드덱 생성.
            case State.CREATE_DECK:
                if (evt == StateEvent.INIT)
                {
                    CreateDeck();
                    evt = StateEvent.PROGRESS;
                }
                else if (evt == StateEvent.PROGRESS)
                {
                    int count = deck.Where(card => card.CompleteMove == true).ToList().Count;
                    if (count == 52)
                    {
                        evt = StateEvent.DONE;
                    }
                }
                else if (evt == StateEvent.DONE)
                {
                    state = State.SHUFFLE_8;
                    evt = StateEvent.INIT;
                }
                break;

            // 바닥 8장 깔기.
            case State.SHUFFLE_8:
                if (evt == StateEvent.INIT)
                {
                    Pop8Cards();
                    evt = StateEvent.PROGRESS;
                }
                else if (evt == StateEvent.PROGRESS)
                {
                    int count = 0;
                    foreach (var slot in bottoms)
                    {
                        count += slot.Value.Where(card => card.CompleteMove == true).
                            ToList().
                            Count;
                    }

                    if (count == 8)
                    {
                        evt = StateEvent.DONE;
                    }
                }
                else if (evt == StateEvent.DONE)
                {
                    state = State.SHUFFLE_10;
                    evt = StateEvent.INIT;
                }
                break;

            // 열장씩 나누기.
            case State.SHUFFLE_10:
                if (evt == StateEvent.INIT)
                {
                    Pop10Cards();
                    evt = StateEvent.PROGRESS;
                }
                else if (evt == StateEvent.PROGRESS)
                {
                    int completeCount = 0;
                    completeCount += hands[0].Where(e => e.CompleteMove == true).ToList().Count;
                    completeCount += hands[1].Where(e => e.CompleteMove == true).ToList().Count;
                    if (completeCount == 20)
                    {
                        evt = StateEvent.DONE;
                    }
                }
                else if (evt == StateEvent.DONE)
                {
                    state = State.SHUFFLE_OPEN_8;
                    evt = StateEvent.INIT;
                    
                }
                break;

            // 8장 뒤집기
            case State.SHUFFLE_OPEN_8:
                if (evt == StateEvent.INIT)
                {
                    FlipCard8(); 
                    evt = StateEvent.PROGRESS;
                }
                else if (evt == StateEvent.PROGRESS)
                {
                    int count = 0;
                    foreach (var slot in bottoms)
                    {
                        count += slot.Value.Where(e => e.Open == true).ToList().Count;
                    }

                    if (count == 8)
                    {
                        evt = StateEvent.DONE;
                    }
                }
                else if (evt == StateEvent.DONE)
                {
                    state = State.HANDS_UP;
                    evt = StateEvent.INIT;

                }

                break;

            case State.HANDS_UP:
                if (evt == StateEvent.INIT)
                {
                    HandsUp();
                    evt = StateEvent.PROGRESS;
                }
                else if (evt == StateEvent.PROGRESS)
                {
                    int completeCount = 0;
                    completeCount += hands[0].Where(e => e.CompleteMove == true).ToList().Count;
                    completeCount += hands[1].Where(e => e.CompleteMove == true).ToList().Count;
                    if (completeCount == 20)
                    {
                        evt = StateEvent.DONE;
                    }
                }
                else if (evt == StateEvent.DONE)
                {
                    state = State.HANDS_OPEN;
                    evt = StateEvent.INIT;

                }
                break;

            case State.HANDS_OPEN:
                if (evt == StateEvent.INIT)
                {
                    HandOpen();
                    evt = StateEvent.PROGRESS;
                }
                else if (evt == StateEvent.PROGRESS)
                {
                    int count = hands[0].Where(e => e.Open == false).ToList().Count;
                    if (count == 0)
                    {
                        evt = StateEvent.DONE;
                    }
                }
                else if (evt == StateEvent.DONE)
                {
                    state = State.HANDS_SORT;
                    evt = StateEvent.INIT;
                }
                break;

            case State.HANDS_SORT: // 손패를 정렬합니다.
                if (evt == StateEvent.INIT)
                {
                    SortHand();
                    evt = StateEvent.PROGRESS;
                }
                else if (evt == StateEvent.PROGRESS)
                {
                    int count = hands[0].Where(e => e.Open == false).ToList().Count;
                    if (count == 0)
                    {
                        evt = StateEvent.DONE;
                    }
                }
                else if (evt == StateEvent.DONE)
                {
                    state = State.CARD_HIT;
                    evt = StateEvent.INIT;
                }
                break;

            case State.CARD_HIT:
                if (evt == StateEvent.INIT)
                {
                    if (OnGameStart != null)
                    {
                        OnGameStart();
                    }

                    // HitCard 에서 PROGRESS 로 변환.
                    if (turnUser == Player.COMPUTER)
                    {
                        HitCard((int)Player.COMPUTER, hands[(int)Player.COMPUTER][0]);
                    }
                }
                else if (evt == StateEvent.PROGRESS)
                {
                    int count = GetMoveAllCount();
                    if (count == 0)
                    {
                        evt = StateEvent.DONE;
                    }
                }
                else if (evt == StateEvent.DONE)
                {
                    state = State.CARD_POP;
                    evt = StateEvent.INIT;
                }
                break;

            case State.CARD_POP:
                if (evt == StateEvent.INIT)
                {
                    Pop1Cards((int)turnUser);
                    evt = StateEvent.PROGRESS;
                }
                else if (evt == StateEvent.PROGRESS)
                {
                    int count = GetMoveAllCount();
                    if (count == 0)
                    {
                        evt = StateEvent.DONE;
                    }
                }
                else if (evt == StateEvent.DONE)
                {
                    state = State.EAT_CHECK;
                    evt = StateEvent.INIT;
                }
                break;

            case State.EAT_CHECK: // 카드 획득 처리.
                if (evt == StateEvent.INIT)
                {
                    EatCheck();
                    evt = StateEvent.PROGRESS;
                }
                else if (evt == StateEvent.PROGRESS)
                {
                    int count = GetMoveAllCount();
                    if (count == 0)
                    {
                        evt = StateEvent.DONE;
                    }
                }
                else if (evt == StateEvent.DONE)
                {
                    // 주인 없는 카드로 설정.
                    foreach (var kindSlot in bottoms)
                    {
                        var list = kindSlot.Value;
                        foreach (var card in list)
                        {
                            card.Owner = Player.NONE;
                        }
                    }

                    state = State.TURN_CHECK;
                    evt = StateEvent.INIT;
                }
                break;

            case State.TURN_CHECK: // 턴 바꾸기.
                if (evt == StateEvent.INIT)
                {
                    if (turnUser == Player.USER)
                    {
                        turnUser = Player.COMPUTER;
                    }
                    else
                    {
                        turnUser = Player.USER;
                    }

                    evt = StateEvent.PROGRESS;
                }
                else if (evt == StateEvent.PROGRESS)
                {
                    int count = GetMoveAllCount();
                    if (count == 0)
                    {
                        evt = StateEvent.DONE;
                    }
                }
                else if (evt == StateEvent.DONE)
                {
                    if (hands[(int)Player.COMPUTER].Count > 0 && turnUser == Player.COMPUTER)
                    {
                        state = State.HANDS_SORT;
                    }
                    else if (hands[(int)Player.USER].Count > 0 && turnUser == Player.USER)
                    {
                        state = State.HANDS_SORT;
                    }
                    else
                    {
                        state = State.GAME_OVER_TIE;
                    }
                    
                    evt = StateEvent.INIT;
                }
                break;

            case State.GAME_OVER_TIE: // 무승부 상태 처리.
                if (evt == StateEvent.INIT)
                {
                    evt = StateEvent.PROGRESS;
                }
                else if (evt == StateEvent.PROGRESS)
                {
                    int count = GetMoveAllCount();
                    if (count == 0)
                    {
                        evt = StateEvent.DONE;
                    }
                }
                else if (evt == StateEvent.DONE)
                {
                    DestroyAllCards();

                    state = State.CREATE_DECK;
                    evt = StateEvent.INIT;
                }
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
            card.MoveTo(new Vector3(0, height * i, 0), delay: i * 0.01f);

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
        if (evt == StateEvent.PROGRESS)
        {
            Debug.Log("allready hit card.");
            return;
        }

        KeyValuePair<int, List<Card>> slot = GetSlot(card);
        bool success = hands[user].Remove(card);
        if (success == true)
        {
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
                }


                slot.Value.Add(card);
                evt = StateEvent.PROGRESS; // 카드 침.
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
                            for (int i = list.Count - 1; i > 0; --i)
                            {
                                var card = list[i];
                                EatScore(card, total - i);
                                list.Remove(card);
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
    private bool Pop1Cards(int user)
    {
        if (deck.Count <= 0)
        {
            Debug.LogError("deck count is 0.");
            return false;
        }

        Card card = deck.Pop();
        KeyValuePair<int, List<Card>> slot = GetSlot(card);
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
         
        return true;
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
            var exist = kindSlot.Value.Where(e => e.GetMonth() == card.GetMonth()).FirstOrDefault();
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
