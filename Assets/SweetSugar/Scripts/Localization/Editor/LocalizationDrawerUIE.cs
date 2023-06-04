using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SweetSugar.Scripts.Localization.Editor
{
    [CustomPropertyDrawer(typeof(LocalizationIndex))]
    public class LocalizationDrawerUIE : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            Rect r1 = position;
            r1.width = 1;

            Rect r2 = position;
            r2.xMin = r1.xMax + 1;
            r2.width = 50;

            EditorGUI.BeginProperty(position, label, property);
            // EditorGUI.PropertyField(r1, property.FindPropertyRelative("text"),new GUIContent("","Default text"));
            EditorGUI.PropertyField(r2, property.FindPropertyRelative("index"),new GUIContent("","Localization line index"));
            r2.x += 50;
            r2.width = 300;
            UnityEditor.EditorGUI.LabelField(r2, "Change text here: Resources/Localization/");
            EditorGUI.EndProperty();
        }
    }
}