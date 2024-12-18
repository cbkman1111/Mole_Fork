using Common.Global;
using Common.Scene;
using UI.Menu;
using UnityEngine;
using tetris;
using static tetris.TetrisShape;

namespace Scenes
{
    public class SceneTetris : SceneBase
    {
        static public int width = 12; // 좌우 하나씩더
        static public int height = 28; // 위아래 4개씩더

        public TetrisShape shapePrefab = null;
        public TetrisBoard board = null;
        private TetrisShape currShape = null;
        
        public override bool Init(JSONObject param)
        {
            var menu = UIManager.Instance.OpenMenu<UIMenuTetris>();
            if (menu != null)
            {
                menu.InitMenu();
            }

            board.InitBoard();
            
            CreateNewShape();
            return true;
        }

        private void CreateNewShape()
        {
            Shapes shape = (Shapes)UnityEngine.Random.Range(0, (int)Shapes.Max);
            var obj = Instantiate<TetrisShape>(shapePrefab);
            if (obj == null)
                return;

            if (!obj.Init(shape, new Vector2(5, 20)))
            {
                return;
            }

            currShape = obj;
            currShape.transform.position = board.GetPosition(currShape.Coordinate.x, currShape.Coordinate.y);
        }

        public void OnLeft()
        {
            var nextRotate = currShape.Rotate;
            var array = currShape.GetArray(nextRotate);
            var x = (int)currShape.Coordinate.x - 1;
            var y = (int)currShape.Coordinate.y;

            if (!board.Possiable(array, nextRotate, x, y))
            {
                return;
            }

            currShape.Coordinate.x--;
            currShape.transform.position = board.GetPosition(currShape.Coordinate.x, currShape.Coordinate.y);
        }

        public void OnRight()
        {
            var nextRotate = currShape.Rotate;
            var array = currShape.GetArray(nextRotate);
            var x = (int)currShape.Coordinate.x + 1;
            var y = (int)currShape.Coordinate.y;

            if (!board.Possiable(array, nextRotate, x, y))
            {
                return;
            }

            currShape.Coordinate.x++;
            currShape.transform.position = board.GetPosition(currShape.Coordinate.x, currShape.Coordinate.y);
        }
        public void OnDown()
        {
            var nextRotate = currShape.Rotate;
            var array = currShape.GetArray(nextRotate);
            var x = (int)currShape.Coordinate.x;
            var y = (int)currShape.Coordinate.y - 1;

            if (!board.Possiable(array, nextRotate, x, y))
            {
                return;
            }

            currShape.Coordinate.y--;
            currShape.transform.position = board.GetPosition(currShape.Coordinate.x, currShape.Coordinate.y);
        }

        public void OnUp()
        {
            var nextRotate = currShape.Rotate;
            var array = currShape.GetArray(nextRotate);
            var x = (int)currShape.Coordinate.x;
            var y = (int)currShape.Coordinate.y + 1;

            if (!board.Possiable(array, nextRotate, x, y))
            {
                return;
            }

            currShape.Coordinate.y++;
            currShape.transform.position = board.GetPosition(currShape.Coordinate.x, currShape.Coordinate.y);
        }

        public void OnRotateShape()
        {
            var nextRotate = (currShape.Rotate + 1) % 4;
            var array = currShape.GetArray(nextRotate);
            var x = (int)currShape.Coordinate.x;
            var y = (int)currShape.Coordinate.y;

            if (!board.Possiable(array, nextRotate, x, y))
            {
                return;
            }

            currShape.RotateShape();
        }

        public void OnInsert()
        {
            if (!board.Possiable(currShape))
            {
                return;
            }

            board.OnInsert(currShape);
            
            Destroy(currShape.gameObject);
            currShape = null;

            CreateNewShape();
        }

        public override void OnTouchBean(Vector3 position) 
        {
            lastTile = null;
        }

        private TetrisTile lastTile = null;
        public override void OnTouchMove(Vector3 position, Vector2 deltaPosition) 
        {
            TetrisTile currTile = null;
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
                        currTile = tile;
                     }

                    Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 5f);
                    Debug.Log(hit.collider.gameObject.name);
                }
            }

            if (currTile == null)
                return;

            if (lastTile == null)
            {
                lastTile = currTile;
            }
            else if (lastTile != currTile)
            {
                var diff = currTile.Coordnate - lastTile.Coordnate;
                const float Drag = 1.0f;

                if (diff.x <= -Drag)
                {
                    OnLeft();
                    lastTile = currTile;
                }
                else if (diff.x >= Drag)
                {
                    OnRight();
                    lastTile = currTile;
                }
                
                if (diff.y <= -Drag)
                {
                    OnDown();
                    lastTile = currTile;
                }
                else if (diff.y >= Drag)
                {
                    OnUp();
                    lastTile = currTile;
                }
            }

            //currShape.Coordinate = tile.Coordnate;
            //currShape.transform.position = board.GetPosition(tile.Coordnate.x, tile.Coordnate.y);
        }
        
        public override void OnTouchEnd(Vector3 position) {
            lastTile = null;
        }
    }
}
