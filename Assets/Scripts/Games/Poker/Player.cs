using System.Collections.Generic;
using System.Linq;

namespace Poker
{
    /// <summary>
    /// 포커 게임 플레이어
    /// </summary>
    public class Player
    {
        public long ID { get; set; }
        public List<Card> Hand { get; set; } = new List<Card>();
        public int Order { get; set; }
        public bool Die { get; set; }
        public HandRank Rank { get; set; }
        public Card RankCard { get; set; }

        public Player(long id)
        {
            ID = id;
            Die = false;
            Order = 0;
            Rank = HandRank.None;
            RankCard = null;
            Hand.Clear();
        }

        public void AddCard(Card card)
        {
            Hand.Add(card);
        }

        public int[] GetCards()
        {
            return Hand.Select(x => x.Value).ToArray();
        }
    }
}
