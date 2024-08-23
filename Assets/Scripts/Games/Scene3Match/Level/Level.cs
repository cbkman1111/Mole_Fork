using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Level;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

namespace Match3
{ 
    /// <summary>
    /// Äµµð.
    /// </summary>
    public enum CandyTypes
    {
        None = 0,

        Red,
        Yellow,
        Green,
        Purple,
        Sky,
        Orange,

        SpecialRed,
        SpecialYellow,
        SpecialGreen,
        SpecialPupple,
        SpecialBlue,
        SpecialOrange,

        Max,
    }

    /// <summary>
    /// ºí·°.
    /// </summary>
    public enum BlockTypes
    {
        None = 0,
        Empty,
        Sugar,

        Max,
    }

    public enum DirectionTypes
    {
        Down,
        Up,
        Left,
        Right,
        Max,
    }

    [Serializable]
    public class Level
    {
        public int Stage = 1;

        public int Row 
        { 
            get => field.Row;
            set => field.Row = value;
        }
    
        public int Col 
        {
            get => field.Col;
            set => field.Col = value;
        }

        public int Layers
        {
            get => field.Layers;
            set => field.Layers = value;
        }

        public FieldData field = new();

        public Level()
        {
            field = new();
        }

        public Level DeepCopy(Level dest)
        {
            Stage = dest.Stage;
            field.Col = dest.Col;
            field.Row = dest.Row;
            field.Layers = dest.Layers;

            foreach(var square in dest.field.Squares)
            {
                field.Squares[square.position.x + square.position.y * Col] = square;
            }
            return this;
        }
    }

    [Serializable]
    public class FieldData
    {
        public int Row = 5;
        public int Col = 5;
        public int Layers = 1;
        public SquareBlock[] Squares = new SquareBlock[81];

        public FieldData()
        {
            for (int i = 0; i < Squares.Length; i++)
            {
                Squares[i] = new();
                Squares[i].position = new Vector2Int(i % Col, i / Col);
                Squares[i].direction = new Vector2(0, -1);
            }
        }
    }

    [Serializable]
    public class SquareBlock
    {
        public List<BlockTypes> block;
        public CandyTypes candy;
        public Vector2 direction;
        public Vector2Int position;

        public SquareBlock()
        {
            block = new List<BlockTypes>();
            direction = new Vector2(0, -1);
            position = Vector2Int.zero;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "LevelContainer", menuName = "LevelContainer", order = 1)]
    public class LevelContainer : ScriptableObject
    {
        public Level level = null;
        public void SetData(Level dest)
        {
            level.DeepCopy(dest);
        }
    }
}
