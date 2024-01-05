using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum PuzzleSquareTypes
{
    NONE = 0,
    EmptySquare,
    SugarSquare,
    WireBlock,
    SolidBlock,
    ThrivingBlock,
    JellyBlock,
    SpiralBlock,
    UndestrBlock,
    BigBlock,
    Marshmello,
    ChocoSpread/*,
        Cake*/
}

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
    private string[] taps = { "Blocks", "Items", "Directions", "Teleports", "Spawners" };

    // 매뉴에 경로를 지정 한다.
    [MenuItem(MenuNameOpen)]
    private static void Open()
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
        GUILayout.Label("멥툴", EditorStyles.boldLabel);
        //helloWorld = EditorGUILayout.TextField("Text Field", helloWorld);

        GUILevelSize();
        GUICreateLoadBtns();
        GUILayerTabs();
        
        /*
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

        tabIndex = GUILayout.Toolbar(tabIndex, taps);
        GUILayout.Label($"layer_{tabIndex}");

        GUILevelSize();
        GUILayout.Space(10);

        GUILayoutOption[] spriteOptions = new[] {
            GUILayout.Width (80),
            GUILayout.Height (80)
        };

        EditorGUILayout.ObjectField(null, typeof(Texture2D), false, spriteOptions);
        EditorGUILayout.Space();
        EditorGUILayout.ObjectField(null, typeof(Sprite), false, spriteOptions);
        */
    }
    private void GUIBlocks()
    {
        GUILayout.BeginHorizontal();
        {
            //GUILayout.Space(30);
            GUILayout.BeginVertical();
            {
                GUILayout.Label("Tools:", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                {
                    /*
                    UnityEngine.GUI.color = new Color(1, 1, 1, 1f);
                    foreach (SquareTypes squareTypeItem in _squareTypeItems)
                    {
                        if (squareTypeItem == SquareTypes.NONE) 
                            continue;

                        var squareTexture = Square.GetSquareTexture(squareTypeItem);
                        if (squareType == squareTypeItem)
                            UnityEngine.GUI.backgroundColor = Color.gray;

                        if (squareTexture.Texture2D != null && GUILayout.Button(
                                new GUIContent(squareTexture.Texture2D, squareTypeItem.ToString()),
                                GUILayout.Width(50), GUILayout.Height(50)))
                        {
                            squareType = squareTypeItem;
                            deleteBlock = false;
                            separateBarBrush = false;
                        }

                        UnityEngine.GUI.backgroundColor = Color.white;
                    }
                    */

                    UnityEngine.GUI.color = new Color(1, 1, 1, 1f);
                }

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                {
                    /*
                    if (separateBarBrush)
                        UnityEngine.GUI.backgroundColor = Color.gray;
                    if (GUILayout.Button(
                        new GUIContent(separateBar, "Separate bar prevent items from falling or moving through"),
                        GUILayout.Width(50),
                        GUILayout.Height(50)))
                    {
                        squareType = SquareTypes.NONE;
                        separateBarBrush = !separateBarBrush;
                    }

                    UnityEngine.GUI.backgroundColor = Color.white;

                    if (GUILayout.Button(new GUIContent("Clear", "clear all field"), GUILayout.Width(50),
                        GUILayout.Height(50)))
                    {
                        ClearLevel();
                        // SaveLevel();
                    }

                    if (GUILayout.Button(new GUIContent("X", "Clear block"), GUILayout.Width(50),
                        GUILayout.Height(50)))
                    {
                        squareType = SquareTypes.NONE;
                        separateBarBrush = false;
                    }

                    if (GUILayout.Button(
                        new GUIContent("Fill+", "Fill with selected block, second click change or clear filling"),
                        GUILayout.Width(50),
                        GUILayout.Height(50)))
                    {
                        FillLevel();
                    }

                    if (deleteBlock)
                        UnityEngine.GUI.backgroundColor = Color.gray;
                    if (GUILayout.Button(new GUIContent("-1", "to Delete a block from layer"), GUILayout.Width(50),
                        GUILayout.Height(50)))
                    {
                        deleteBlock = !deleteBlock;
                        separateBarBrush = false;
                    }
                    */
                    UnityEngine.GUI.backgroundColor = Color.white;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }

    private void GUICreateLoadBtns()
    {
        GUILayoutOption[] spriteOptions = new[] {
            GUILayout.Width (200),
            GUILayout.Height (40)
        };
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Map", spriteOptions))
        {
            // Do Somthing.
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Create NewMap", spriteOptions))
        {
            // Do Somthing.
        }
        GUILayout.EndHorizontal();
    }

    private void GUILayerTabs()
    {
        GUILayout.Label("Layers", EditorStyles.boldLabel);
        tabIndex = GUILayout.Toolbar(tabIndex, taps);
        //GUILayout.Label($"layer_{tabIndex}");

        GUILayout.Space(10);
    }

    private void GUILevelSize()
    {
        const int maxRows = 10;
        const int maxCols = 10;
        /*
        int oldValue = levelData.GetField(subLevelNumber - 1).maxRows +
                       levelData.GetField(subLevelNumber - 1).maxCols;
        */

        GUILayout.BeginHorizontal();
        GUILayout.Label("Level", GUILayout.Width(50));
        GUILayout.Space(10);

        EditorGUILayout.IntField("", 6, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Columns", GUILayout.Width(80));
        GUILayout.Space(10);

        EditorGUILayout.IntField("", 12, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        /*
levelData.GetField(subLevelNumber - 1).maxCols = EditorGUILayout.IntField("",
    levelData.GetField(subLevelNumber - 1).maxCols, GUILayout.Width(50));
*/

        GUILayout.BeginHorizontal();
        GUILayout.Label("Rows", GUILayout.Width(80));
        GUILayout.Space(10);

        EditorGUILayout.IntField("", maxRows, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        //
        GUILayout.Space(10);
    }
    }
