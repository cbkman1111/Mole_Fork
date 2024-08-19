using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gostop
{
    /// <summary>
    /// 카드 리스트.
    /// </summary>
    public class CardList : List<Card>
    {
        public void Destroy()
        {
            foreach (var card in this)
            {
                DOTween.KillAll(card.gameObject);
                GameObject.Destroy(card.gameObject);
            }

            Clear();
        }

        public int MoveCount()
        {
            return this.Where(c => c.ListTween.Count != 0).ToList().Count;
        }

        public List<Card> SameList(Card card)
        {
            return this.Where(c => c.Month == card.Month && c.Month != 13).ToList();
        }

        public List<Card> GetList(int month)
        {
            return this.Where(c => c.Month == month).ToList();
        }
        
        public List<Card> GetListNot13()
        {
            return this.Where(c => c.Month != 13).ToList();
        }

        public void AddRange(List<Card> cards)
        {
            this.AddRange(cards);
        }

        public void OrderByNum()
        {
            this.OrderBy(card => card.Num).ToList();
        }
    }
}
