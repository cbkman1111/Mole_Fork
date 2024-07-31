using SweetSugar.Scripts.Level;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Match3
{
    [Serializable]
    public class Level
    {
        public int Row = 0;
        public int Col = 0;
        public int Stage = 0;

        public Level DeepCopy(Level level)
        {
            Row = level.Row;
            Col = level.Col;
            Stage = level.Stage;

            return this;
        }
    }

    [CreateAssetMenu(fileName = "LevelContainer", menuName = "LevelContainer", order = 1)]
    public class LevelContainer : ScriptableObject
    {
        public Level level;

        public void SetData(Level level)
        {
            this.level = level;
        }
    }
}
