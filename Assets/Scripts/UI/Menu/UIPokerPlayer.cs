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

        private Player Player = null;

        public bool InitCards(Player player)
        {
            Player = player;
            if (Player == null)
                return false;

            foreach (var card in Cards)
            {
                card.SetCard(null);
            }

            if (Player.Hand != null)
            {
                for (int i = 0; i < Player.Hand.Count; i++)
                {
                    bool ret = Cards[i].SetCard(Player.Hand[i]);
                    if (ret == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void OpenCards(int count)
        {
            for (int i = 0; i < Cards.Length; i++)
            {
                if (i <= count)
                {
                    Cards[i].OpenCard();
                }
            }
        }

        public void UpdateHandRank()
        {
            if (Player == null)
                return;

            var rank = Player.Rank;
            var card = Player.RankCard;
            string str1 = $"{rank.ToString()} {card.Kind} {card.Value}";
            SetTextMeshPro("Text-HandRank", str1);

            var winner = Player.Winner;
            var order = Player.Order;
            var str2 = winner ? "Winner" : $"Order: {order}";
            SetTextMeshPro("Text-Rank", str2);
        }
    }
}

