using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

namespace ExcelConverter.Editor
{
    public partial class ExcelConverter : EditorWindow
    {
        private const string MenuNameRoot = "엑셀/엑셀 익스포터";
        private const string MenuNameOpen = MenuNameRoot + "/열기";
        
        private string excelFilePath = string.Empty; // Path to Excel File
        private string jsonOutputPath = string.Empty; // Path to Save Json File
        private int sheetNum = 0; // Sheet Number to Convert

        /// <summary>
        /// 매치3 -> 스테이지 에디터 -> 열기
        /// </summary>
        [MenuItem(MenuNameOpen)]
        private static void Open()
        {
            var editorWindow = GetWindow(typeof(ExcelConverter));
            editorWindow.titleContent = new GUIContent("엑셀 익스포터");

            ExcelConverter levelEditor = editorWindow as ExcelConverter;
            levelEditor.Init();
        }

        /// <summary>
        /// 초기화.
        /// </summary>
        public void Init()
        {
            excelFilePath = string.Empty;
            jsonOutputPath = string.Empty;
            sheetNum = 0;
        }


        public void ConvertCSharpClass()
        {
            
        }

        public void ConvertExcelToJson(int sheetNum)
        {
            if (string.IsNullOrEmpty(excelFilePath) || string.IsNullOrEmpty(jsonOutputPath))
            {
                Debug.LogError("Excel Path, Json Path is NULL");
                return;
            }

            using (FileStream stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //using (FileStream stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(stream);

                var root = new Dictionary<string, object>();

                for (int sheetIdx = 0; sheetIdx < workbook.NumberOfSheets; sheetIdx++)
                {
                    ISheet sheet = workbook.GetSheetAt(sheetIdx);
                    if (sheet == null)
                        continue;

                    var rows = new List<Dictionary<string, object>>();
                   
                    IRow headerRow = sheet.GetRow(0);
                    var tableName = headerRow.GetCell(0).ToString().Replace("#", ""); // Get Table Name from Header Row
                    int cellCount = headerRow.LastCellNum;

                    // Extract Data looping every row in sheet
                    for (int i = 1; i <= sheet.LastRowNum; i++) // 0 is Header
                    {
                        IRow row = sheet.GetRow(i);
                        var rowData = new Dictionary<string, object>();

                        ICell mergedCell = null;
                        for (int j = 1; j < cellCount; j++)
                        {
                            ICell cellHeader = headerRow.GetCell(j);
                            if (cellHeader.IsMergedCell == true)
                            {
                                if (mergedCell == null)
                                {
                                    mergedCell = cellHeader;
                                    rowData[mergedCell.ToString()] = new List<object>();
                                }

                                var list = rowData.Last().Value as List<object>;
                                ICell cellData = row.GetCell(j);
                                list.Add(GetValueFromCell(row.GetCell(j))); //Get Cell Value
                            }
                            else
                            {
                                mergedCell = null;
                                ICell cellData = row.GetCell(j);
                                string columnName = cellHeader.ToString(); //Get ColumnName from Header
                                rowData[columnName] = GetValueFromCell(cellData); //Get Cell Value
                            }
                        }

                        rows.Add(rowData); // Add Row data to List
                    }

                    //save Data after convert Json
                    // 감싸는 객체 생성

                    //if (root.TryGetValue(tableName, out var existRows))
                    if (root.TryGetValue(tableName, out var obj))
                    {
                        var existRows = obj as List<Dictionary<string, object>>;
                        existRows.AddRange(rows); // Add new rows to existing rows
                    }
                    else 
                    {
                        root.Add(tableName, rows);//
                    }
                }

                // 엑셀 파일에서 추출된 정보를 json 파일로 저장.
                foreach (var pair in root)
                {
                    var key = pair.Key;
                    var value = pair.Value as List<Dictionary<string, object>>;
                    var table = new Dictionary<string, object> {
                        { "Data", value }
                    };

                    string json = JsonConvert.SerializeObject(table, Formatting.Indented);
                    string path = $"{jsonOutputPath}/{key}.json";
                    
                    File.WriteAllText(path, json);
                    Debug.Log($"Convert Excel To Json : {path}");
                    Debug.Log($"{json.ToString()}");
                }

            }
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
                    return cell.CellFormula; // or evaluate the formula
                default:
                    return cell.ToString();
            }
        }
    }
}
