using Common.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Scenes.ScenePoker;

namespace Poker
{
    public class Player
    {
        public List<Card> Hand { get; set; } = new List<Card>();

        public void AddCard(Card card)
        {
            Hand.Add(card);
        }

        public int[] GetCards()
        {
            return Hand.Select(x => x.Value).ToArray();
        }


        public HandRank EvaluateHand()
        {
            if (IsRoyalFlush(Hand)) 
                return HandRank.RoyalFlush;

            if (IsStraightFlush(Hand)) 
                return HandRank.StraightFlush;

            if (IsFourOfAKind(Hand)) 
                return HandRank.FourOfAKind;

            if (IsFullHouse(Hand)) 
                return HandRank.FullHouse;

            if (IsFlush(Hand)) 
                return HandRank.Flush;

            if (IsStraight(Hand)) 
                return HandRank.Straight;

            if (IsThreeOfAKind(Hand)) 
                return HandRank.ThreeOfAKind;

            if (IsTwoPair(Hand)) 
                return HandRank.TwoPair;

            if (IsOnePair(Hand)) 
                return HandRank.OnePair;

            return HandRank.HighCard;
        }

        private bool IsRoyalFlush(List<Card> hand)
        {
            return IsStraightFlush(hand) && hand.Any(card => card.Value == 14);
        }

        private bool IsStraightFlush(List<Card> hand)
        {
            return IsFlush(hand) && IsStraight(hand);
        }

        private bool IsFourOfAKind(List<Card> hand)
        {
            return hand.GroupBy(card => card.Value).Any(group => group.Count() == 4);
        }

        private bool IsFullHouse(List<Card> hand)
        {
            var groups = hand.GroupBy(card => card.Value).ToList();
            return groups.Count == 2 && groups.Any(group => group.Count() == 3);
        }

        private bool IsFlush(List<Card> hand)
        {
            return hand.GroupBy(card => card.Kind).Any(group => group.Count() == 5);
        }

        private bool IsStraight(List<Card> hand)
        {
            var orderedValues = hand.Select(card => card.Value).OrderBy(value => value).ToList();
            return orderedValues.Zip(orderedValues.Skip(1), (a, b) => (b - a)).All(diff => diff == 1);
        }

        private bool IsThreeOfAKind(List<Card> hand)
        {
            return hand.GroupBy(card => card.Value).Any(group => group.Count() == 3);
        }

        private bool IsTwoPair(List<Card> hand)
        {
            return hand.GroupBy(card => card.Value).Count(group => group.Count() == 2) == 2;
        }

        private bool IsOnePair(List<Card> hand)
        {
            return hand.GroupBy(card => card.Value).Any(group => group.Count() == 2);
        }

    }
}
