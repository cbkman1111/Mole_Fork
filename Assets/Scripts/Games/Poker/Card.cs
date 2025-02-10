using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pocker.UIPokerCard;

namespace Poker
{
    public class Card
    {
        public enum CardKind
        {
            Clubs = 0,
            Hearts,
            Diamond,
            Spades
        }

        public CardKind Kind { get; set; }
        public int Value { get; set; } // 2-14 (11: Jack, 12: Queen, 13: King, 14: Ace)

        public Card(CardKind kind, int value)
        {
            Kind = kind;
            Value = value;
        }
    }
}