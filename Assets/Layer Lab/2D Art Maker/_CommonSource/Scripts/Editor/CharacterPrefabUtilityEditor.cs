using UnityEditor;
using UnityEngine;

namespace LayerLab.ArtMaker
{
#if UNITY_EDITOR
    [CustomEditor(typeof(CharacterPrefabUtility))]
    public class CharacterPrefabUtilityEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
           
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Create Prefab", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Save the current character state as a prefab and generate a thumbnail image.\n" +
                                    "- Transparent PNG thumbnail will be generated\n" +
                                    "- Thumbnail size and camera settings can be adjusted", MessageType.Info);
           
            var script = (CharacterPrefabUtility)target;
           
            if (GUILayout.Button("Create Character Prefab (with Thumbnail)", GUILayout.Height(30)))
            {
                if (Application.isPlaying && Player.Instance != null)
                {
                    script.CreateCharacterPrefab();
                }
                else
                {
                    EditorUtility.DisplayDialog("Notice", "Prefab can only be created in Play Mode.", "OK");
                }
            }
           
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("How to Use:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("1. Enter Play Mode", EditorStyles.label);
            EditorGUILayout.LabelField("2. Customize your character", EditorStyles.label);
            EditorGUILayout.LabelField("3. Click 'Create Character Prefab' button", EditorStyles.label);
            EditorGUILayout.LabelField("4. Check results in Assets/CharacterPrefabs/ folder", EditorStyles.label);
        }
    }
#endif
}