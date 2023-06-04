using System;

namespace SweetSugar.Scripts.Integrations.Network.EpsilonServer
{
    [Serializable]
    public class ResultObject
    {
        public string facebookId;
        public string playerId;
        public int score;
        public int stars;
        //for levels table
        public int level;
        //for players table
        public int maxLevel;
        //for boost table
        public string name;
        public int count;
    }
}