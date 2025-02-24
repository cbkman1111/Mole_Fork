using System.Collections.Generic;
using System.Linq;

namespace Poker
{
    public enum BetType
    {
        Fold,
        Check,
        Bet,
        Call,
        Raise,
        AllIn,
    }

    public enum PlayUser
    {
        Player1 = 0,
        Player2,
        Player3,
        Player4,
        Player5,

        Observer1,
        Observer2,
        Observer3,
        Observer4,
        Observer5,

        Max,
    }

    public enum HandRank
    {
        None = 0,

        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }
}
