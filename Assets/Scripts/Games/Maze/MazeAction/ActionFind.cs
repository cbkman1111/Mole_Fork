using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Common.Global;
using Scenes;
using UnityEngine;
using static MazeMouse;

namespace MAZE
{
    public class ActionFind : ActionMaze
    {
        public override TaskStatus OnUpdate()
        {
            var scene = AppManager.Instance.CurrScene as SceneMaze;
            var rows = scene.mazeGenerator.Width;
            var cols = scene.mazeGenerator.Height;
            var maps = scene.mazeGenerator.maze;

            int[] frontX = new int[] { 0, 0, -1, 1 };
            int[] frontY = new int[] { -1, 1, 0, 0 };
            
            int[] rightX = new int[] { 1, -1, 0, 0 };
            int[] rightY = new int[] { 0, 0, -1, 1 };
            
            int x = mouse.Coordinate.x;
            int y = mouse.Coordinate.y;

            if (x == rows - 1 && y == cols - 1)
            {
                return TaskStatus.Failure;
            }

            // 현재 방향에서 오른쪽.
            int dir = mouse.Direction;
            int x1 = x + rightX[dir]; // 오른쪽
            int y1 = y + rightY[dir];
            int x2 = x + frontX[dir]; // 정면
            int y2 = y + frontY[dir];

            switch ((Dir)dir)
            {
                case Dir.Up:
                    if (x1 >= 0 && x1 < rows && y1 >= 0 && y1 < cols &&
                        maps[x, y].HasRightWall == false)
                    {
                        mouse.CoordinateNext = new Vector2Int(x1, y1);
                        mouse.Direction = (int)Dir.Right;
                    }
                    else if (x2 >= 0 && x2 < rows && y2 >= 0 && y2 < cols &&
                        maps[x2, y2].HasBottomWall == false)
                    {
                        mouse.CoordinateNext = new Vector2Int(x2, y2);
                    }
                    else
                    {
                        mouse.Direction = (int)Dir.Left;
                    }
                    break;

                case Dir.Down:
                    if (x1 >= 0 && x1 < rows && y1 >= 0 && y1 < cols &&
                        maps[x1, y1].HasRightWall == false)
                    {
                        mouse.CoordinateNext = new Vector2Int(x1, y1);
                        mouse.Direction = (int)Dir.Left;
                    }
                    else if (x2 >= 0 && x2 < rows && y2 >= 0 && y2 < cols &&
                        maps[x, y].HasBottomWall == false)
                    {
                        mouse.CoordinateNext = new Vector2Int(x2, y2);
                    }
                    else
                    {
                        mouse.Direction = (int)Dir.Right;
                    }
                    break;

                case Dir.Left:
                    if (x1 >= 0 && x1 < rows && y1 >= 0 && y1 < cols &&
                        maps[x1, y1].HasBottomWall == false)
                    {
                        mouse.CoordinateNext = new Vector2Int(x1, y1);
                        mouse.Direction = (int)Dir.Up;
                    }
                    else if (x2 >= 0 && x2 < rows && y2 >= 0 && y2 < cols &&
                        maps[x2, y2].HasRightWall == false)
                    {
                        mouse.CoordinateNext = new Vector2Int(x2, y2);
                    }
                    else
                    {
                        mouse.Direction = (int)Dir.Down;
                    }
                    break;

                case Dir.Right:
                    if (x1 >= 0 && x1 < rows && y1 >= 0 && y1 < cols &&
                        maps[x, y].HasBottomWall == false)
                    {
                        mouse.CoordinateNext = new Vector2Int(x1, y1);
                        mouse.Direction = (int)Dir.Down;
                    }
                    else if (x2 >= 0 && x2 < rows && y2 >= 0 && y2 < cols &&
                        maps[x, y].HasRightWall == false)
                    {
                        mouse.CoordinateNext = new Vector2Int(x2, y2);
                    }
                    else
                    {
                        mouse.Direction = (int)Dir.Up;
                    }
                    break;
            }

            mouse.SetArrow();
            return TaskStatus.Success;

            /*
            for (int i = 0; i < 4; i++)
            {
                int newRow = row + directions[i, 0];
                int newCol = col + directions[i, 1];

                if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                {
                    bool allreadyVisited = false;// visited.Contains(new Tuple<int, int>(newRow, newCol));

                    // 사이에 벽이 있는지 검사.
                    bool wallExist = false;
                    switch (i)
                    {
                        case 0: // 오른쪽
                            wallExist = maps[row, col].HasRightWall;
                            break;
                        case 1: // 왼쪽
                            wallExist = maps[newRow, newCol].HasRightWall;
                            break;
                        case 2: // 아래
                            wallExist = maps[row, col].HasBottomWall;
                            break;
                        case 3: // 위
                            wallExist = maps[newRow, newCol].HasBottomWall;
                            break;
                    }

                    if (wallExist == false && allreadyVisited == false)
                    {
                        //visited.Add(new Tuple<int, int>(newRow, newCol));
                        //queue.Enqueue(new Tuple<int, int, int>(newRow, newCol, distance + 1));
                        mouse.CoordinateNext = new Vector2Int(newRow, newCol);
                        return TaskStatus.Success;
                    }
                }
            }

            */
        }
    }
}