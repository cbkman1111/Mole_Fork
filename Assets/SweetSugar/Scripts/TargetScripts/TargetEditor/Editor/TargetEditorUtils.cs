using System.Linq;
using SweetSugar.Scripts.System;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEditor;

namespace SweetSugar.Scripts.TargetScripts.TargetEditor.Editor
{
    public static class TargetEditorUtils
    {
        private static TargetEditorScriptable target;

        public static TargetEditorScriptable TargetEditorScriptable
        {
            get
            {
                if (target == null)
                {
                    target = AssetDatabase.LoadAssetAtPath<TargetEditorScriptable>("Assets/SweetSugar/Resources/Levels/TargetEditorScriptable.asset");
                }

                return target;
            }
        }

        public static TargetContainer GetTargetContainer(SerializedProperty property)
        {
            var propertyParent = PropertyUtils.GetParent(property) as TargetObject;
            return TargetEditorScriptable.GetTargetbyName(propertyParent.targetType.GetTarget().name);
        }
    }
}