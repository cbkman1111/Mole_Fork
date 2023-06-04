using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SweetSugar.Scripts.TargetScripts.TargetSystem
{
    /// <summary>
    /// Target editor
    /// </summary>
    public class TargetEditorScriptable : ScriptableObject
    {
        public List<TargetContainer> targets = new List<TargetContainer>();

        public TargetContainer GetTargetbyName(string getTargetsName) => targets.First(i => i.name == getTargetsName);
    }
}