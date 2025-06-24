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
        private const string MenuNameRoot = "엑셀/엑셀 익스포터";
        private const string MenuNameOpen = MenuNameRoot + "/열기";

        private string ExcelFilePath = string.Empty; // Path to Excel File
        private string ExcelFile = string.Empty; // Path to Excel File
        private string ExcelInputPath = string.Empty; // Path to Save Json File
        private string JsonOutputPath = string.Empty; // Path to Save Json File

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
            var assetsPath = Application.dataPath;

            ExcelInputPath = $"{Directory.GetParent(assetsPath).FullName}/ExcelData";
            JsonOutputPath = $"{assetsPath}/Resources/TableData";
            ExcelFilePath = string.Empty;
        }

        /// <summary>
        /// 엑셀 파일을 읽어서 JSON 파일로 변환합니다.
        /// </summary>
        /// <param name="sheetNum"></param>
        public void ConvertExcelToJson()
        {
            if (string.IsNullOrEmpty(ExcelFilePath) || string.IsNullOrEmpty(JsonOutputPath))
            {
                Debug.LogError("Excel Path, Json Path is NULL");
                return;
            }

            Giant.Excel.Excel excel = new(ExcelFilePath, JsonOutputPath);
        }
    }
}
