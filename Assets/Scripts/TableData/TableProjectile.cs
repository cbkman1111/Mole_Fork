using System;
using System.Collections.Generic; // Add missing import statement

namespace Common.Table
{ 
    [Serializable]
    public class Projectile
    {
        public long ID { get; set; }
        public string NAME_TID { get; set; }
        public string PREFAB { get; set; }
        public List<HitState> HIT_STATE { get; set; }
        public List<int> TEST { get; set; }
        public int LAST { get; set; }
    }

    [Serializable]
    public class HitState
    {
        public int STATE_TYPE { get; set; }
        public long ID { get; set; }
    }

    [Serializable]
    public class TableProjectile : DataTable
    {
        public Dictionary<long, Projectile> Data { get; set; }

        TableProjectile()
        {
            Data = new (); 
        }
    }    
}