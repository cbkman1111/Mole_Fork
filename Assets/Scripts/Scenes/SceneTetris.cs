using Common.Global;
using Common.Scene;
using System.Collections;
using System.Collections.Generic;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneTetris : SceneBase
    {
        public TetrisTile tilePrefab = null;
        public TetrisShape shapePrefab = null;

        public enum Shapes { 
            Square = 0,
            T,
            L,
            J,
            Z,
            S,
            I,

            Max
        };

        static public int width = 12; // 좌우 하나씩더
        static public int height = 28; // 위아래 4개씩더

        private TetrisTile[,] boardArray = new TetrisTile[width, height];
        public GameObject board = null;
        private TetrisShape currShape = null;
        
        public override bool Init(JSONObject param)
        {
            var menu = UIManager.Instance.OpenMenu<UIMenuTetris>("UIMenuTetris");
            if (menu != null)
            {
                menu.InitMenu();
            }

            InitBoard();

            var shape = UnityEngine.Random.Range(0, (int)Shapes.Max);
            CreateNewShape((Shapes)shape);
            return true;
        }

        private void CreateNewShape(Shapes shape)
        {
            var obj = Instantiate<TetrisShape>(shapePrefab);
            if (obj == null)
                return;

            if (!obj.Init(shape, new Vector2(5, 20)))
            {
                return;
            }

            currShape = obj;
            currShape.transform.position = GetPosition(currShape.Coordinate.x, currShape.Coordinate.y);
        }

        Vector3 GetPosition(float x, float y)
        {
            const float tileSize = 1.0f;
            float tileHarfSize = tileSize * 0.5f;
            float total_width = tileSize * width;
            float total_height = tileSize * height;

            float xpos = x * tileSize - (total_width * 0.5f) + tileHarfSize;
            float ypos = y * tileSize - (total_height * 0.5f) + tileHarfSize;

            return new Vector3(xpos, ypos);
        }

        public void OnLeft() 
        {
            if (!Possiable(currShape.Rotate, currShape.Coordinate.x - 1, currShape.Coordinate.y))
            {
                return;
            }

            currShape.Coordinate.x--;
            currShape.transform.position = GetPosition(currShape.Coordinate.x, currShape.Coordinate.y);
        }

        public void OnRight() 
        {
            if (!Possiable(currShape.Rotate, currShape.Coordinate.x + 1, currShape.Coordinate.y))
            {
                return;
            }

            currShape.Coordinate.x++;
            currShape.transform.position = GetPosition(currShape.Coordinate.x, currShape.Coordinate.y);
        }
        public void OnDown() 
        {
            if (!Possiable(currShape.Rotate, currShape.Coordinate.x, currShape.Coordinate.y - 1))
            {
                return;
            }

            currShape.Coordinate.y--;
            currShape.transform.position = GetPosition(currShape.Coordinate.x, currShape.Coordinate.y);
        }

        public void OnUp()
        {
            if (!Possiable(currShape.Rotate, currShape.Coordinate.x, currShape.Coordinate.y + 1))
            {
                return;
            }

            currShape.Coordinate.y++;
            currShape.transform.position = GetPosition(currShape.Coordinate.x, currShape.Coordinate.y);
        }

        public void OnRotateShape()
        {
            var nextRotate = (currShape.Rotate + 1) % 4;
            if (!Possiable(nextRotate, currShape.Coordinate.x, currShape.Coordinate.y))
            {
                return;
            }
            
            currShape.RotateShape();
        }

        public void OnInsert()
        {
            var arry = currShape.Arr;
            var rotate = currShape.Rotate;
            var coord = currShape.Coordinate;

            // Shape 모양있는 부분이 바깥으로 나가는지 확인.
            for (int row = 0; row < TetrisShape.Height; row++)
            {
                for (int col = 0; col < TetrisShape.Width; col++)
                {
                    if (arry[rotate, row, col] != 0)
                    {
                        int boardX = (int)coord.x + col;
                        int boardY = (int)coord.y + row;

                        if (boardArray[boardX, boardY].Value == 0)
                        {
                            boardArray[boardX, boardY].SetValue(arry[rotate, row, col]);
                        }
                    }
                }
            }

            Destroy(currShape.gameObject);
            currShape = null;

            var shape = UnityEngine.Random.Range(0, (int)Shapes.Max);
            CreateNewShape((Shapes)shape);
        }

        private bool Possiable(int rotate, float x, float y)
        {
            var arry = currShape.GetArray(rotate);

            // Shape 모양있는 부분이 바깥으로 나가는지 확인.
            for (int row = 0; row < TetrisShape.Height; row++)
            {
                for (int col = 0; col < TetrisShape.Width; col++)
                {
                    if (arry[row, col] != 0)
                    {
                        int boardX = (int)x + col;
                        int boardY = (int)y + row;

                        if (boardX >= width || boardX < 0)
                            return false;

                        if (boardY >= height || boardY < 0)
                            return false;

                        if (boardArray[boardX, boardY].Value != 0)
                            return false;
                    }
                }
            }

            return true;
        }

        private void InitBoard()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var obj = Instantiate<TetrisTile>(tilePrefab, board.transform);
                    if (obj == null)
                        continue;

                    obj.transform.position = GetPosition(x, y);
                    obj.name = $"{x},{y}";
                    obj.Coordnate = new Vector2(x, y);
                    obj.SetValue(0);
                    
                    // 외곽 영역
                    if (x == 0 || x == width - 1 || y <= 4 || y >= 24)
                    {
                        obj.SetValue(-1);
                    }

                    boardArray[x, y] = obj;
                }
            }
        }

        public override void OnTouchBean(Vector3 position) {

            /*
            Ray ray = MainCamera.ScreenPointToRay(position);
 
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                var layer = hit.collider.gameObject.layer;
                if (layer == LayerMask.NameToLayer("Tile"))
                {
                    var obj = hit.collider.gameObject;
                    var tile = obj.GetComponent<TetrisTile>();
                    if (tile != null)
                    {
                        if (Possiable(currShape.Rotate, tile.Coordnate.x, tile.Coordnate.y) == true)
                        {
                            currShape.Coordinate = tile.Coordnate;
                            currShape.transform.position = GetPosition(tile.Coordnate.x, tile.Coordnate.y);
                        }
                    }

                    Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 5f);
                    Debug.Log(hit.collider.gameObject.name);
                }
            }
            */

            movedPosition = Vector2.zero;
        }

        private Vector2 movedPosition = Vector2.zero;

        public override void OnTouchMove(Vector3 position, Vector2 deltaPosition) {

            movedPosition += deltaPosition;
            var diff = Vector2.zero - movedPosition;

            if (diff.x < -10.0)
            {
                movedPosition = Vector2.zero;
                OnLeft();
            }
            else if (diff.x >= 10.0)
            {
                movedPosition = Vector2.zero;
                OnRight();
            }
            else if (diff.y < -10.0)
            {
                movedPosition = Vector2.zero;
                OnDown();
            }
            else if (diff.y >= 10.0)
            {
                movedPosition = Vector2.zero;
                OnUp();
            }

            /*
            Ray ray = MainCamera.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                var layer = hit.collider.gameObject.layer;
                if (layer == LayerMask.NameToLayer("Tile"))
                {
                    var obj = hit.collider.gameObject;
                    var tile = obj.GetComponent<TetrisTile>();
                    if (tile != null)
                    {
                        if (Possiable(currShape.Rotate, tile.Coordnate.x, tile.Coordnate.y) == true)
                        {
                            currShape.Coordinate = tile.Coordnate;
                            currShape.transform.position = GetPosition(tile.Coordnate.x, tile.Coordnate.y);
                        }
                    }

                    Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 5f);
                    Debug.Log(hit.collider.gameObject.name);
                }
            }
            */
        }
        
        public override void OnTouchEnd(Vector3 position) {
            movedPosition = Vector3.zero;
        }
    }
}
