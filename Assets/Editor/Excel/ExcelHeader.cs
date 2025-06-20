using System.Collections.Generic;
using NPOI.SS.Util;
using System.Linq;
using System;
using NPOI.SS.UserModel;

namespace ExcelConverter.Editor
{
    /// <summary>
    /// Excel Header Class to hold table name, data row index and data dictionary.
    /// </summary>
    public class ExcelHeader
    {
        public string TableName { get; set; } // Name of the Table
        public Dictionary<string, object> DataKey; // Dictionary to hold column names and their corresponding cell addresses or values
        public List<Dictionary<string, object>> Data;


        /// <summary>
        /// 헤더 다음 Row 리턴. (데이터의 시작)
        /// </summary>
        /// <returns></returns>
        public int GetDataRowLine()
        {
            int row = 0;

            foreach (var pair in DataKey)
            {
                var key = pair.Key;

                if (key.Contains("[]") == true)
                {
                    var list = pair.Value as List<CellAddress>;
                    var c = list.First().Row;
                    row = Math.Max(row, c);
                }
                else if (key.Contains("[{}]") == true)
                {
                    var list = pair.Value as List<object>;
                    var dic = list.First() as Dictionary<string, CellAddress>;
                    row = Math.Max(row, dic.First().Value.Row);
                }
                else
                {
                    var address = pair.Value as CellAddress;
                    row = Math.Max(row, address.Row);
                }
            }

            // 모든 객체를 돌아서 Row 젤 높은거 + 1 
            return row + 1;
        }
    }
}