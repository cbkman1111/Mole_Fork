using System;
using System.Collections.Generic; // Add missing import statement

namespace Common.Table
{
    [Serializable]
    public class Skill
    {
        public long ID { get; set; }
        public int TYPE { get; set; }
        public string NAME_TID { get; set; }
        public string ICON { get; set; }
        public float COOL_TIME { get; set; }
        public int RANGE { get; set; }
        public int FACTOR { get; set; }
        public float FRAME { get; set; }
        public List <ProjectileState> PROJECTILE_STATES { get; set; }
        public List<SkillState> SKILL_STATES { get; set; }
    }

    [Serializable]
    public class ProjectileState
    {
        public float START_TIME { get; set; }
        public int RANGE_TYPE { get; set; }
        public int PROJECTILE_TYPE { get; set; }
        public long PROJECTILE_ID { get; set; }
    }

    [Serializable] 
    public class SkillState
    {
        public float START_TIME { get; set; }
        public int STATE_TARGET { get; set; }
        public int STATE_TYPE { get; set; }
        public long STATE_ID { get; set; } 
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