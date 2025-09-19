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
        public string TableName { get; set; } 
        public Dictionary<string, object> DataKey { get; set; } 
        public int HeaderLine { get; set; }
        public int DataLine => HeaderLine;
        public int LastRow()
        {
            int lastDataRow = -1;
            for (int i = 0; i <= Sheet.LastRowNum; i++)
            {
                IRow row = Sheet.GetRow(i);
                if (row == null)
                    continue;

                // row에 실제 데이터가 있는지 확인
                bool hasData = false;
                for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                {
                    ICell cell = row.GetCell(j);
                    if (cell != null && cell.CellType != NPOI.SS.UserModel.CellType.Blank && !string.IsNullOrEmpty(cell.ToString()))
                    {
                        hasData = true;
                        break;
                    }
                }
                if (hasData)
                    lastDataRow = i;
            }

            return lastDataRow;
        } 

        public ExcelHeader(ISheet sheet)
        {
            Sheet = sheet;
            DataKey = new Dictionary<string, object>();

            IRow row0 = GetRow(0);
            TableName = row0.GetCell(0).ToString().Replace("#", "");
            
            // 헤더의 첫번째 셀은 테이블 이름이므로 제외하고 시작.
            for (int i = 1; i < row0.LastCellNum; i++)
            {
                ICell cell = row0.GetCell(i);
                CellAddress address = cell.Address;

                var name = cell.ToString();
                
                if (string.IsNullOrEmpty(name) == true || name.Contains("!") == true)
                {
                    continue;
                }

                // 배열 묶음
                else if (name.Contains("[{}]") == true)
                {
                    // 리스트를 넣고.
                    var list = new List<object>();

                    // 배열의 수량 대입.
                    IRow row1 = GetRow(1);
                    List<object> children = new();

                    // 1. 컬럼 수량 계산.
                    int totalColumns = 0;
                    for (int col = i; col < row1.LastCellNum; col++)
                    {
                        ICell cellTop = row0.GetCell(col);
                        if (cell != cellTop && cellTop.CellType != CellType.Blank)
                            break;

                        totalColumns++;
                    }
                    
                    int lastColumn = i + totalColumns;
                    for (int col = i; col < lastColumn; col++)
                    {
                        ICell cellNext = row1.GetCell(col);
                        if (cellNext.ToString() == "{}")
                            children.Add(cellNext.Address);
                    }

                    // 2. 각 배열 묶음의 하위 필드명 추출
                    int dataSize = totalColumns / children.Count;
                    IRow row2 = GetRow(2);

                    for (int j = 0; j < children.Count; j++)
                    {
                        CellAddress childAddress = children[j] as CellAddress;
                        var dic = new Dictionary<string, CellAddress>();
                        for (int k = 0; k < dataSize; k++)
                        {
                            ICell cellNext = row2.GetCell(childAddress.Column + k);
                            if (cellNext == null || cellNext.CellType == CellType.Blank || cellNext.ToString().Contains("!") == true)
                                continue;

                            dic.Add(cellNext.ToString(), cellNext.Address);
                        }

                        list.Add(dic);
                    }

                    DataKey.Add(name, list);
                }
                // 배열의 값.
                else if (name.Contains("[]") == true)
                {
                    var list = new List<CellAddress>();
                    for (int j = i; j < row0.LastCellNum; j++)
                    {
                        ICell cellTop = row0.GetCell(j);
                        if (cellTop == null)
                            continue;

                        if (j > i && cellTop.CellType != CellType.Blank)
                            break;

                        list.Add(cellTop.Address);
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
                    var cell = list.First();
                    for (int i = cell.Row + 1; i < 10; i++)
                    {
                        IRow rowNext = GetRow(i);
                        var cellNext = rowNext.GetCell(cell.Column);
                        var name = cellNext.ToString();

                        if (string.IsNullOrEmpty(name) == true || name.Contains("!") == true)
                            row = cellNext.Address.Row;
                        else
                            break;
                    }

                    row = Math.Max(row, list.First().Row);

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