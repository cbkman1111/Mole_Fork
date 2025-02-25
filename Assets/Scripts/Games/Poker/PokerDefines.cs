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

    public enum SevenPokerState
    {
        None = 0,
        Start,
        CardDeal3, // 3장 돌리기
        OpenFirstCard, // 1장 오픈
        Bet1, // 1차 베팅(3구)
        CardDeal4, //  4번째 카드 받기
        Bet2, // 2차 베팅(4구)
        CardDeal5, // 5번째 카드 받기
        Bet3, // 3차 베팅(5구)
        CardDeal6, // 6번째 카드 받기
        Bet4, // 4차 베팅(6구)
        CardDealHidden, // 7번째 히든 카드 받기
        Bet5Last, // 5차 베팅(7구)
        CheckWinner, // 승자 확인
        GameOver,
    }
}
