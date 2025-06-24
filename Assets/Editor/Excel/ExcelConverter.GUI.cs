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
        /// <summary>
        /// GUI Draw
        /// </summary>
        void OnGUI()
        {
            try
            {
                DrawTitle();

                DrawFilePath();

                DrawExportButton();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ExcelConverter OnGUI Error: {e.Message}");
            }
        }

        /// <summary>
        /// 제목
        /// </summary>
        private void DrawTitle()
        {
            GUILayout.Label("Select Excel File Path", EditorStyles.boldLabel);
        }

        /// <summary>
        /// 입력 파일 & 출력 경로
        /// </summary>
        private void DrawFilePath()
        {
            if (GUILayout.Button("Select Excel File"))
            {
                string path = EditorUtility.OpenFilePanel("Select Excel File", ExcelInputPath, "xlsx");
                ExcelFilePath = path;
                ExcelFile = Path.GetFileName(path);
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Input", ExcelFile);
            EditorGUILayout.TextField("OutPut", JsonOutputPath);

            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Json 익스포트
        /// </summary>
        private void DrawExportButton()
        {
            GUILayout.Space(10);

            if (GUILayout.Button("Convert Excel To Json"))
            {
                ConvertExcelToJson();
            }
        }
    }
}