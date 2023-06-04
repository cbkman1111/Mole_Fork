using System.Linq;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.System;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEditor;
using UnityEngine;

namespace SweetSugar.Scripts.TargetScripts.TargetEditor.Editor
{
    [CustomPropertyDrawer(typeof(BoolScoreShow))]
    public class ShowScoreDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var show = property.FindPropertyRelative("ShowTheScore");
            var targetContainer = TargetEditorUtils.GetTargetContainer(property);
            if (targetContainer != null && targetContainer.name=="Stars")
                EditorGUI.PropertyField(position, show);

            EditorGUI.EndProperty();
        }
    }
}