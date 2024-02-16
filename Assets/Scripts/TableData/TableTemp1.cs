using System;
using Unity.Collections;
using System.Collections.Generic; // Add missing import statement

namespace Common.Table
{ 
    [Serializable]
    public class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Area { get; set; }
    }

    [Serializable]
    public class TableTemp1 : DataTable
    {
        public List<Person> Data {get; set;}

        TableTemp1()
        {
            Data = new List<Person>();            
        }
    }    
}