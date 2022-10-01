using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    public Card prefabCard = null;
    public Sprite[] sprites = null;

    public Transform deckPosition;
    public List<Transform> cardPosition;
    
    public Transform[] player;
    public Transform[] playerHand;

    private Dictionary<int, List<Card>> bottoms = null;
    private List<Card>[] hands = null;
    private Stack<Card> deck = null;

    private float cardWidth = 0;
    private float cardHeight = 0;

    private State state = State.WAIT;
    private StateEvent evt = StateEvent.INIT;

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

        START_GAME,
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
        hands = new List<Card>[2];
        hands[0] = new List<Card>();
        hands[1] = new List<Card>();
        
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

        float y = Screen.height * 0.1f;

        return true;
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
                    if (count == 48)
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
                    int count = hands[0].Where(e => e.CompleteMove == true).ToList().Count;
                    if (count == 10)
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

            case State.HANDS_SORT:
                if (evt == StateEvent.INIT)
                {
                    evt = StateEvent.PROGRESS;
                }
                else if (evt == StateEvent.PROGRESS)
                {
                    evt = StateEvent.DONE;
                }
                else if (evt == StateEvent.DONE)
                {
                    state = State.START_GAME;
                    evt = StateEvent.INIT;
                }
                break;

            case State.START_GAME:
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool CreateDeck()
    {
        for (int i = 0; i < 48; i++)
        {
            Sprite sp = sprites[i];
            Card card = Instantiate<Card>(prefabCard);
            card.Init(i + 1, sp);
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
                
                Vector3 position = player[user].position + new Vector3(randX, randY, randZ);
                card.SetPhysicDiable(false);
                card.MoveTo(
                    position,
                    time: 0.1f,
                    delay: user * 0.2f + i * 0.1f);

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
                Vector3 position = playerHand[user].transform.position + new Vector3(x, 0, 0);
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

        }
        
        for (int index = 0; index < hands[1].Count; index++)
        {
            Card card = hands[1][index];
            card.SetOpen(false);
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="card"></param>
    public void HitCard(int user, Card card)
    {
        KeyValuePair<int, List<Card>> slot = GetSlot(card);
        bool success = hands[user].Remove(card);
        if (success == true)
        {
            if (slot.Key != -1)
            {
                float randX = UnityEngine.Random.Range(0.1f, 0.2f);
                float randZ = UnityEngine.Random.Range(0.1f, 0.2f);
                int stackCount = slot.Value.Count;

                Vector3 position = cardPosition[slot.Key - 1].position + new Vector3(stackCount * randX, stackCount * card.Height, stackCount * randZ);
                card.MoveTo(
                    position, 
                    time: 0.2f,
                    complete: () => {
                        card.SetPhysicDiable(false);
                    });

                slot.Value.Add(card);
            }
        }    
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
            var exist = kindSlot.Value.Where(e => e.Kind == card.Kind).FirstOrDefault();
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
            count += slot.Value.Where(card => card.CompleteMove).ToList().Count;
        }

        return count;
    }
}
