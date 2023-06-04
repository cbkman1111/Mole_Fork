using System;
using Malee;
using Malee.Editor;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.Localization;
using SweetSugar.Scripts.LinqConstructor.AnyFieldInspector.Editor;
using SweetSugar.Scripts.TargetScripts.TargetEditor;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEditor;
using UnityEngine;
namespace SweetSugar.Scripts.Editor
{
public class TargetEditor : EditorWindow
{
    private static TargetEditor window;
    TargetEditorScriptable targetObject;
    private SerializedObject so;
    private ReorderableList list;
    private Vector2 scrollPos;

    [MenuItem("Sweet Sugar/Settings/Target editor")]
    public static void Init()
    {

        // Get existing open window or if none, make a new one:
        window = (TargetEditor)GetWindow(typeof(TargetEditor),false, "Target editor");
        window.Show();
    }
    void OnEnable()
    {
        targetObject = AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Resources/Levels/TargetEditorScriptable.asset", typeof(TargetEditorScriptable)) as TargetEditorScriptable;
        so = new SerializedObject(targetObject);
        list = new ReorderableList( so.FindProperty("targets"));
    }

    void OnGUI()
    {
        so.Update();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginVertical();
        scrollPos =
            EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));
        GUILayout.Space(10);
        list.DoLayoutList();
//        GuiList.Show(targetObject.targets, () => {        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/SweetSugar/Resources/Levels/TargetEditorScriptable.asset");});

        GUILayout.Space(30);
        if (GUILayout.Button("Save"))
        {
            SaveSettings();
        }
        GUILayout.Space(20);
        if (GUILayout.Button("Update all level targets"))
        {
            if (EditorUtility.DisplayDialog("Warning!", "Replace all level targets?", "Ok", "Cancel"))
            {
                var targetsEditor = AssetDatabase.LoadAssetAtPath<TargetEditorScriptable>("Assets/SweetSugar/Resources/Levels/TargetEditorScriptable.asset");

                var levels = Resources.LoadAll<LevelContainer>("Levels");
                for (int i = 1; i <= levels.Length; i++)
                {
                    LevelData levelData = null;
                    levelData = LoadingManager.LoadlLevel(i, levelData);
                    var targetLevel = OpenTarget(i);
                    targetLevel.LoadFromLevel(levelData,targetsEditor);
                    Debug.Log("level " + i + " updated");
                }
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        so.ApplyModifiedProperties();
        // if (EditorGUI.EndChangeCheck()) SaveSettings();
        


    }
    
    private TargetLevel OpenTarget(int levelNumber)
    {
        var asset = Resources.Load<TargetLevel>("Levels/Targets/TargetLevel" + levelNumber);
        if (asset == null)
        {
            asset = CreateInstance<TargetLevel>();
            asset.name = "Level" + levelNumber;
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/SweetSugar/Resources/Levels/Targets/TargetLevel" + levelNumber + ".asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        return asset;
    }

        void SaveSettings()
        {
            EditorUtility.SetDirty(targetObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}



