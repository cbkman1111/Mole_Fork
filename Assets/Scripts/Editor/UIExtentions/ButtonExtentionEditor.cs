using Scenes;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace UI.Extention
{
    /// <summary>
    /// ����Ƽ ��ư
    /// true ��� ���� Ŭ������ Ŀ���� ó�� �ǵ��� �ϴ� �ɼ�.
    /// </summary>
    [CustomEditor(typeof(ButtonExtention), true)]
    public class ButtonExtentionEditor : UnityEditor.UI.ButtonEditor
    {
        private ButtonExtention component = null;

        /// <summary>
        /// �ν����� UI ����.
        /// </summary>
        public override void OnInspectorGUI()
        {
            component = (ButtonExtention)target;
            
            //base.OnInspectorGUI();

            EditorGUILayout.BeginVertical();
            
            GUILayout.Label("Ŀ���� ��ư.", EditorStyles.boldLabel);

            GUILine(1);
            GUILayout.Label("��ư ����", EditorStyles.boldLabel);
            component.ClickSound = EditorGUILayout.IntField("���� ID", component.ClickSound);

            if (GUILayout.Button("Ŭ�� �׽�Ʈ", GUILayout.Width(200), GUILayout.Height(20)))
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
