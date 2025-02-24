using System.Collections.Generic;
using System.Linq;

namespace Poker
{
    /// <summary>
    /// 포커 게임 글로벌 데이터.
    /// </summary>
    public class PokerData
    {
        private Deck Deck { get; set; } = null;
        public Dictionary<PlayUser, Player> Players { get; set; } = null;

        public PokerData(long[] players)
        {
            Deck = new();
            Deck.Shuffle();

            Players = new()
            {
                { PlayUser.Player1, new Player(players[0]) },
                { PlayUser.Player2, new Player(players[1]) },
                { PlayUser.Player3, new Player(players[2]) },
                { PlayUser.Player4, new Player(players[3]) },
                { PlayUser.Player5, new Player(players[4]) },
                { PlayUser.Observer1, new Player(players[5]) },
                { PlayUser.Observer2, new Player(players[6]) },
                { PlayUser.Observer3, new Player(players[7]) },
                { PlayUser.Observer4, new Player(players[8]) },
                { PlayUser.Observer5, new Player(players[9]) }
            };

            DealCard(2);
            UpdateHandRank();
            UpdateRank();
        }

        /// <summary>
        /// 옵져버를 제외한 플레이어간의 족보 확인.
        /// </summary>
        /// <returns></returns>
        public List<PlayUser> UpdateRank()
        {
            var list = Players.Where(e => e.Key >= PlayUser.Player1 && e.Key <= PlayUser.Player5).ToList();
            var listOrder = list.OrderByDescending(x => x.Value.Rank).
                ThenByDescending(x => x.Value.RankCard.Value).
                ThenByDescending(x => x.Value.RankCard.Kind).Select(x => x.Key).ToList();

            for (int i = 0; i < listOrder.Count; i++)
            {
                var index = listOrder[i];
                Players[index].Order = i;
                Players[index].Winner = i == 0;
            }

            return listOrder;
        }

        public void DealCard(int count)
        {
            for (PlayUser index = PlayUser.Player1; index <= PlayUser.Player5; index++)
            {
                for (int i = 0; i < count; i++)
                    Players[index].AddCard(Deck.DeQueue());
            }
        }

        public void UpdateHandRank()
        {
            for (PlayUser index = PlayUser.Player1; index <= PlayUser.Player5; index++)
            {
                EvaluateHand(Players[index]);
            }
        }

        public void EvaluateHand(Player player)// List<Card> Hand)
        {
            var Hand = player.Hand;
            
            var royalFlushCard = IsRoyalFlush(Hand);
            if (royalFlushCard != null)
            {
                player.Rank = HandRank.RoyalFlush;
                player.RankCard = royalFlushCard;
                return;
            }

            var straightFlushCard = IsStraightFlush(Hand);
            if (straightFlushCard != null)
            {
                player.Rank = HandRank.StraightFlush;
                player.RankCard = straightFlushCard;
                return;
            }

            var fourOfAKindCard = IsFourOfAKind(Hand);
            if (fourOfAKindCard != null)
            {
                player.Rank = HandRank.FourOfAKind;
                player.RankCard = fourOfAKindCard;
                return;
            }

            var fullHouseCard = IsFullHouse(Hand);
            if (fullHouseCard != null)
            {
                player.Rank = HandRank.FullHouse;
                player.RankCard = fullHouseCard;
                return;
            }

            var flushCard = IsFlush(Hand);
            if (flushCard != null)
            {
                player.Rank = HandRank.Flush;
                player.RankCard = flushCard;
                return;
            }

            var straightCard = IsStraight(Hand);
            if (straightCard != null)
            {
                player.Rank = HandRank.Straight;
                player.RankCard = straightCard;
                return;
            }

            var threeOfAKindCard = IsThreeOfAKind(Hand);
            if (threeOfAKindCard != null)
            {
                player.Rank = HandRank.ThreeOfAKind;
                player.RankCard = threeOfAKindCard;
                return;
            }

            var twoPairCard = IsTwoPair(Hand);
            if (twoPairCard != null)
            {
                player.Rank = HandRank.TwoPair;
                player.RankCard = twoPairCard;
                return;
            }

            var onePairCard = IsOnePair(Hand);
            if (onePairCard != null)
            {
                player.Rank = HandRank.OnePair;
                player.RankCard = onePairCard;
                return;
            }

            var highCard = Hand.OrderByDescending(card => card.Value)
                               .ThenByDescending(card => card.Kind)
                               .FirstOrDefault();

            if (highCard != null)
            { 
                player.Rank = HandRank.HighCard;
                player.RankCard = highCard;
                return;
            }

            player.Rank = HandRank.None;
            player.RankCard = null;
        }

        private Card IsRoyalFlush(List<Card> hand)
        {
            var straightFlushCard = IsStraightFlush(hand);
            if (straightFlushCard != null && hand.Any(card => card.Value == 14))
            {
                return hand.OrderByDescending(card => card.Value)
                           .ThenByDescending(card => card.Kind)
                           .FirstOrDefault();
            }

            return null;
        }

        private Card IsStraightFlush(List<Card> hand)
        {
            var flushCard = IsFlush(hand);
            var straightCard = IsStraight(hand);

            if (flushCard != null && straightCard != null)
            {
                return hand.OrderByDescending(card => card.Value)
                           .ThenByDescending(card => card.Kind)
                           .FirstOrDefault();
            }

            return null;
        }

        private Card IsFourOfAKind(List<Card> hand)
        {
            var fourOfAKindGroup = hand.GroupBy(card => card.Value)
                                       .Where(group => group.Count() == 4)
                                       .OrderByDescending(group => group.Key)
                                       .FirstOrDefault();

            return fourOfAKindGroup?.OrderByDescending(card => card.Kind).FirstOrDefault();
        }

        private Card IsFullHouse(List<Card> hand)
        {
            var groups = hand.GroupBy(card => card.Value).ToList();
            var threeOfAKindGroup = groups.FirstOrDefault(group => group.Count() == 3);
            var pairGroup = groups.FirstOrDefault(group => group.Count() == 2);

            if (threeOfAKindGroup != null && pairGroup != null)
            {
                return threeOfAKindGroup.OrderByDescending(card => card.Value)
                                        .ThenByDescending(card => card.Kind)
                                        .FirstOrDefault();
            }

            return null;
        }

        private Card IsFlush(List<Card> hand)
        {
            var flushGroup = hand.GroupBy(card => card.Kind)
                                 .Where(group => group.Count() == 5)
                                 .OrderByDescending(group => group.Max(card => card.Value))
                                 .FirstOrDefault();

            return flushGroup?.OrderByDescending(card => card.Value)
                             .ThenByDescending(card => card.Kind)
                             .FirstOrDefault();
        }

        private Card IsStraight(List<Card> hand)
        {
            var orderedValues = hand.Select(card => card.Value).OrderBy(value => value).ToList();
            bool isStraight = orderedValues.Zip(orderedValues.Skip(1), (a, b) => (a - b)).All(diff => diff == 1);

            return isStraight ? hand.OrderByDescending(card => card.Value)
                                      .ThenByDescending(card => card.Kind)
                                      .FirstOrDefault() : null;
        }

        private Card IsThreeOfAKind(List<Card> hand)
        {
            var threeOfAKindGroup = hand.GroupBy(card => card.Value)
                                        .Where(group => group.Count() == 3)
                                        .OrderByDescending(group => group.Key)
                                        .FirstOrDefault();

            return threeOfAKindGroup?.OrderByDescending(card => card.Value)
                        .ThenByDescending(card => card.Kind)
                        .FirstOrDefault();
        }

        private Card IsTwoPair(List<Card> hand)
        {
            var pairGroups = hand.GroupBy(card => card.Value)
                                 .Where(group => group.Count() == 2)
                                 .OrderByDescending(group => group.Key)
                                 .Take(2)
                                 .ToList();

            if (pairGroups.Count == 2)
            {
                return pairGroups.SelectMany(group => group)
                    .OrderByDescending(card => card.Value)
                    .ThenByDescending(card => card.Kind)
                    .FirstOrDefault();
            }

            return null;
        }

        private Card IsOnePair(List<Card> hand)
        {
            var pairGroup = hand.GroupBy(card => card.Value)
                                .Where(group => group.Count() == 2)
                                .OrderByDescending(group => group.Key)
                                .FirstOrDefault();

            return pairGroup?.OrderByDescending(card => card.Value)
                            .ThenByDescending(card => card.Kind)
                            .FirstOrDefault();
        }
    }
}
