using System.Collections.Generic;
using System.Linq;
using Common.Global.Singleton;
using Poker;

namespace Common.Global
{
    public class GlobalGameManager : MonoSingleton<GlobalGameManager>
    {
        public Poker.PokerData PockerData = null;

        protected override bool Init()
        {
            PockerData = null;
            return true;
        }

        public Poker.PokerData CreatePokerGame(long[] players)
        {
            PockerData = new Poker.PokerData(players);
            return PockerData;
        }


    }
}