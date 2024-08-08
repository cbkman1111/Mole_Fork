using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Level;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Match3
{
    class RectTexture
    {
        public Texture2DSize square;
        public Rect rect;
    }

    /// <summary>
    /// 씬 인스펙터에 버튼을 연결해서 기능 처리.
    /// </summary>
    [CustomEditor(typeof(Scene3Match))]
    public class MapEditor : Editor
    {
        private string inputRow = "5";
        private string inputCol = "5";
        private int Row = 0;
        private int Col = 0;
        
        Level level = null;

        /// <summary>
        /// 인스펙터 UI 생성.
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            InputSize();
   
            Tool();
        }

        /// <summary>
        /// 좌우 사이즈 입력 받기.
        /// </summary>
        private void InputSize()
        {
            inputRow = GUILayout.TextField(inputRow, GUILayout.Width(200));
            inputCol = GUILayout.TextField(inputCol, GUILayout.Width(200));
            if (int.TryParse(inputRow, out var valueWidth) == true)
            {
                Row = valueWidth;
                inputRow = $"{valueWidth}";
            }
                

            if (int.TryParse(inputCol, out var valueHeight) == true)
            {
                Col = valueHeight;
                inputCol = $"{valueHeight}";
            }
        }

        private void Tool()
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < 10; i++)
            {
                GUILayout.BeginVertical();
                Color squareColor = new Color(0.8f, 0.8f, 0.8f);
                var imageButton = new object();
                if (GUILayout.Button(imageButton as Texture, GUILayout.Width(40), GUILayout.Height(40)))
                {
                }

                GUI.enabled = false; // 텍스트 비활성.
                GUILayout.TextField("test");
                GUI.enabled = true; // 다른 요소를 위해 다시 활성화
                GUILayout.EndVertical();

            }
            GUILayout.EndHorizontal();
        }
    }
}
