using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ExcelConverter.Editor
{
    /// <summary>
    /// ExcelConverter Editor Window to convert Excel files to JSON format.
    /// </summary>
    public partial class ExcelConverter : EditorWindow
    {
        private int SheetNum = 0; // Sheet Number to Convert
        private IFormulaEvaluator Evaluator;

        /// <summary>
        /// 초기화.
        /// </summary>
        public void Init()
        {
            ExcelFilePath = string.Empty;
            JsonOutputPath = string.Empty;
            SheetNum = 0;
        }

        /// <summary>
        /// C# 클래스로 변환하는 기능은 현재 구현되어 있지 않습니다.   
        /// </summary>
        public void ConvertCSharpClass()
        {}


        /// <summary>
        /// 엑셀 파일을 읽어서 JSON 파일로 변환합니다.
        /// </summary>
        /// <param name="sheetNum"></param>
        public void ConvertExcelToJson(int sheetNum)
        {
            if (string.IsNullOrEmpty(ExcelFilePath) || string.IsNullOrEmpty(JsonOutputPath))
            {
                Debug.LogError("Excel Path, Json Path is NULL");
                return;
            }

            // 파일 오픈.
            using (FileStream stream = new FileStream(ExcelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // 워크 시트 열기.
                IWorkbook workbook = new XSSFWorkbook(stream);
                Evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();

                for (int sheetIdx = 0; sheetIdx < workbook.NumberOfSheets; sheetIdx++)
                {
                    ISheet sheet = workbook.GetSheetAt(sheetIdx);
                    if (sheet == null)
                        continue;

                    // 헤더 정보를 추출.
                    var header = GetHeader(sheet);

                    // 데이터 취합.
                    for (int i = header.GetDataRowLine(); i < sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null)
                            continue;

                        Dictionary<string, object> data = new();
                        foreach (var pair in header.DataKey)
                        {
                            var key = pair.Key;

                            if (key.Contains("[]") == true)
                            {
                                var name = key.Replace("[]", "");
                                var list = pair.Value as List<CellAddress>;
                                var dataList = new List<object>();

                                foreach (var cell in list)
                                {
                                    var address = cell as CellAddress;
                                    dataList.Add(GetValueFromCell(GetCell(row, address.Column)));
                                }

                                data.Add(name, dataList);
                            }
                            else if (key.Contains("[{}]") == true)
                            {
                                var name = key.Replace("[{}]", "");
                                var list = pair.Value as List<object>;
                                var dataList = new List<object>();
                                
                                foreach (var obj in list)
                                {
                                    var dic = obj as Dictionary<string, CellAddress>;
                                    var dataDic = new Dictionary<string, object>();
                                    dataList.Add(dataDic);
                                    foreach (var p in dic)
                                    {
                                        Debug.Log($"Column Name: {p.Key}, Address: {p.Value}");
                                        dataDic.Add(p.Key, GetValueFromCell(GetCell(row, p.Value.Column)));
                                    }
                                }

                                data.Add(name, dataList);
                            }
                            else
                            {
                                var name = key;
                                var address = pair.Value as CellAddress;
                                var cell = GetCell(row, address.Column);
                                var value = GetValueFromCell(cell);
                                data.Add(name, value);
                            }
                        }

                        header.Data.Add(data);
                    }

                    // 엑셀 파일에서 추출된 정보를 json 파일로 저장.
                    var table = new Dictionary<string, object> {
                            { "Data", header.Data }
                        };

                    string json = JsonConvert.SerializeObject(table, Formatting.Indented);
                    string path = $"{JsonOutputPath}/{header.TableName}.json";
                    File.WriteAllText(path, json);
                }

                // 완료 팝업.
                EditorUtility.DisplayDialog("Excel to Json Conversion", "Conversion completed successfully!", "OK");
            }
        }
    }
}
