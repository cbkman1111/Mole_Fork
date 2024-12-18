using Common.UIObject;
using System.IO;
using UI.Popup;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;


[CustomEditor(typeof(UIRoot))]
public class UIManagerTool : Editor
{
    UIRoot rootObject = null;

    private void OnEnable()
    {
        rootObject = target as UIRoot;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(50); // 공백 추가
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // 위쪽 여백 추가
        GUILayout.BeginVertical();

        string msg = "Do Something";
        if (GUILayout.Button(msg, GUILayout.Width(200), GUILayout.Height(100)))
        {
            //SetAllPath();
        }

        GUI.enabled = false; // 텍스트 비활성.
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace(); // 위쪽 여백 추가
        GUILayout.EndHorizontal();
    }

    public string GetPrefabPath(string name)
    {
        string[] guids = AssetDatabase.FindAssets($"{name} t:prefab");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return path;
        }

        return string.Empty;
    }

    private void SetAllPath()
    {
        string name = typeof(UIPopupNormal).Name;
        string path = GetPrefabPath(name);
        
        /*
        string[] allPath = Directory.GetFiles("Assets/Resources/UI", "*.prefab", SearchOption.AllDirectories);
        foreach (string path in allPath)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                var splits = path.Split("/");
                string key = prefab.name;
                //string path =                 //rootObject.AddPath(prefab.name, "");
            }
        }
        */
    }
}
