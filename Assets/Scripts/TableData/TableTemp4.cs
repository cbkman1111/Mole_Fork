using System;
using System.Collections.Generic; // Add missing import statement

namespace Common.Table
{
    [Serializable]
    public class TableTemp4 : DataTable
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}