using Scenes;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace UI.Extention
{
    /// <summary>
    /// 유니티 버튼
    /// true 상속 받은 클래스도 커스텀 처리 되도록 하는 옵션.
    /// </summary>
    [CustomEditor(typeof(ButtonExtention), true)]
    public class ButtonExtentionEditor : UnityEditor.UI.ButtonEditor
    {
        private ButtonExtention component = null;

        /// <summary>
        /// 인스펙터 UI 생성.
        /// </summary>
        public override void OnInspectorGUI()
        {
            component = (ButtonExtention)target;
            
            //base.OnInspectorGUI();

            EditorGUILayout.BeginVertical();
            
            GUILayout.Label("커스텀 버튼.", EditorStyles.boldLabel);

            GUILine(1);
            GUILayout.Label("버튼 사운드", EditorStyles.boldLabel);
            component.ClickSound = EditorGUILayout.IntField("사운드 ID", component.ClickSound);

            if (GUILayout.Button("클릭 테스트", GUILayout.Width(200), GUILayout.Height(20)))
            {
                component.Click();
            }


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
