using System.Collections;
using UnityEditor;

namespace SweetSugar.Scripts.System
{
    public class DefineSymbolsUtils
    {
        #if UNITY_EDITOR
        public static void SwichSymbol(string symbol)
        {
            BuildTargetGroup[] _buildTargets = GetBuildTargets();
            foreach (BuildTargetGroup _target in _buildTargets)
            {
                var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(_target);
                if (!defines.Contains(symbol))
                    defines = defines + "; " + symbol;
                else
                    defines.Replace(symbol, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(_target, defines);
            }
        }

        public static void AddSymbol(string symbol)
        {
            BuildTargetGroup[] _buildTargets = GetBuildTargets();
            foreach (BuildTargetGroup _target in _buildTargets)
            {
                var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(_target);
                AddDefine(symbol, _target, ref defines);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(_target, defines);
            }
        }

        public static void DeleteSymbol(string symbol)
        {
            BuildTargetGroup[] _buildTargets = GetBuildTargets();
            foreach (BuildTargetGroup _target in _buildTargets)
            {
                var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(_target);
                DeleteDefine(symbol, _target, ref defines);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(_target, defines);
            }
        }

        private static void DeleteDefine(string symbol, BuildTargetGroup buildTargetGroup, ref string defines)
        {
            defines = defines.Replace(symbol, "");
        }

        private static BuildTargetGroup[] GetBuildTargets()
        {
            ArrayList _targetGroupList = new ArrayList();
            _targetGroupList.Add(BuildTargetGroup.Standalone);
            _targetGroupList.Add(BuildTargetGroup.Android);
            _targetGroupList.Add(BuildTargetGroup.iOS);
            _targetGroupList.Add(BuildTargetGroup.WSA);
            return (BuildTargetGroup[]) _targetGroupList.ToArray(typeof(BuildTargetGroup));
        }

        private static void AddDefine(string symbols, BuildTargetGroup _target, ref string defines)
        {
            if (!defines.Contains(symbols))
            {
                defines = defines + "; " + symbols;
            }
        }
        #endif
    }
}