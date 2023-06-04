using System.Linq;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.System;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEditor;
using UnityEngine;

namespace SweetSugar.Scripts.TargetScripts.TargetEditor.Editor
{
    [CustomPropertyDrawer(typeof(Count))]
    public class CountDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var count = property.FindPropertyRelative("count");
            var targetContainer = TargetEditorUtils.GetTargetContainer(property);
            if (targetContainer != null && targetContainer.setCount == SetCount.Manually)
                EditorGUI.PropertyField(position, count);

            EditorGUI.EndProperty();
        }
    }
}