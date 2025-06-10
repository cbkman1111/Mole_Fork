using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Scenes.EllersAlgorithm;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;


public partial class ExcelConverter : EditorWindow
{
    string excelFilePath = string.Empty; // Path to Excel File
    string jsonOutputPath = string.Empty; // Path to Save Json File
    int sheetNum = 0; // Sheet Number to Convert

    void OnGUI()
    {
        try
        {
            //GUILoadBtn();

            GUILayout.Label("Select Excel File Path", EditorStyles.boldLabel);

            if (GUILayout.Button("Select Excel File"))
            {
                string path = EditorUtility.OpenFilePanel("Select Excel File", "", "xlsx");
                if (!string.IsNullOrEmpty(path))
                {
                    excelFilePath = path;
                }
            }

            EditorGUILayout.TextField("Excel File Path", excelFilePath);

            GUILayout.Space(5);

            GUILayout.Label("Sheet Number", EditorStyles.boldLabel);
            sheetNum = EditorGUILayout.IntField("Enter Sheet Number", sheetNum);

            GUILayout.Space(10);

            GUILayout.Label("Json Output Path", EditorStyles.boldLabel);

            string outputFileName = Path.GetFileNameWithoutExtension(excelFilePath);
            if (GUILayout.Button("Select Json Output Path"))
            {
                string path = EditorUtility.SaveFilePanel("Select Json Output Path", "", outputFileName, "json");
                if (!string.IsNullOrEmpty(path))
                {
                    jsonOutputPath = path;
                }
            }


            EditorGUILayout.TextField("Json File Path", jsonOutputPath);

            GUILayout.Space(10);

            if (GUILayout.Button("Convert Excel To Json"))
            {

                // Create ExcelToJson Class Instance and Call Convert Method 
                /*
                ExcelConverter converter = new ExcelConverter
                {
                    excelFilePath = excelFilePath,
                    jsonOutputPath = jsonOutputPath
                };
                */

                //this.excelFilePath = excelFilePath;
                //this.jsonOutputPath = jsonOutputPath;
                ConvertExcelToJson(sheetNum);
                // Call Convert Method 
                //converter.ConvertExcelToJson(sheetNum);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ExcelConverter OnGUI Error: {e.Message}");
        }
    }

    private void GUILoadBtn()
    {
        GUILayoutOption[] spriteOptions = new[] {
                GUILayout.Width (200),
                GUILayout.Height (40)
            };

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("레벨 로드", spriteOptions))
        {
           
        }

        GUILayout.EndHorizontal();
    }

    
    public void ConvertExcelToJson(int sheetNum)
    {
        //string excelFilePath = "E:\\projects\\ExcelToJson";// EditorUtility.OpenFilePanel("Select Excel File", "", "xlsx,xls");
        //string jsonOutputPath = "E:\\projects\\ExcelToJson\\output.json"; // EditorUtility.SaveFilePanel("Save Json File", "", "output.json", "json");
        if (string.IsNullOrEmpty(excelFilePath) || string.IsNullOrEmpty(jsonOutputPath))
        {
            Debug.LogError("Excel Path, Json Path is NULL");
            return;
        }

        // Open Excel File
        using (FileStream stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
        {
            IWorkbook workbook = new XSSFWorkbook(stream); //For .xlsx

            ISheet sheet;
            try
            {
                sheet = workbook.GetSheetAt(sheetNum); // Select Sheet
            }
            catch
            {
                Debug.LogError("Invalid Sheet Num");
                return;
            }

            // List for data from converted json
            var rowsData = new List<Dictionary<string, object>>();
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            // Extract Data looping every row in sheet
            for (int i = 1; i <= sheet.LastRowNum; i++) // 0 is Header
            {
                IRow row = sheet.GetRow(i);
                var rowData = new Dictionary<string, object>();

                for (int j = 0; j < cellCount; j++)
                {
                    string columnName = headerRow.GetCell(j).ToString(); //Get ColumnName from Header
                    ICell cell = row.GetCell(j);
                    rowData[columnName] = GetValueFromCell(cell); //Get Cell Value
                }

                rowsData.Add(rowData); // Add Row data to List
            }

            //save Data after convert Json
            string json = JsonConvert.SerializeObject(rowsData, Formatting.Indented);
            File.WriteAllText(jsonOutputPath, json);

            Debug.Log($"Convert Excel To Json : {jsonOutputPath}");
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
                return cell.NumericCellValue;
            case CellType.Boolean:
                return cell.BooleanCellValue;
            case CellType.Formula:
                return cell.CellFormula; // or evaluate the formula
            default:
                return cell.ToString();
        }
    }
}


