using Scenes;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace UI.Extention
{
    /// <summary>
    /// 유니티 버튼
    /// </summary>
    [CustomEditor(typeof(ButtonExtention))]
    public class ButtonExtentionEditor : UnityEditor.UI.ButtonEditor
    {
        private ButtonExtention component = null;

        /// <summary>
        /// 인스펙터 UI 생성.
        /// </summary>
        public override void OnInspectorGUI()
        {
            component = (ButtonExtention)target;
            
            base.OnInspectorGUI();

            EditorGUILayout.BeginVertical();

            GUILine(1);
            GUILayout.Label("버튼 사운드", EditorStyles.boldLabel);
            component.ClickSound = EditorGUILayout.IntField("Click Sound", component.ClickSound);

            /*
            if (GUILayout.Button("테스트", GUILayout.Width(200), GUILayout.Height(20)))
            {
                component.PointerDown();
            }
            */


            EditorGUILayout.EndVertical();
        }

        void GUILine(int lineHeight = 1)
        {
            EditorGUILayout.Space();
            Rect rect = EditorGUILayout.GetControlRect(false, lineHeight);
            rect.height = lineHeight;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            EditorGUILayout.Space();
        }
    }
}
