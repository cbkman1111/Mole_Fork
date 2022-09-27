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

    private Dictionary<int, List<Card>> slots = null;
    private Dictionary<int, List<Card>> user = null;

    private Stack<Card> deck = null;

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

    public bool Init()
    {
        deck = new Stack<Card>();
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

        return true;
    }

    public void Suffle()
    {
        StartCoroutine(CreateDeck());
    }

    private IEnumerator CreateDeck()
    {
        yield return new WaitForSeconds(0.1f);

        int count = 0;
        bool complete = false;
        while (complete == false)
        {
            Sprite sp = sprites[count];

            Card card = Instantiate<Card>(prefabCard);
            card.Init(count + 1, sp);
            card.transform.position = new Vector3(0, card.Height * count, 0);
            deck.Push(card);

            count++;
            if (count == 48)
            {
                complete = true;
            }

            yield return new WaitForSeconds(0.05f);
        }

        StartCoroutine(PopCards());
        StartCoroutine(PopCards());
    }

    private IEnumerator PopCards()
    {
        yield return new WaitForSeconds(0.0f);
        int count = 0;
        while (deck.Count > 0)
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
                Vector3 random = new Vector3(slot.Value.Count * randX, 0, slot.Value.Count * randZ);
                card.transform.position = cardPosition[slot.Key].position + random;
                slot.Value.Add(card);
            }

            card.Flip(true);
            card.SetKinematic(false);

            /*
            var selectList = cards.Where(c => c.Key == 0).ToList();

            int randIndex = UnityEngine.Random.Range(0, selectList.Count);
            var empty = selectList[randIndex];
            var emptyList = new List<Card>();
            emptyList.Add(card);
            cards.Add(randIndex, emptyList);

            card.transform.position = cardPosition[randIndex].position;
            */


            count++;
            if (count == 4)
                break;

            yield return new WaitForSeconds(0.1f);
        }
    }
}
