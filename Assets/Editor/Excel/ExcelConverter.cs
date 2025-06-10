using UnityEditor;
using UnityEngine;


public partial class ExcelConverter : EditorWindow
{
    private const string MenuNameRoot = "엑셀/엑셀 익스포터";
    private const string MenuNameOpen = MenuNameRoot + "/열기";

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
    }
}

