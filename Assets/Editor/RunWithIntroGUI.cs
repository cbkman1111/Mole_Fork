using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class RunWithIntroGUI
{
    static RunWithIntroGUI()
    {
        UnityToolbarExtender.ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
    }

    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();

        var prevGUIEnabled = GUI.enabled;
        GUI.enabled = (false == Application.isPlaying);
        if (GUILayout.Button(new GUIContent("타이틀 시작", "타이틀 씬으로 부터 시작")))
        {
            //PlayerPrefs.SetString("StartScene", "Intro");
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            EditorSceneManager.OpenScene("Assets/Scenes/SceneIntro.unity");
            EditorSettings.enterPlayModeOptionsEnabled = true;
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }

        GUI.enabled = prevGUIEnabled;
    }
}
