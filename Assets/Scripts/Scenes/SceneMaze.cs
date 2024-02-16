using System.Collections.Generic;
using Common.Global;
using Common.Scene;
using UI.Menu;
using UnityEngine;
using System;
using Scenes.EllersAlgorithm;
using UI.Popup;
using Unity.Jobs;
using Unity.Collections;

namespace Scenes
{
    namespace EllersAlgorithm
    {
        /// <summary>
        /// This class represents a "set" in the context of Eller's algorithm. 
        /// It simply holds a list of cells that are member of this set.
        /// </summary>
        public class Set
        {
            // The cells that are in that set.
            public List<Cell> Cells = new List<Cell>();
        }

        /// <summary>
        /// This class represents a "cell" in the context of Eller's algorithm. 
        /// It holds information about in which set the cell is and if it has a wall on the right or on the bottom.
        /// </summary>
        public class Cell
        {
            public Set Set;                                     // The set the cell is in. (Can be null if the cell has no set, yet.)
            public bool HasRightWall;                           // Has the cell a wall on the right?
            public bool HasBottomWall;                          // Has the cell a wall on the bottom?

            public GameObject block;
        }

        /// <summary>
        /// This class generates and stores the maze.
        /// </summary>
        public class Maze
        {
            // The bias is in other words the "seed" of the maze and can affect its shape. 
            // If the bias is a lot smaller than the maxBias the maze will be more vertically.
            // If the bias is a lot bigger than the maxBias the maze will be more horizontally.
            private const int MaxBias = 64;
            private const int Bias = 32;

            private const int Wall = 1;                   // This is the symbol used to represent a wall
            private const int Path = 0;                   // This is the symbol used to represent a path

            private readonly System.Random _rnd = new System.Random();        // Used to generate random values.

            private int width;                                  // The width of the maze.
            private int height;                                 // The height of the maze.
            public int Width { get => width; }
            public int Height { get => height; }

            private List<Set> sets;                             // A list of the sets in the current row.
            private List<Cell> row;                             // The current row. (NOTE: The Eller's only needs to load one row in the memory to generate a maze!)

            public Cell[,] maze;                               // A two dimensional array of cells to store the maze into it.

            /// <summary>
            /// Generates the maze.
            /// </summary>
            /// <param name="height">
            /// The height of the maze.
            /// </param>
            /// <param name="width">
            /// The width of the maze.
            /// </param>
            public void GenerateMaze(int width, int height)
            {
                this.width = width;                             // Sets the width of the maze.
                this.height = height;                           // Sets the height of the maze.

                maze = new Cell[width, height];                 // Initalizes an empty two dimensional array to store the maze into it.
                sets = new List<Set>();                         // Initialize new list of sets.
                row = new List<Cell>();                         // Initialize new row.

                // Fill the first row with new empty cells
                for (int i = 0; i < this.width; i++)
                {
                    row.Add(new Cell());
                }

                // This is the section where the algorithm finally executes after all the preperations.
                for (int x = 0; x < this.height; x++)
                {

                    // Handles the last row which is a special case.
                    if (x == this.height - 1)
                    {
                        // Every cell that has no set, will have its own unique set.
                        InitSets();

                        // In the last row every cell will have a bottom wall
                        foreach (Cell cell in row)
                        {
                            cell.HasBottomWall = true;
                        }

                        // Create right walls
                        for (int i = 0; i < row.Count - 1; i++)
                        {
                            // Delete all rows that divide different sets
                            if (row[i].Set != row[i + 1].Set)
                            {
                                row[i].HasRightWall = false;
                            }
                            else
                            {
                                row[i].HasRightWall = true;
                            }

                        }

                        // The last cell in a row always has to have a right wall because that's where the border of the maze is.
                        row[row.Count - 1].HasRightWall = true;

                        // Stores the row in the maze.
                        WriteRowIntoMaze(x);

                        // End the loop because the last row has already been done.
                        continue;
                    }

                    // Every cell that has no set, will have its own unique set.
                    InitSets();

                    // If there are multiple cells with the same set place a wall between them. 
                    // Otherwise you will get "holes" in your maze. Just remove this code-segment and generate some mazes to see it!
                    for (int i = 0; i < row.Count - 1; i++)
                    {
                        if (row[i].Set == row[i + 1].Set)
                        {
                            row[i].HasRightWall = true;
                        }
                    }

                    // Create the right walls. 
                    CreateRightWalls();

                    // Create bottom walls. (NOTE: Each set need at least one cell without a bottom wall.)
                    CreateDownWalls();

                    // Stores the row in the maze.
                    WriteRowIntoMaze(x);

                    // Prepare the next row.
                    PrepareNextRow();
                }
            }

            /// <summary>
            /// Returns a random bool and is used to define if a right wall should be created or not.
            /// </summary>
            private bool CreateWall
            {
                get
                {
                    int x = _rnd.Next(0, MaxBias + 1);

                    if (x > Bias)
                    {
                        return true;
                    }

                    return false;
                }
            }

            /// <summary>
            /// This will define foreach cell in a row if it has a right wall or not.
            /// </summary>
            private void CreateRightWalls()
            {
                // i is the left cell(lc) and i + 1 is the right cell(rc) => | lc | rc |
                for (int i = 0; i < row.Count - 1; i++)
                {

                    // Randomly create a wall.
                    if (CreateWall)
                    {
                        row[i].HasRightWall = true;
                    }
                    else if (row[i].Set == row[i + 1].Set)
                    {
                        // If the left and the right cell have the same set there needs to be a right wall to not create loops in the maze.
                        row[i].HasRightWall = true;
                    }
                    else
                    {
                        // Merge cells to the same set.
                        row[i + 1].Set.Cells.Remove(row[i + 1]);
                        row[i].Set.Cells.Add(row[i + 1]);
                        row[i + 1].Set = row[i].Set;
                    }
                }

                // The last cell in a row always has to have a right wall because that's where the border of the maze is.
                row[row.Count - 1].HasRightWall = true;
            }

            /// <summary>
            /// This will define foreach cell in a row if it has a bottom wall or not.
            /// </summary>
            private void CreateDownWalls()
            {
                // NOTE: Every set needs to have at least one path downwards.
                foreach (Set set in sets.ToArray())
                {
                    // Check if the set is used by any cells. (NOTE: There can be sets without any cells due to merging the sets of different cells.)
                    if (set.Cells.Count > 0)
                    {
                        // The cellIndices store the information which cells of a set should NOT have bottom walls.
                        List<int> cellIndices = new List<int>();

                        // If a set only has one cell it must not have a bottom wall!
                        if (set.Cells.Count == 1)
                        {
                            cellIndices.Add(0);
                        }
                        else
                        {
                            // Randomly choose how many paths you want to have downwards. (NOTE: Each set needs at least one path downwards!)
                            int downPaths = _rnd.Next(1, set.Cells.Count + 1);

                            // Randomly choose which cells of the set should have the downPaths.
                            for (int i = 0; i < downPaths; i++)
                            {
                                int index;

                                // Make sure that an index won't be added multiple times!
                                do
                                {
                                    index = _rnd.Next(0, set.Cells.Count);

                                } while (cellIndices.Contains(index));

                                cellIndices.Add(index);
                            }
                        }

                        // Add bottom walls.
                        for (int k = 0; k < set.Cells.Count; k++)
                        {
                            if (!cellIndices.Contains(k))
                            {
                                set.Cells[k].HasBottomWall = true;
                            }
                            else
                            {
                                set.Cells[k].HasBottomWall = false;
                            }
                        }
                    }
                    else
                    {
                        // Remove empty sets to clean up a little bit.
                        sets.Remove(set);
                    }
                }
            }

            /// <summary>
            /// Prepares a new row.
            /// </summary>
            private void PrepareNextRow()
            {
                foreach (Cell cell in row)
                {
                    // Remove all right walls for the next row.
                    cell.HasRightWall = false;

                    // If a cell in the previous row had a down wall the cell beneathe must not have a down wall nor a set. 
                    if (cell.HasBottomWall)
                    {
                        cell.Set.Cells.Remove(cell);
                        cell.Set = null;
                        cell.HasBottomWall = false;
                    }
                }
            }

            /// <summary>
            /// For the algorithm to work you need to make a unique set for each cell where the set is empty.
            /// </summary>
            private void InitSets()
            {
                foreach (Cell cell in row)
                {
                    if (cell.Set == null)
                    {
                        Set set = new Set();        // Create a new set...
                        cell.Set = set;             // ...and assign it to the cell

                        set.Cells.Add(cell);        // Add the cell to the set.
                        sets.Add(set);              // Add the set into the list of sets.
                    }
                }
            }

            /// <summary>
            /// Eller's algorithm always loads only one row into memory. So you need to store every row before you move on to the next one.
            /// You could also print every row before moving on to the next row, but then you can't access the whole maze! (Or simply doing both :P)
            /// </summary>
            /// <param name="h">
            /// 'h' stands for height and tells the how many rows this is.
            /// </param>
            private void WriteRowIntoMaze(int h)
            {
                for (int i = 0; i < row.Count; i++)
                {
                    // You need to store a new object into the arrays cell, because otherwise it will only store 
                    // the reference to the rows cell and it will be overridden after the next step in the loop
                    Cell cell = new Cell
                    {
                        HasBottomWall = row[i].HasBottomWall,
                        HasRightWall = row[i].HasRightWall
                    };

                    maze[i, h] = cell;
                }
            }

            /// <summary>
            /// Prints the whole maze
            /// </summary>
            public void PrintMaze()
            {
                var prefab = ResourcesManager.Instance.LoadInBuild<GameObject>("Prefab/MazeBlock");

                float TileSize = 0.9f;
                var startX = (width * TileSize) * -0.5f;
                var startY = (height * TileSize) * 0.5f;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        var block = GameObject.Instantiate<GameObject>(prefab);
                        var left = block.transform.Find("Left");
                        var right = block.transform.Find("Right");
                        var down = block.transform.Find("Down");
                        var up = block.transform.Find("Up");

                        block.transform.position = new Vector3(startX + j * TileSize, startY - i * TileSize, 0);
                        maze[j, i].block = block;

                        if (j == 0)
                        {
                            left.gameObject.SetActive(true);
                        }
                        else
                        {
                            left.gameObject.SetActive(false);
                        }
                        if (i == 0)
                        {
                            up.gameObject.SetActive(true);
                        }
                        else
                        {
                            up.gameObject.SetActive(false);
                        }

                        if (maze[j, i].HasRightWall && maze[j, i].HasBottomWall)
                        {
                            //left.gameObject.SetActive(false);
                            right.gameObject.SetActive(true);
                            down.gameObject.SetActive(true);
                            //up.gameObject.SetActive(false);
                        }
                        else if (maze[j, i].HasRightWall && !maze[j, i].HasBottomWall)
                        {
                            //left.gameObject.SetActive(false);
                            right.gameObject.SetActive(true);
                            down.gameObject.SetActive(false);
                            //up.gameObject.SetActive(false);
                        }
                        else if (maze[j, i].HasBottomWall && !maze[j, i].HasRightWall)
                        {
                            //left.gameObject.SetActive(false);
                            right.gameObject.SetActive(false);
                            down.gameObject.SetActive(true);
                           // up.gameObject.SetActive(false);
                        }
                        else
                        {
                            //left.gameObject.SetActive(false);
                            right.gameObject.SetActive(false);
                            down.gameObject.SetActive(false);
                            //up.gameObject.SetActive(false);
                        }
                    }

                    Console.WriteLine();
                }
            }

            /// <summary>
            /// Because I'm using this algorithm to generate maps for a pixel style game with a grid-system I need to translate the maze. 
            /// (Ongoing a cell of the grid-system of the game will be called "block".)
            /// The game only can handle if a block in the grid should be a wall or a path. It can NOT handle a single block having a path AND a wall.
            /// So in this case a cell in the context of eller's algorithm will be four blocks shaped in a square in my game.
            /// Imagine a cell having a right wall. With four blocks I can have the two blocks on the right being walls and the remaining two blocks on the 
            /// left being the path the player can go throug.
            /// </summary>
            /// <returns>
            /// Returns a two dimensional string array. So I can simply exchange the strings representing a wall or a path to the corresponding game object.
            /// </returns>
            public int[,] TranslateMaze()
            {
                // Because I'm translating one cell into four blocks the array needs to be two times the height and two times the width.
                // Then I'm also adding +2 so I can generate the most left wall and the most top wall, too. 
                // NOTE: Eller's algorithm does not generate the most left wall and the most top wall by default, because it only generates right and bottom walls!
                int[,] mazeTranslation = new int[height * 2 + 2, width * 2 + 2];

                for (int i = 0; i < height + 1; i++)
                {
                    // The indices of the maze[x, y] still needs to start at zero so the first row won't be left out. 
                    // Because of the generation of the top and the left border of the wall there's an offset of 1.
                    int y = i - 1;

                    for (int j = 0; j < width + 1; j++)
                    {
                        // The indices of the maze[x, y] still needs to start at zero so the first row won't be left out. 
                        // Because of the generation of the top and the left border of the wall there's an offset of 1.
                        int x = j - 1;

                        // Creates the top border of the maze.
                        if (i == 0)
                        {
                            mazeTranslation[i * 2, j * 2] = Wall;                   // This is the top left block
                            mazeTranslation[i * 2, j * 2 + 1] = Wall;               // This is the top right block
                            mazeTranslation[i * 2 + 1, j * 2] = Wall;               // This is the bottom left block
                            mazeTranslation[i * 2 + 1, j * 2 + 1] = Wall;           // This is the bottom right block

                            continue;
                        }

                        // Create most left border of the maze.
                        if (j == 0)
                        {
                            mazeTranslation[i * 2, j * 2] = Wall;                   // This is the top left block
                            mazeTranslation[i * 2, j * 2 + 1] = Wall;               // This is the top right block
                            mazeTranslation[i * 2 + 1, j * 2] = Wall;               // This is the bottom left block
                            mazeTranslation[i * 2 + 1, j * 2 + 1] = Wall;           // This is the bottom right block

                            continue;
                        }

                        if (maze[x, y].HasRightWall && maze[x, y].HasBottomWall)
                        {
                            mazeTranslation[i * 2, j * 2] = Path;                   // This is the top left block
                            mazeTranslation[i * 2, j * 2 + 1] = Wall;               // This is the top right block
                            mazeTranslation[i * 2 + 1, j * 2] = Wall;               // This is the bottom left block
                            mazeTranslation[i * 2 + 1, j * 2 + 1] = Wall;           // This is the bottom right block

                            // Fills corners to look nicer. (Remove this section and run the code to see what I mean.)
                            if (i > 1)
                            {
                                mazeTranslation[i * 2 - 1, j * 2 + 1] = Wall;
                            }

                            // Fills corners to look nicer. (Remove this section and run the code to see what I mean.)
                            if (j > 1)
                            {
                                mazeTranslation[i * 2 + 1, j * 2 - 1] = Wall;
                            }
                        }
                        else if (maze[x, y].HasRightWall && !maze[x, y].HasBottomWall)
                        {
                            mazeTranslation[i * 2, j * 2] = Path;                   // This is the top left block
                            mazeTranslation[i * 2, j * 2 + 1] = Wall;               // This is the top right block
                            mazeTranslation[i * 2 + 1, j * 2] = Path;               // This is the bottom left block
                            mazeTranslation[i * 2 + 1, j * 2 + 1] = Wall;           // This is the bottom right block

                            // Fills corners to look nicer. (Remove this section and run the code to see what I mean.)
                            if (i > 1)
                            {
                                mazeTranslation[i * 2 - 1, j * 2 + 1] = Wall;
                            }
                        }
                        else if (!maze[x, y].HasRightWall && maze[x, y].HasBottomWall)
                        {
                            mazeTranslation[i * 2, j * 2] = Path;                   // This is the top left block
                            mazeTranslation[i * 2, j * 2 + 1] = Path;               // This is the top right block
                            mazeTranslation[i * 2 + 1, j * 2] = Wall;               // This is the bottom left block
                            mazeTranslation[i * 2 + 1, j * 2 + 1] = Wall;           // This is the bottom right block

                            // Extends bottom walls to look nicer. (Remove this section and run the code to see what I mean.)
                            if (j > 1)
                            {
                                mazeTranslation[i * 2 + 1, j * 2 - 1] = Wall;
                            }
                        }
                        else
                        {
                            mazeTranslation[i * 2, j * 2] = Path;                   // This is the top left block
                            mazeTranslation[i * 2, j * 2 + 1] = Path;               // This is the top right block
                            mazeTranslation[i * 2 + 1, j * 2] = Path;               // This is the bottom left block
                            mazeTranslation[i * 2 + 1, j * 2 + 1] = Path;           // This is the bottom right block
                        }
                    }
                }

                // Return the translated maze.
                return mazeTranslation;
            }
        }
    }

    public struct MyJob : IJob
    {
        public long b;
        public NativeArray<long> result;

        public void Execute()
        {
            for (long i = 0; i < 990000000; i++)
            {
                b = i;
            }

            result[0] = b;
        }
    }

    public class SceneMaze : SceneBase
    {
        private UIMenuMaze menu = null;
        private int rows = 10;
        private int cols = 10;
        
        //NativeArray<long> result = new NativeArray<long>(1, Allocator.TempJob);

        //MazeGenerator mazeGenerator = null;
        public Maze mazeGenerator = null;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            menu = UIManager.Instance.OpenMenu<UIMenuMaze>("UIMenuMaze");
            if (menu != null)
            {
                menu.InitMenu();
            }

            mazeGenerator = new Maze();
            mazeGenerator.GenerateMaze(rows, cols);
            mazeGenerator.PrintMaze();

            var maze = mazeGenerator.TranslateMaze();
            //int n1 = EscapeMaze(maze);
            //int n2 = EscapeMaze2(mazeGenerator.maze);
            //mazeGenerator.SolveMaze();

            var prefabPlayer = ResourcesManager.Instance.LoadInBuild<MazeMouse>("Prefab/MazeMouse");
            var player = GameObject.Instantiate<MazeMouse>(prefabPlayer);
            player.Coordinate = Vector2Int.zero;
            player.transform.position = mazeGenerator.maze[0, 0].block.transform.position;

            var prefabStart = ResourcesManager.Instance.LoadInBuild<GameObject>("Prefab/MazeStart");
            var start = GameObject.Instantiate<GameObject>(prefabStart);
            start.transform.position = mazeGenerator.maze[0, 0].block.transform.position;

            var prefabGoal = ResourcesManager.Instance.LoadInBuild<GameObject>("Prefab/MazeGoal");
            var goal = GameObject.Instantiate<GameObject>(prefabGoal);
            goal.transform.position = mazeGenerator.maze[rows -1, cols - 1].block.transform.position;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maps"></param>
        /// <returns></returns>
        public static int EscapeMaze(int[,] maps)
        {
            int rows = maps.GetLength(0);
            int cols = maps.GetLength(1);
            int[,] directions = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } }; // 아래, 위, 오른쪽, 왼쪽

            Queue<Tuple<int, int, int>> queue = new Queue<Tuple<int, int, int>>();
            HashSet<Tuple<int, int>> visited = new HashSet<Tuple<int, int>>();

            queue.Enqueue(new Tuple<int, int, int>(2, 2, 1));
            visited.Add(new Tuple<int, int>(2, 2));

            while (queue.Count > 0)
            {
                Tuple<int, int, int> current = queue.Dequeue();
                int row = current.Item1;
                int col = current.Item2;
                int distance = current.Item3;

                if (row == rows - 2 && col == cols - 2) // 출구에 도달한 경우
                {
                    return distance;
                }

                for (int i = 0; i < 4; i++)
                {
                    int newRow = row + directions[i, 0];
                    int newCol = col + directions[i, 1];

                    if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                    {
                        bool allreadyVisited = visited.Contains(new Tuple<int, int>(newRow, newCol));
                        if (maps[newRow, newCol] == 0 && allreadyVisited == false)
                        {
                            visited.Add(new Tuple<int, int>(newRow, newCol));
                            queue.Enqueue(new Tuple<int, int, int>(newRow, newCol, distance + 1));
                        }
                    }
                }
            }

            return -1; // 출구에 도달할 수 없는 경우
        }

        /// <summary>
        /// 미리 로딩해야 할 데이터 처리.
        /// </summary>


        public async override void Load(Action<float> update)
        {
            //Amount = 1f;

            update(0.1f);
        }
       
        public void OnGameOver()
        {
            var popup = UIManager.Instance.OpenPopup<PopupNormal>("PopupNormal");
            popup.SetUI();
            popup.SetOnClose(() => {
                AppManager.Instance.ChangeScene(Scenes.SceneMenu);
            });
        }

        public override void OnTouchBean(Vector3 position)
        {
            Ray ray = MainCamera.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                var layer = hit.collider.gameObject.layer;
                if (layer == LayerMask.NameToLayer("Tile"))
                {
                    var obj = hit.collider.gameObject;
               
                }

                Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 5f);
                Debug.Log(hit.point);
            }
        }

        public override void OnTouchEnd(Vector3 position)
        {

        }

        public override void OnTouchMove(Vector3 position, Vector2 deltaPosition)
        {

        }
    }
}