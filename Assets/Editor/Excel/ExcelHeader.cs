using System.Collections.Generic;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Linq;
using System;

namespace Giant.Excel
{
    public class ExcelHeader
    {
        private ISheet Sheet = null;
        public string TableName { get; set; } // Name of the Table
        public Dictionary<string, object> DataKey { get; set; } // Dictionary to hold column names and their corresponding cell addresses or values
        public Dictionary<long, object> Data { get; set; }
        public int HeaderLine { get; set; } // Header Row Line
        public int DataLine => HeaderLine + 1;
        public int LastRow => Sheet.LastRowNum;

        public ExcelHeader(ISheet sheet)
        {
            Sheet = sheet;

            int headerIndex = 0;
            IRow rowFirst = GetRow(headerIndex);
            TableName = rowFirst.GetCell(0).ToString().Replace("#", "");
            DataKey = new Dictionary<string, object>();
            Data = new Dictionary<long, object>();

            // 헤더의 첫번째 셀은 테이블 이름이므로 제외하고 시작.
            for (int i = 1; i < rowFirst.LastCellNum; i++)
            {
                ICell cell = rowFirst.GetCell(i);
                CellAddress address = cell.Address;

                var name = cell.ToString();
                if (name == string.Empty)
                {
                    continue;
                }

                // 배열.
                if (name.Contains("[]") == true)
                {
                    var list = new List<CellAddress>();
                    for (int j = i; j < rowFirst.LastCellNum; j++)
                    {
                        ICell cellTop = rowFirst.GetCell(j);
                        if (cellTop == null)
                            continue;

                        if (j > i && cellTop.CellType != CellType.Blank)
                            break;

                        list.Add(cellTop.Address);
                    }

                    DataKey.Add(name, list);
                }
                // 배열 묶음
                else if (name.Contains("[{}]") == true)
                {
                    // 리스트를 넣고.
                    var list = new List<object>();

                    // 배열의 수량 대입.
                    int size = 0;
                    IRow rowSecond = GetRow(headerIndex + 1);
                    List<object> childs = new();
                    for (int j = i; j < rowSecond.LastCellNum; j++)
                    {
                        ICell cellTop = rowFirst.GetCell(j);
                        ICell cellNext = rowSecond.GetCell(j);
                        if (cellNext == null)
                            continue;

                        if (j > i && cellTop.CellType != CellType.Blank)
                            break;

                        if (cellNext.ToString() == "{}")
                            childs.Add(cellNext.Address);

                        size++;
                    }

                    int dataSize = size / childs.Count;
                    IRow rowArrayData = GetRow(headerIndex + 2);
                    for (int j = 0; j < childs.Count; j++)
                    {
                        CellAddress childAddress = childs[j] as CellAddress;
                        var dic = new Dictionary<string, CellAddress>();
                        for (int k = 0; k < dataSize; k++)
                        {
                            ICell cellNext = rowArrayData.GetCell(childAddress.Column + k);
                            if (cellNext == null || cellNext.CellType == CellType.Blank)
                                continue;

                            dic.Add(cellNext.ToString(), cellNext.Address);
                        }

                        list.Add(dic);
                    }

                    DataKey.Add(name, list);
                }
                // 일반 헤더.
                else
                {
                    DataKey.Add(name, address);
                }
            }

            HeaderLine = GetHeaderLine();
        }

        public IRow GetRow(int rowIndex)
        {
            if (Sheet == null)
                return null;

            return Sheet.GetRow(rowIndex);
        }

        /// <summary>
        /// 헤더 다음 Row 리턴. (데이터의 시작)
        /// </summary>
        /// <returns></returns>
        private int GetHeaderLine()
        {
            int row = 0;

            foreach (var pair in DataKey)
            {
                var key = pair.Key;
                if (key.Contains("[{}]") == true)
                {
                    var list = pair.Value as List<object>;
                    var dic = list.First() as Dictionary<string, CellAddress>;
                    row = Math.Max(row, dic.First().Value.Row);
                }
                else if (key.Contains("[]") == true)
                {
                    var list = pair.Value as List<CellAddress>;
                    var c = list.First().Row;
                    row = Math.Max(row, c);
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