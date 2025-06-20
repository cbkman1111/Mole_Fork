using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExcelConverter.Editor
{
    /// <summary>
    /// ExcelConverter Editor Window to convert Excel files to JSON format.
    /// </summary>
    public partial class ExcelConverter : EditorWindow
    {

        private ExcelHeader GetHeader(ISheet sheet)
        {
            int headerIndex = 0;
            IRow rowFirst = sheet.GetRow(headerIndex);

            ExcelHeader excelHeader = new ExcelHeader();
            excelHeader.TableName = rowFirst.GetCell(0).ToString().Replace("#", "");
            excelHeader.DataKey = new Dictionary<string, object>();
            excelHeader.Data = new List<Dictionary<string, object>>();

            // 헤더의 첫번째 셀은 테이블 이름이므로 제외하고 시작.
            for (int i = 1; i < rowFirst.LastCellNum; i++)
            {
                ICell cell = rowFirst.GetCell(i);
                CellAddress address = cell.Address;
                var c = address.Column;
                var r = address.Row;

                var name = cell.ToString();
                if (name == string.Empty)
                {
                    continue;
                }

                // 배열.
                if (name.Contains("[]") == true)
                {
                    var list = new List<CellAddress>();
                    excelHeader.DataKey.Add(name, list);
                    for (int j = i; j < rowFirst.LastCellNum; j++)
                    {
                        ICell cellTop = rowFirst.GetCell(j);
                        if (cellTop == null)
                            continue;

                        if (j > i && cellTop.CellType != CellType.Blank)
                            break;

                        list.Add(cellTop.Address);
                    }
                }
                // 배열 묶음
                else if (name.Contains("[{}]") == true)
                {
                    // 리스트를 넣고.
                    var list = new List<object>();
                    excelHeader.DataKey.Add(name, list);

                    // 배열의 수량 대입.
                    int size = 0;
                    IRow rowSecond = sheet.GetRow(headerIndex + 1);
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
                    IRow rowArrayData = sheet.GetRow(headerIndex + 2);
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
                }
                // 일반 헤더.
                else
                {
                    excelHeader.DataKey.Add(name, address);
                }
            }

            return excelHeader;
        }

        public ICell GetCell(IRow row, int col)
        {
            return row.GetCell(col);
        }


        private object GetValueFromCell(ICell cell)
        {
            if (cell == null || cell.CellType == CellType.Blank)
                return null;
            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue;

                case CellType.Numeric:
                    // 정수/소수 구분
                    double d = cell.NumericCellValue;
                    if (Math.Floor(d) == d)
                        return (int)d; // 정수로 반환
                    else
                        return d;      // 소수(실수)로 반환

                case CellType.Boolean:
                    return cell.BooleanCellValue;

                case CellType.Formula:
                    var eval = Evaluator.Evaluate(cell);
                    switch (eval.CellType)
                    {
                        case CellType.String:
                            return eval.StringValue;
                        case CellType.Numeric:
                            double val = eval.NumberValue;
                            if (Math.Floor(val) == val)
                                return (int)val;
                            else
                                return val;
                        case CellType.Boolean:
                            return eval.BooleanValue;
                        default:
                            return eval.FormatAsString();
                    }
                default:
                    return cell.ToString();
            }
        }
    }
}
