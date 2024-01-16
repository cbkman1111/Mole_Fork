using Common.Global;
using Scenes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tetris
{
    public partial class TetrisShape : MonoBehaviour
    {
        public enum Shapes
        {
            Square = 0,
            T,
            L,
            J,
            Z,
            S,
            I,

            Max
        };

        public SpriteRenderer square = null;
        public Vector2 Coordinate;
        public int Rotate = 0;

        private SpriteRenderer[,] sprites = new SpriteRenderer[Width, Height];
        public int[,,] Arr { get; set; }

        public bool Init(Shapes shape, Vector2 coor)
        {
            switch (shape)
            {
                case Shapes.Square:
                    Arr = arraySquare;
                    break;
                case Shapes.T:
                    Arr = arrayT;
                    break;
                case Shapes.L:
                    Arr = arrayL;
                    break;
                case Shapes.J:
                    Arr = arrayJ;
                    break;
                case Shapes.Z:
                    Arr = arrayZ;
                    break;
                case Shapes.S:
                    Arr = arrayS;
                    break;
                case Shapes.I:
                    Arr = arrayI;
                    break;
            }

            Coordinate = coor;
            Rotate = 0;

            for (int col = 0; col < Width; col++)
            {
                for (int row = 0; row < Height; row++)
                {
                    var obj = Instantiate<SpriteRenderer>(square, transform);
                    if (obj == null)
                        break;

                    var posX = col * 1;
                    //var posY = (Height - 1) - row * 1;
                    var posY = row * 1;
                    obj.transform.localPosition = new Vector3(posX, posY);
                    obj.sortingOrder = 10;

                    if (Arr[Rotate, row, col] == 1)
                        obj.color = Color.red;
                    else if (Arr[Rotate, row, col] == 2)
                        obj.color = Color.blue;
                    else
                    {
                        var color = obj.color;
                        color = Color.magenta;
                        color.a = 0.5f;
                        obj.color = color;
                        obj.sortingOrder = 0;
                    }

                    sprites[col, row] = obj;
                    obj.name = $"{row},{col}";
                }
            }

            return true;
        }

        public void RotateShape()
        {
            Rotate = (Rotate + 1) % 4;

            for (int col = 0; col < Width; col++)
            {
                for (int row = 0; row < Height; row++)
                {
                    var obj = sprites[col, row];
                    if (obj == null)
                        break;

                    obj.sortingOrder = 10;
                    if (Arr[Rotate, row, col] == 1)
                        obj.color = Color.red;
                    else if (Arr[Rotate, row, col] == 2)
                        obj.color = Color.blue;
                    else
                    {
                        var color = obj.color;
                        color = Color.magenta;
                        color.a = 0.5f;
                        obj.color = color;
                        obj.sortingOrder = 0;
                    }

                    obj.name = $"{row},{col}";
                }
            }
        }

        public int[,] GetArray(int rotate)
        {
            int[,] array = new int[Width, Height];
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    array[row, col] = Arr[rotate, row, col];
                }
            }

            return array;
        }
    }

}