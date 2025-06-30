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
        public int AGE { get; set; }
        public int ENABLE { get; set; }
        public string SKILLS { get; set; }
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