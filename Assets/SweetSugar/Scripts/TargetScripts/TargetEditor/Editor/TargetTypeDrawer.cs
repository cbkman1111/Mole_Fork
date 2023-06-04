using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using System.Linq;
using SweetSugar.Scripts.System;
using SweetSugar.Scripts.TargetScripts.TargetEditor;
using SweetSugar.Scripts.TargetScripts.TargetEditor.Editor;

namespace Sweet_sugar.Assets.SweetSugar.Scripts.TargetScripts.TargetEditor
{
    [CustomPropertyDrawer(typeof(TargetType))]
    public class TargetTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            var targetType = property.FindPropertyRelative("type");
            targetType.intValue = EditorGUI.Popup(position, targetType.intValue, GetTargetsNames());
            if (EditorGUI.EndChangeCheck())
            {
                var targetsEditor = TargetEditorUtils.TargetEditorScriptable;
                var targetObject = PropertyUtils.GetParent(property) as TargetObject;
                SpriteList sprites = targetsEditor.GetTargetbyName(GetTargetsNames()[targetType.intValue]).defaultSprites.FirstOrDefault()?.sprites.Copy();
                targetObject.sprites = sprites;
                var targetLevel = property.serializedObject.targetObject as TargetLevel;
                targetObject.targetType.type = targetType.intValue;
                targetLevel.saveData();
                property.serializedObject.Update();
            }
            EditorGUI.EndProperty();
        }

        public string[] GetTargetsNames()
        {
            return TargetEditorUtils.TargetEditorScriptable.targets.Select(i => i.name).ToArray();
        }
    }
}