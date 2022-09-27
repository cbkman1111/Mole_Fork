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
    public Transform player1;
    public Transform player2;

    private Dictionary<int, List<Card>> slots = null;
    private List<Card>[] hands = null;
    private Stack<Card> deck = null;

    private State state = State.WAIT;

    public enum State { 
        WAIT = 0,
        CREATE_DECK,
        SHUFFLE_8,
        SHUFFLE_10,
        SHUFFLE_CHECK,

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

        deck = new Stack<Card>();
        hands = new List<Card>[2];
        hands[0] = new List<Card>();
        hands[1] = new List<Card>();

        slots = new Dictionary<int, List<Card>>();
        slots.Add(0, new List<Card>());
        slots.Add(1, new List<Card>());
        slots.Add(2, new List<Card>());
        slots.Add(3, new List<Card>());
        slots.Add(4, new List<Card>());
        slots.Add(5, new List<Card>());
        slots.Add(6, new List<Card>());
        slots.Add(7, new List<Card>());
        slots.Add(8, new List<Card>());
        slots.Add(9, new List<Card>());
        slots.Add(10, new List<Card>());
        slots.Add(11, new List<Card>());
        slots.Add(12, new List<Card>());


        StartCoroutine(GameProc());
        return true;
    }

    /// <summary>
    /// 게임 시작.
    /// </summary>
    public void StartGame()
    {
        state = State.CREATE_DECK;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameProc()
    {
        while (true)
        {
            switch (state)
            {
                case State.WAIT:
                    break;
                case State.CREATE_DECK:
                    if (CreateDeck() == true)
                    {
                        state = State.SHUFFLE_8;
                    }
                    break;
                case State.SHUFFLE_8:
                    if (Pop8Cards() == true)
                    {
                        state = State.SHUFFLE_10;
                    }
                    break;
                case State.SHUFFLE_10:
                    if (Pop10Cards() == true)
                    {
                        state = State.SHUFFLE_CHECK;
                    }
                    break;

                case State.SHUFFLE_CHECK:
                    break;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool CreateDeck()
    {
        int index = deck.Count;
        Sprite sp = sprites[index];

        Card card = Instantiate<Card>(prefabCard);
        card.Init(index + 1, sp);

        float height = card.Height * 0.5f;
        card.transform.position = new Vector3(0, height * index, 0);
        deck.Push(card);
       
        return deck.Count == 48;
    }
    /// <summary>
    /// 바닥 8장 뿌리기.
    /// </summary>
    /// <returns></returns>
    private bool Pop8Cards()
    {
        Card card = deck.Pop();

        int key = -1;
        foreach (var slot in slots)
        {
            var exist = slot.Value.Where(e => e.Kind == card.Kind).FirstOrDefault();
            if (exist != null)
            {
                key = slot.Key;
                break;
            }
        }

        if (key == -1)
        {
            var empty = slots.Where(c => c.Value.Count == 0).FirstOrDefault();
            card.transform.position = cardPosition[empty.Key].position;
            empty.Value.Add(card);
        }
        else
        {
            var slot = slots.Where(c => c.Key == key).FirstOrDefault();
            float randX = UnityEngine.Random.Range(0.25f, 1.0f);
            float randZ = UnityEngine.Random.Range(0.55f, 1.5f);
            int stackCount = slot.Value.Count;
            Vector3 random = new Vector3(stackCount * randX, stackCount * card.Height, stackCount * randZ);
            card.transform.position = cardPosition[slot.Key].position + random;
            slot.Value.Add(card);
        }

        card.Flip(true);
        card.SetKinematic(false);

        // 나눈 카드의 합을 구합니다.
        int count = 0;
        foreach (var slot in slots)
        {
            count += slot.Value.Count;
        }
         
        return count == 8;
    }

    /// <summary>
    /// 10장씩 나눠주기
    /// </summary>
    /// <returns></returns>
    private bool Pop10Cards()
    {
        if (hands[0].Count < 10)
        {
            int count = hands[0].Count;
            Card card = deck.Pop();
            card.transform.position = player1.position + new Vector3(count * 1.5f, count * card.Height, 0);
            card.Flip(true);
            card.SetKinematic(false);
            hands[0].Add(card);
        }
        else if(hands[1].Count < 10)
        {
            int count = hands[1].Count;
            Card card = deck.Pop();
            card.transform.position = player2.position + new Vector3(count * -1.5f, count * card.Height, 0);
            card.SetKinematic(false);
            hands[1].Add(card);
        }

        if (hands[0].Count == 10 && hands[1].Count == 10)
        {
            return true;
        }

        return false;
    }
}
