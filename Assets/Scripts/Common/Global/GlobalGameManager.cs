using System.Collections.Generic;
using System.Linq;
using Common.Global.Singleton;
using Poker;

namespace Common.Global
{
    public class GlobalGameManager : MonoSingleton<GlobalGameManager>
    {
        public Poker.SevenPokerData PockerGameData { get; set; } = null;

        protected override bool Init()
        {
            PockerGameData = null;
            return true;
        }

        public Poker.SevenPokerData CreatePokerGame(long[] players)
        {
            PockerGameData = new Poker.SevenPokerData(players);
            return PockerGameData;
        }
    }
}