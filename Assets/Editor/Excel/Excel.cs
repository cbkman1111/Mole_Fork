using System.Collections.Generic;
using System.IO;
using NPOI.SS.Util;
using System.Linq;
using UnityEngine;
using System;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using Newtonsoft.Json;
using System.Runtime.Remoting.Messaging;
using MathNet.Numerics.LinearAlgebra;

namespace Giant.Excel
{
    public class Excel
    {
        private string FileInput = string.Empty;
        private string FileOutput = string.Empty;
        
        private IWorkbook Workbook = null;

        public Dictionary<string, object> DataKey;
        public List<Dictionary<string, object>> Data;
        
        private IFormulaEvaluator Evaluator;

        public Excel(string file, string output)
        {
            FileInput = file;
            FileOutput = output;
          
            bool success = ReadExcelFile();
            if (!success)
            {
                Debug.LogError("Failed to read Excel file.");
                return;
            }

            ConvertToJson();
        }

        /// <summary>
        /// 엑셀 파일을 읽어옵니다.
        /// </summary>
        /// <returns></returns>
        private bool ReadExcelFile()
        {
            if (string.IsNullOrEmpty(FileInput) || string.IsNullOrEmpty(FileOutput))
            {
                Debug.LogError("Excel Path, Json Path is NULL");
                return false;
            }

            using (FileStream stream = new FileStream(FileInput, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Workbook = new XSSFWorkbook(stream);
                Evaluator = Workbook.GetCreationHelper().CreateFormulaEvaluator();
            }

            return true;
        }

        /// <summary>
        /// 엑셀 파일을 JSON 으로 변환.
        /// </summary>
        private void ConvertToJson()
        {
            Dictionary<string, Dictionary<long, object>> combineData = new();
            //Dictionary<long, object> combineData = new();
            //string tableName = string.Empty;

            for (int sheetIdx = 0; sheetIdx < Workbook.NumberOfSheets; sheetIdx++)
            {
                ISheet sheet = Workbook.GetSheetAt(sheetIdx);
                if (sheet == null)
                    continue;

                if (sheet.SheetName == "NOTE")
                    continue;

                // 헤더 정보를 추출.
                var header = new ExcelHeader(sheet);
                //tableName = header.TableName;

                // row 데이터 취합.
                for (int i = header.DataLine; i <= header.LastRow; i++)
                {
                    IRow row = header.GetRow(i);
                    if (row == null)
                        continue;

                    Dictionary<string, object> rowData = new();
                    foreach (var pair in header.DataKey)
                    {
                        var key = pair.Key;
                          
                        if (key.Contains("[{}]") == true)
                        {
                            var name = key.Replace("[{}]", "");
                            var list = pair.Value as List<object>;
                            var dataList = new List<object>();

                            foreach (var obj in list)
                            {
                                var dic = obj as Dictionary<string, CellAddress>;
                                var dataDic = new Dictionary<string, object>();
                                dataList.Add(dataDic);
                                foreach (var dataPair in dic)
                                {
                                    var value = GetValueFromCell(GetCell(row, dataPair.Value.Column));
                                    dataDic.Add(dataPair.Key, value);
                                }
                            }

                            rowData.Add(name, dataList);
                        }
                        else if (key.Contains("[]") == true)
                        {
                            var name = key.Replace("[]", "");
                            var list = pair.Value as List<CellAddress>;
                            var dataList = new List<object>();

                            foreach (var cell in list)
                            {
                                dataList.Add(GetValueFromCell(GetCell(row, cell.Column)));
                            }

                            rowData.Add(name, dataList);
                        }
                        else
                        {
                            var name = key;
                            var address = pair.Value as CellAddress;
                            var cell = GetCell(row, address.Column);
                            var value = GetValueFromCell(cell);
                            rowData.Add(name, value);
                        }
                    }

                    rowData.TryGetValue("ID", out object id);
                    var uniqueKey = long.Parse(id.ToString());

                    if(combineData.TryGetValue(header.TableName, out var existingData))
                    {
                        // 이미 존재하는 키라면 덮어쓰기
                        existingData[uniqueKey] = rowData;
                    }
                    else
                    {
                        // 새로운 테이블 이름으로 추가
                        var tableData = new Dictionary<long, object> { { uniqueKey, rowData } };
                        combineData.Add(header.TableName, tableData);
                    }

                    //combineData.Add(uniqueKey, rowData);
                }
            }

            foreach (var data in combineData)
            {
                var key = data.Key;
                var dic = data.Value;
                WriteJson(key, dic);
            }
                
            // 완료 팝업.
            EditorUtility.DisplayDialog("Excel to Json Conversion", "Conversion completed successfully!", "OK");
        }

        /// <summary>
        /// 헤더 정보를 JSON 파일로 저장합니다.
        /// </summary>
        /// <param name="header"></param>
        private void WriteJson(string tableName, Dictionary<long, object> data)
        {                 
            // 엑셀 파일에서 추출된 정보를 json 파일로 저장.
            var table = new Dictionary<string, object> {
                        { "Data", data }
                    };

            string json = JsonConvert.SerializeObject(table, Formatting.Indented);
            string path = $"{FileOutput}/{tableName}.json";
            
            File.WriteAllText(path, json);
        }

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
