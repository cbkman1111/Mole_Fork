using DG.Tweening;
using Gostop;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gostop
{
    public class CardList
    {
        private List<Card> list = null;
        public int Count => list.Count;
        public List<Card> List => list;

        public CardList()
        {
            list = new List<Card>();
        }

        public int MoveCount()
        {
            return list.Where(c => c.ListTween.Count != 0).ToList().Count;
        }

        public List<Card> SameList(Card card)
        {
            return list.Where(c => c.Month == card.Month && c.Month != 13).ToList();
        }

        public List<Card> GetList(int month)
        {
            return list.Where(c => c.Month == month).ToList();
        }

        public List<Card> GetListNot13()
        {
            return list.Where(c => c.Month != 13).ToList();
        }

        public void Reverse()
        {
            list.Reverse();
        }

        public void Destroy()
        {
            foreach (var card in list)
            {
                DOTween.KillAll(card.gameObject);
                GameObject.Destroy(card.gameObject);
            }

            Clear();
        }

        public void Add(Card card)
        {
            list.Add(card);
            if (list.Count > 10)
            {
                Debug.LogError("손패가 10장을 초과하였습니다.");
            }
        }

        public void AddRange(List<Card> cards)
        {
            list.AddRange(cards);
        }

        public bool Remove(Card card)
        {
            return list.Remove(card);
        }

        public void Clear()
        {
            list.Clear();
        }

        public void OrderByNum()
        {
            list = list.OrderBy(card => card.Num).ToList();
        }

        public Card Get(int index)
        {
            return list[index];
        }
    }
}
