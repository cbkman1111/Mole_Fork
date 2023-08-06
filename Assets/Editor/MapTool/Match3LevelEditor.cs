using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelEditor : EditorWindow
{
    private const string MenuNameRoot = "매치3/스테이지 에디터";
    private const string MenuNameOpen = MenuNameRoot + "/열기";
    private const string MenuNameLoad = MenuNameRoot + "/불러오기 &#l";
    private const string MenuNameSave = MenuNameRoot + "/저장하기 &#s";

    private string helloWorld;
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;
    public string[] options = new string[] { "Cube", "Sphere", "Plane" };
    public int index = 0;
    public int tabIndex = 0;
    public string[] tabSubject = new string[] { "layer_1", "layer_2", "layer_3", "layer_4", "layer_5" };
    // 매뉴에 경로를 지정 한다.
    [MenuItem(MenuNameOpen)]
    private static void Init()
    {
        var window = GetWindow(typeof(LevelEditor));
        window.titleContent = new GUIContent("스테이지 에디터");
    }

    [MenuItem(MenuNameLoad)]
    public static void DoLoadStage()
    {
        if (!HasOpenInstances<LevelEditor>()) return;
        var window = (LevelEditor)GetWindow(typeof(LevelEditor));
        //window.PuzzleEdit.LoadStage();
    }

    [MenuItem(MenuNameSave)]
    public static void DoSaveStage()
    {
        if (!HasOpenInstances<LevelEditor>()) return;
        var window = (LevelEditor)GetWindow(typeof(LevelEditor));
        //window.PuzzleEdit.SaveStage();
    }

    void OnGUI()
    {
        // 텍스트 필드.
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        helloWorld = EditorGUILayout.TextField("Text Field", helloWorld);

        // 그룹
        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();

        // 버튼.
        Rect r = (Rect)EditorGUILayout.BeginVertical("Button");
        if (GUI.Button(r, GUIContent.none))
        {
            Debug.Log("Go here");
        }

        GUILayout.Label("I'm inside the button");
        GUILayout.Label("So am I");
        EditorGUILayout.EndVertical();

        // 
        index = EditorGUILayout.Popup(selectedIndex: index, displayedOptions: options);
        if (GUILayout.Button("Create"))
        {
            // Do Somthing.
        }

        tabIndex = GUILayout.Toolbar(tabIndex, tabSubject);
        GUILayout.Label($"layer_{tabIndex}");
    }
}
