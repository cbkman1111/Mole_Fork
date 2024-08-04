using System;
using UnityEngine;

namespace Match3
{
    [Serializable]
    public class Level
    {
        public int Row = 5;
        public int Col = 5;
        public int Stage = 1;
        public int Layers = 1;

        public Level DeepCopy(Level level)
        {
            Row = level.Row;
            Col = level.Col;
            Stage = level.Stage;
            Layers = level.Layers;
            return this;
        }
    }

    [CreateAssetMenu(fileName = "LevelContainer", menuName = "LevelContainer", order = 1)]
    public class LevelContainer : ScriptableObject
    {
        public Level level = null;

        public void SetData(Level level)
        {
            this.level = new();
            this.level.DeepCopy(level);
        }
    }
}
