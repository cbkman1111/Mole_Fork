using Match3;
using Scenes;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Level;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gostop
{
    /// <summary>
    /// 씬 인스펙터에 버튼을 연결해서 기능 처리.
    /// </summary>
    [CustomEditor(typeof(SceneGostop))]
    public class GostopEditor : Editor
    {
        //BoardSetting boardTime = null;

        /// <summary>
        /// 인스펙터 UI 생성.
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Tool();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Tool()
        {
            GUILayout.BeginHorizontal();
            //for (int i = 0; i < 10; i++)
            {
                GUILayout.BeginVertical();
                Color squareColor = new Color(0.8f, 0.8f, 0.8f);
                var imageButton = new object();
                if (GUILayout.Button(imageButton as Texture, GUILayout.Width(40), GUILayout.Height(40)))
                {


                }

                //GUI.enabled = false; // 텍스트 비활성.
                //GUILayout.TextField("test");
                //GUI.enabled = true; // 다른 요소를 위해 다시 활성화
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }


        /// <summary>
        /// 고스톱 보드 설정 파일 생성.
        /// </summary>
        private void CreateBoardSettingAsset()
        {
            var path = "Assets/Scenes/SceneGostop/Resources/";
            string fileName = "BoardSetting";
            var container = ScriptableObjectUtil.CreateAsset<BoardSettingContainer>(path, fileName);
            container.SetData(new BoardSetting());
            EditorUtility.SetDirty(container);
            AssetDatabase.SaveAssets();
        }
    }
}
