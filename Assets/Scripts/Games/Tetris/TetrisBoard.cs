using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static tetris.TetrisShape;

namespace tetris
{
    public class TetrisBoard : MonoBehaviour
    {
        [SerializeField]
        private int width = 10; // 좌우 하나씩더
        [SerializeField]
        private int height = 24; // 위아래 4개씩더
        [SerializeField]
        private TetrisTile tilePrefab = null;
        [SerializeField]
        private TetrisShape shapePrefab = null;

        private TetrisTile[,] boardArray = null;

        /// <summary>
        /// 
        /// </summary>
        public void InitBoard()
        {
            boardArray = new TetrisTile[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var obj = Instantiate<TetrisTile>(tilePrefab, transform);
                    if (obj == null)
                        continue;

                    obj.transform.position = GetPosition(x, y);
                    obj.name = $"{x},{y}";
                    obj.Coordnate = new Vector2(x, y);
                    obj.SetValue(0);
                    if (y >= 20)
                    {
                        obj.SetValue(-1);
                    }
                    
                    boardArray[x, y] = obj;
                }
            }
        }

        public Vector3 GetPosition(float x, float y)
        {
            const float tileSize = 1.0f;
            float tileHarfSize = tileSize * 0.5f;
            float total_width = tileSize * width;
            float total_height = tileSize * height;

            float xpos = x * tileSize - (total_width * 0.5f) + tileHarfSize;
            float ypos = y * tileSize - (total_height * 0.5f) + tileHarfSize;

            return new Vector3(xpos, ypos);
        }

        public void OnInsert(TetrisShape insertShape)
        {
            var arry = insertShape.Arr;
            var rotate = insertShape.Rotate;
            var coord = insertShape.Coordinate;

            if (Possiable(insertShape) == false)
                return;
             
            // Shape 모양있는 부분이 바깥으로 나가는지 확인.
            for (int row = 0; row < TetrisShape.Height; row++)
            {
                for (int col = 0; col < TetrisShape.Width; col++)
                {
                    if (arry[rotate, row, col] != 0)
                    {
                        int boardX = (int)coord.x + col;
                        int boardY = (int)coord.y + row;

                        if (boardX >= width || boardX < 0)
                            continue;

                        if (boardY >= height || boardY < 0)
                            continue;

                        var tile = boardArray[boardX, boardY];

                        if (boardY >= 20)
                        {
                            return;
                        }
                        
                        if (tile.Value == 0)
                        {
                            tile.SetValue(arry[rotate, row, col]);
                        }
                    }
                }
            }
        }

        public bool Possiable(TetrisShape insertShape)
        {
            var x = insertShape.Coordinate.x;
            var y = insertShape.Coordinate.y;
            var rotate = insertShape.Rotate;
            var array = insertShape.GetArray(insertShape.Rotate);

            return Possiable(array, rotate, (int)x, (int)y);
        }

        public bool Possiable(int[,] arr, int rotate, int x, int y)
        {
            // Shape 모양있는 부분이 바깥으로 나가는지 확인.
            for (int row = 0; row < TetrisShape.Height; row++)
            {
                for (int col = 0; col < TetrisShape.Width; col++)
                {
                    if (arr[row, col] != 0)
                    {
                        int boardX = (int)x + col;
                        int boardY = (int)y + row;

                        if (boardX >= width || boardX < 0)
                            return false;

                        if (boardY >= height || boardY < 0)
                            return false;

                        if (boardArray[boardX, boardY].Value > 0)
                            return false;
                    }
                }
            }

            return true;
        }
    }
}
