using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SweetSugar.Scripts.TargetScripts.TargetEditor
{
    [CustomPropertyDrawer(typeof(SpriteObject))]
    public class SpriteObjectDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            Rect r1 = position;
            r1.width = 20;

            Rect r2 = position;
            r2.width = 50;
            r2.xMin = r1.xMax + 10;
            Rect r3 = position;
            r3.xMin = r2.xMax + 50;
            EditorGUI.BeginProperty(position, label, property);
            var findPropertyRelative = property.FindPropertyRelative("icon");
            var sprite = (Sprite) findPropertyRelative.objectReferenceValue;
            EditorGUI.LabelField(r1, new GUIContent(sprite?.texture));
            var obj = property.serializedObject;
            if (obj.targetObject.name.Contains("TargetEditorScriptable"))
                EditorGUI.PropertyField(r2, property.FindPropertyRelative("uiSprite"), GUIContent.none);
            else r3.xMin -= 50;
            EditorGUI.PropertyField(r3, findPropertyRelative, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}