using System.Linq;
using SweetSugar.Scripts.System;
using SweetSugar.Scripts.TargetScripts.TargetEditor;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEditor;
using UnityEngine;

namespace SweetSugar.Scripts.Items.Editor
{
    [CustomPropertyDrawer(typeof(SpawnAmountObj))]
    public class SpawnAmountDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var count = property.FindPropertyRelative("SpawnAmount");
            Item item = PropertyUtils.GetParent(property) as Item;
            Rect r1 = position;
            r1.height = EditorGUIUtility.singleLineHeight;
            if (item.currentType == ItemsTypes.INGREDIENT)
            {
                EditorGUI.LabelField(r1, "How many ingredients can be spawned at the same time");
                r1.y+= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(r1, count, new GUIContent("Spawn amount"));
            }
            else if (item.currentType == ItemsTypes.NONE)
            {
                EditorGUI.LabelField(r1, "How many time bonuses can be spawned at the same time");
                r1.y+= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(r1, count, new GUIContent("Spawn amount"));
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
        }
    }
}