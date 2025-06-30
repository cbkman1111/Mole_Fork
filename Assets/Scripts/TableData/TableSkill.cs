using System;
using System.Collections.Generic; // Add missing import statement

namespace Common.Table
{ 
    [Serializable]
    public class Skill
    {
        public long ID { get; set; }
        public string NAME_TID { get; set; }
        public int RANGE { get; set; }
        public int COOL_TIME { get; set; }
        public string PROJECTILE { get; set; }
    }

    [Serializable]
    public class TableSkill : DataTable
    {
        public Dictionary<long, Skill> Data {get; set;}

        TableSkill()
        {
            Data = new ();            
        }
    }    
}