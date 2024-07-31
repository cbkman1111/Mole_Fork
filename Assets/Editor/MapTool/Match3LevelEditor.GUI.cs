using Match3;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

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
            GUILayout.Label("멥툴", EditorStyles.boldLabel);

            GUILevelSize();
            GUICreateLoadBtns();
            GUILayerTabs();
            GUILayer();
            GUITool();
            GUIBoard();
        }

        /// <summary>
        /// 로드 버튼.
        /// </summary>
        private void GUICreateLoadBtns()
        {
            GUILayoutOption[] spriteOptions = new[] {
            GUILayout.Width (200),
            GUILayout.Height (40)
        };
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Map", spriteOptions))
            {
                LoadLevel(index);
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Create NewMap", spriteOptions))
            {
                CreateMap(index);
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 탭 버튼.
        /// </summary>
        private void GUILayerTabs()
        {
            GUILayout.Label("Layers", EditorStyles.boldLabel);
            tabIndex = GUILayout.Toolbar(tabIndex, taps);
            GUILayout.Space(10);
        }

        /// <summary>
        /// 툴 버튼.
        /// </summary>
        private void GUITool()
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < 10; i++)
            {
                Color squareColor = new Color(0.8f, 0.8f, 0.8f);
                var imageButton = new object();
                if (GUILayout.Button(imageButton as Texture, GUILayout.Width(40), GUILayout.Height(40)))
                {
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
                GUILayout.Label("Layers " + (layer + 1) + " / " + layers, EditorStyles.label, GUILayout.Width(100));
                GUILayout.Space(0);
                layer = (int)GUILayout.HorizontalSlider(layer, layers - 1, 0, GUILayout.Width(100));
                GUILayout.Space(50);
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 보드.
        /// </summary>
        private void GUIBoard()
        {
            List<RectTexture> rects = new List<RectTexture>();
            GUILayout.BeginVertical();

            for (int row = 0; row < maxRows; row++)
            {
                GUILayout.BeginHorizontal();

                for (int col = 0; col < maxCols; col++)
                {
                    Color squareColor = new Color(0.8f, 0.8f, 0.8f);
                    var imageButton = new object();
                    UnityEngine.GUI.color = squareColor;
                    if (GUILayout.Button(imageButton as Texture, GUILayout.Width(40), GUILayout.Height(40)))
                    {
                    }

                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

        }

        /// <summary>
        /// 레벨 정보.
        /// </summary>
        private void GUILevelSize()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Level", GUILayout.Width(50));
            GUILayout.Space(10);

            EditorGUILayout.IntField("", 6, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Columns", GUILayout.Width(80));
            GUILayout.Space(10);

            EditorGUILayout.IntField("", 12, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Rows", GUILayout.Width(80));
            GUILayout.Space(10);

            EditorGUILayout.IntField("", maxRows, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
    }
}