using System.Collections.Generic;
using System.Linq;

namespace Poker
{
    // 포커 플레이어.
    public class Player
    {
        public long ID { get; set; }
        public List<Card> Hand { get; set; } = new List<Card>();
        
        public bool Winner { get; set; }
        public int Order { get; set; }
        public HandRank Rank { get; set; }
        public Card RankCard { get; set; }

        public Player(long id)
        {
            ID = id;
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
