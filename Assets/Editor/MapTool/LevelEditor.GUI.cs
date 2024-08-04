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
                    break;
                case (int)TabTypes.Candy:
                    GUIToolCandy();
                    break;
                case (int)TabTypes.Direction:
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
            GUILayout.Label("멥툴", labelStyle);
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
            GUILayout.Label("레이어", EditorStyles.boldLabel);
            TabIndex = GUILayout.Toolbar(TabIndex, tabs);

            GUILayout.Space(10);
        }

        private void GUIToos()
        {
            GUILayout.Label("툴", EditorStyles.boldLabel);
            GUILayout.Space(10);
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
            GUILayout.Label("보드", EditorStyles.boldLabel);
            GUILayout.Space(10);
            GUILayout.Space(20); // 공백 라인 추가

            List<RectTexture> rects = new List<RectTexture>();
            GUILayout.BeginVertical();
            
            for (int row = 0; row < level.Row; row++)
            {
                GUILayout.BeginHorizontal();

                for (int col = 0; col < level.Col; col++)
                {
                    Color squareColor = new Color(0.8f, 0.8f, 0.8f);
                    var imageButton = new object();
                    UnityEngine.GUI.color = squareColor;
                    
                    if (GUILayout.Button(imageButton as Texture, GUILayout.Width(40), GUILayout.Height(40)))
                    {
                        ClickBoard(row, col);
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
            switch (TabIndex)
            {
                case (int)TabTypes.Block:
                    break;

                case (int)TabTypes.Candy:
                    break;

                case (int)TabTypes.Direction:
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

            level.Stage = EditorGUILayout.IntField("", level.Stage, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Columns", GUILayout.Width(80));
            GUILayout.Space(10);

            level.Col = EditorGUILayout.IntField("", level.Col, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Rows", GUILayout.Width(80));
            GUILayout.Space(10);

            level.Row = EditorGUILayout.IntField("", level.Row, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
    }
}