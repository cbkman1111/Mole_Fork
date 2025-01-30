using Common.UIObject;
using Poker;
using UnityEngine;
using static Poker.Player;

namespace Pocker
{
    public class UIPokerPlayer : UIBase
    {
        [SerializeField]
        private UIPokerCard[] Cards;

        public bool InitCards(Player player)
        {
            if(player.Hand == null || player.Hand.Count != 7)
            {
                return false;
            }

            for(int i = 0; i < player.Hand.Count; i++)
            {
                bool ret = Cards[i].SetCard(player.Hand[i]);
                if(ret == false)
                {
                    return false;
                }
            }

            return true;
        }

        public void OpenCards(int count)
        {
            for (int i = 0; i < Cards.Length; i++)
            {
                if(i <= count)
                    Cards[i].OpenCard();
            }
        }

        public void SetHandRank(Player player)
        {
            var rank = player.Rank;
            var card = player.RankCard;
            string str1 = $"{rank.ToString()} {card.Kind} {card.Value}";
            SetTextMeshPro("Text-HandRank", str1);

            var winner = player.Winner;
            var order = player.Order;
            var str2 = winner ? "Winner" : $"Order: {order}";
            SetTextMeshPro("Text-Rank", str2);
        }
    }
}

