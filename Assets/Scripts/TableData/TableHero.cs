using System;
using System.Collections.Generic; // Add missing import statement

namespace Common.Table
{ 
    [Serializable]
    public class Hero
    {
        public int ID { get; set; }
        public int TYPE { get; set; }
        public string NAME_TID { get; set; }
        public float RADIUS { get; set; }
        public string PREFAB { get; set; }
        public string ICON { get; set; }
        public int TIER { get; set; }    
        public int AGE { get; set; } 
        public long LEVEL_GROUP_ID { get; set; }
        public List<int> KEYWORDS { get; set; }
        public List<long> SKILLS { get; set; }
    }

    [Serializable]
    public class TableHero : DataTable
    {
        public Dictionary<long, Hero> Data {get; set;}

        TableHero()
        {
            Data = new ();            
        }
    }    
}