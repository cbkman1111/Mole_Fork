using System;
using System.Collections.Generic; // Add missing import statement

namespace Common.Table
{
        
    [Serializable]
    public class PersonSimple
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    [Serializable]
    public class TableTemp2 : DataTable
    {
        public List<PersonSimple> Data {get; set;}

        TableTemp2()
        {
            Data = new List<PersonSimple>();            
        }
    }
}