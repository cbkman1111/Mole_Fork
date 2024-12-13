using Scenes;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace UI.Extention
{
    /// <summary>
    /// ����Ƽ ��ư
    /// </summary>
    [CustomEditor(typeof(ButtonExtention))]
    public class ButtonExtentionEditor : UnityEditor.UI.ButtonEditor
    {
        private ButtonExtention component = null;

        /// <summary>
        /// �ν����� UI ����.
        /// </summary>
        public override void OnInspectorGUI()
        {
            component = (ButtonExtention)target;
            
            base.OnInspectorGUI();

            EditorGUILayout.BeginVertical();

            GUILine(1);
            GUILayout.Label("��ư ����", EditorStyles.boldLabel);
            component.ClickSound = EditorGUILayout.IntField("Click Sound", component.ClickSound);

            /*
            if (GUILayout.Button("�׽�Ʈ", GUILayout.Width(200), GUILayout.Height(20)))
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
