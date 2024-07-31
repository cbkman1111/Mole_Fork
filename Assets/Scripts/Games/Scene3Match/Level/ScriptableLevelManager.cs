using System;
using System.IO;
using UnityEngine;
using UnityEditor;


namespace Match3
{
    public static class ScriptableLevelManager
    {
        #if UNITY_EDITOR
        public static void CreateFileLevel(int level, Level _levelData)
        {
            //var path = "Assets/SweetSugar/Resources/Levels/";
            var path = "Assets/Scenes/Scene3Match/Resources/Level/";
            if (Resources.Load("Level/Level_" + level))
            {
                SaveLevel(path, level, _levelData);
            }
            else
            {
                /*
                string fileName = "Level_" + level;
                var newLevelData = ScriptableObjectUtility.CreateAsset<LevelContainer>(path, fileName);
                newLevelData.SetData(_levelData.DeepCopy(level));
                EditorUtility.SetDirty(newLevelData);
                AssetDatabase.SaveAssets();
                */
            }
        }

        public static void SaveLevel(string path, int level, Level _levelData)
        {
            var levelScriptable = Resources.Load("Level/Level_" + level) as LevelContainer;
            if (levelScriptable != null)
            {
                levelScriptable.SetData(_levelData);
                EditorUtility.SetDirty(levelScriptable);
            }

            AssetDatabase.SaveAssets();
        }
        #endif

        public static Level LoadLevel(int level)
        {
            var levelScriptable = Resources.Load("Level/Level_" + level) as LevelContainer;
            Level levelData = null;

            if(levelScriptable)
            {
                levelData = levelScriptable.level;
            }
            else
            {
                //var levelScriptables = Resources.Load("Level/LevelScriptable") as LevelScriptable;
                //var ld = levelScriptables.levels.TryGetElement(level - 1, null);
                //levelData = ld.DeepCopy(level);
            }

            return levelData;
        }
    }
}