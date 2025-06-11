using System.IO;
using UnityEditor;
using UnityEngine;

namespace ExcelConverter.Editor
{
    /// <summary>
    /// Excel to Json Converter Editor Window
    /// </summary>
    public partial class ExcelConverter : EditorWindow
    {
        void OnGUI()
        {
            try
            {
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

                //string outputFileName = Path.GetFileNameWithoutExtension(excelFilePath);
                //string outputfolder = EditorUtility.OpenFolderPanel("Select Json Output Folder", "", "");
                if (GUILayout.Button("Select Json Output Path"))
                {
                    jsonOutputPath = EditorUtility.OpenFolderPanel("Select Json Output Folder", "", "");
                    /*
                    string path = EditorUtility.SaveFilePanel("Select Json Output Path", "", outputfolder, "json");
                    if (!string.IsNullOrEmpty(path))
                    {
                        jsonOutputPath = path;
                    }
                    */
                }

                EditorGUILayout.TextField("Json File Path", jsonOutputPath);

                GUILayout.Space(10);

                if (GUILayout.Button("Convert Excel To Json"))
                {
                    ConvertExcelToJson(sheetNum);
                }

                GUILayout.Space(10);

                GUILayout.Label("Json to C# class", EditorStyles.boldLabel);

                if (GUILayout.Button("Convert Json To c#"))
                {
                    ConvertCSharpClass();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ExcelConverter OnGUI Error: {e.Message}");
            }
        }
    }
}