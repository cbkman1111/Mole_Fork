using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Match3
{
    /// <summary>
    /// 맵 에디터.
    /// </summary>
    public partial class LevelEditor : EditorWindow
    {
        /// <summary>
        /// GUI 드로우.
        /// </summary>
        void OnGUI()
        {
            GUITitle(); // title draw.

            GUILevelSize(); // level size draw.
            GUILayerTabs();
            GUILayer();

            GUIToos(); // 툴 타이틀.

            switch (TabIndex)
            {
                case (int)TabTypes.Block:
                    GUIToolBlock();
                    GUIToolBlockSub();
                    break;
                case (int)TabTypes.Candy:
                    GUIToolCandy();
                    break;
                case (int)TabTypes.Direction:
                    GUIToolDirection();
                    break;
                case (int)TabTypes.Teleport:
                    break;
                case (int)TabTypes.Generate:
                    break;
            }
            
            GUIBoard();
            GUICreateLoadBtns();
        }

        /// <summary>
        /// 
        /// </summary>
        private void GUITitle()
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
            labelStyle.fontSize = 20; // 원하는 폰트 크기로 설정
            GUILayout.Label("맵툴", labelStyle);
            GUILayout.Space(10);

            GUILayoutOption[] spriteOptions = new[] {
                GUILayout.Width (200),
                GUILayout.Height (40)
            };
        }

        /// <summary>
        /// 로드 버튼.
        /// </summary>
        private void GUILoadBtn()
        {
            GUILayoutOption[] spriteOptions = new[] {
                GUILayout.Width (200),
                GUILayout.Height (40)
            };

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("레벨 로드", spriteOptions))
            {
                LoadLevel(level.Stage);
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 로드 버튼.
        /// </summary>
        private void GUICreateLoadBtns()
        {
            GUILayout.Space(20); // 공백 라인 추가

            GUILayoutOption[] spriteOptions = new[] {
                GUILayout.Width (200),
                GUILayout.Height (40)
            };

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("레벨 로드", spriteOptions))
            {
                var stage = level.Stage;
                LoadLevel(stage);
            }

            GUILayout.Space(10);
            if (GUILayout.Button("레벨 저장", spriteOptions))
            {
                SaveLevel();
            }

            GUILayout.Space(10);
            if (GUILayout.Button("초기화", spriteOptions))
            {
                Init();
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 탭 버튼.
        /// </summary>
        private void GUILayerTabs()
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
            labelStyle.fontSize = 20; // 원하는 폰트 크기로 설정
            GUILayout.Label("레이어", labelStyle);

            TabIndex = GUILayout.Toolbar(TabIndex, tabs);

            GUILayout.Space(10);
        }

        private void GUIToos()
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
            labelStyle.fontSize = 20; // 원하는 폰트 크기로 설정
            GUILayout.Label("툴", labelStyle);

            GUILayout.Space(10);
        }

        private void GUIToolBlockSub()
        {
            GUILayout.BeginHorizontal();

            UnityEngine.GUI.color = SelectBlockDelete == true ? Color.white : new Color(0.4f, 0.4f, 0.4f);
            if (GUILayout.Button("지우기", GUILayout.Width(60), GUILayout.Height(40)))
            {
                SelectBlockDelete = !SelectBlockDelete;
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 툴 버튼.
        /// </summary>
        private void GUIToolBlock()
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < (int)BlockTypes.Max; i++)
            {
                Color squareColor = Color.white;
                if (i == SelectBlock)
                {
                    squareColor = Color.white;
                }
                else
                {
                    squareColor = new Color(0.4f, 0.4f, 0.4f);
                }

                UnityEngine.GUI.color = squareColor;
                if (GUILayout.Button(blockIcons[i] as Texture, GUILayout.Width(60), GUILayout.Height(60)))
                {
                    SelectBlock = i;
                }
            }

            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// 툴 버튼.
        /// </summary>
        private void GUIToolCandy()
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < (int)CandyTypes.Max; i++)
            {
                Color squareColor = Color.white;
                if (i == SelectCandy)
                {
                    squareColor = Color.white; 
                }
                else
                {
                    squareColor = new Color(0.4f, 0.4f, 0.4f);
                }

                UnityEngine.GUI.color = squareColor;
                if (GUILayout.Button(candyIcons[i] as Texture, GUILayout.Width(60), GUILayout.Height(60)))
                {
                    SelectCandy = i;
                }
            }

            GUILayout.EndHorizontal();
        }

        private void GUIToolDirection()
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < (int)DirectionTypes.Max; i++)
            {
                Color squareColor = Color.white;
                if (i == SelectDirection)
                {
                    squareColor = Color.white;
                }
                else
                {
                    squareColor = new Color(0.4f, 0.4f, 0.4f);
                }

                UnityEngine.GUI.color = squareColor;
                if (GUILayout.Button(directions[i] as Texture, GUILayout.Width(60), GUILayout.Height(60)))
                {
                    SelectDirection = i;
                }
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 레이어 슬라이드.
        /// </summary>
        private void GUILayer()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Layers " + (Layer + 1) + " / " + level.Layers, EditorStyles.label, GUILayout.Width(100));
                GUILayout.Space(0);
                Layer = (int)GUILayout.HorizontalSlider(Layer, level.Layers - 1, 0, GUILayout.Width(100));
                GUILayout.Space(50);
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 보드.
        /// </summary>
        private void GUIBoard()
        {
            UnityEngine.GUI.color = Color.white;

            GUILayout.Space(10);
            GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
            labelStyle.fontSize = 20; // 원하는 폰트 크기로 설정
            GUILayout.Label("보드", labelStyle);
            GUILayout.BeginVertical();
            
            for (int row = 0; row < level.Row; row++)
            {
                GUILayout.BeginHorizontal();

                for (int col = 0; col < level.Col; col++)
                {
                    int adress = row * level.Col + col;
                    var square = level.field.Squares[adress];
                    Color squareColor = new Color(0.8f, 0.8f, 0.8f);
                    var imageButton = new object();
                    UnityEngine.GUI.color = squareColor;

                    switch (TabIndex)
                    {
                        case (int)TabTypes.Direction:
                            if(square.direction.x == 0 && square.direction.y == -1)
                            {
                                imageButton = directions[0];
                            }
                            else if (square.direction.x == 0 && square.direction.y == 1)
                            {
                                imageButton = directions[1];
                            }
                            else if (square.direction.x == -1 && square.direction.y == 0)
                            {
                                imageButton = directions[2];
                            }
                            else if (square.direction.x == 1 && square.direction.y == 0)
                            {
                                imageButton = directions[3];
                            }

                            if (imageButton != null)
                            {
                                if (GUILayout.Button(imageButton as Texture, GUILayout.Width(40), GUILayout.Height(40)))
                                {
                                    ClickBoard(row, col);
                                }
                            }
                            break;

                        case (int)TabTypes.Block:
                            BlockTypes blockTypes = BlockTypes.None;
                            if (square.block.Count > Layer)
                            {
                                blockTypes = square.block[Layer];
                            }

                            imageButton = blockIcons[(int)blockTypes];
                            if (GUILayout.Button(imageButton as Texture, GUILayout.Width(40), GUILayout.Height(40)))
                            {
                                ClickBoard(row, col);
                            }

                            break;

                        case (int)TabTypes.Candy:
                            UnityEngine.GUI.color = Color.white;
                            imageButton = candyIcons[(int)square.candy];
                            if (GUILayout.Button(imageButton as Texture, GUILayout.Width(40), GUILayout.Height(40)))
                            {
                                ClickBoard(row, col);
                            }
                            break;
                        default:
                            if (GUILayout.Button(imageButton as Texture, GUILayout.Width(40), GUILayout.Height(40)))
                            {
                                ClickBoard(row, col);
                            }
                            break;
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 탭 상태에 따른 보드 클릭시 처리.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void ClickBoard(int row, int col)
        {
            int adress = row * level.Col + col;
            var square = level.field.Squares[adress];

            switch (TabIndex)
            {
                case (int)TabTypes.Block:
                    if(square.block.Contains((BlockTypes)SelectBlock) == false)
                    {
                        square.block.Add((BlockTypes)SelectBlock);
                    }
                    break;

                case (int)TabTypes.Candy:
                    level.field.Squares[row * level.Col + col].candy = (CandyTypes)SelectCandy;
                    break;

                case (int)TabTypes.Direction:
                    switch (SelectDirection)
                    {
                        case (int)DirectionTypes.Down:
                            level.field.Squares[row * level.Col + col].direction = new Vector2(0, -1);
                            break;
                        case (int)DirectionTypes.Up:
                            level.field.Squares[row * level.Col + col].direction = new Vector2(0, 1);
                            break;
                        case (int)DirectionTypes.Left:
                            level.field.Squares[row * level.Col + col].direction = new Vector2(-1, 0);
                            break;
                        case (int)DirectionTypes.Right:
                            level.field.Squares[row * level.Col + col].direction = new Vector2(1, 0);
                            break;
                    }
                    break;
                case (int)TabTypes.Teleport:
                    break;
                case (int)TabTypes.Generate:
                    break;
            }
        }
        /// <summary>
        /// 레벨 정보.
        /// </summary>
        private void GUILevelSize()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Stage", GUILayout.Width(80));
            GUILayout.Space(10);

            Stage = EditorGUILayout.IntField("", Stage, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Columns", GUILayout.Width(80));
            GUILayout.Space(10);

            Col = EditorGUILayout.IntField("", Col, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Rows", GUILayout.Width(80));
            GUILayout.Space(10);

            Row = EditorGUILayout.IntField("", Row, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            // 크기가 바뀌면 데이터 변경.
            if(level.Col != Col || level.Row != Row)
            {
                level.Col = Col;
                level.Row = Row;
                level.field = new();
            }

            GUILayout.Space(10);
        }
    }
}