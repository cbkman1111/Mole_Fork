using Match3;
using Scenes;
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
        // 자주 사용되는 GUILayout
        // https://howudong.tistory.com/145
        // 아이콘.
        // https://github.com/halak/Unity-editor-icons

        private SceneGostop sceneGostop = null;

        private int HandIndex;
        private int HandCardNum;
        private int DeckCardNum;
        private int DeckIndex;

        private void OnEnable()
        {
            sceneGostop = (SceneGostop)target;
        }

        /// <summary>
        /// 인스펙터 UI 생성.
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);
            GUILayout.Label("손패 교체 설정", EditorStyles.boldLabel);
            ChangeCard();

            GUILayout.Space(10);
            GUILayout.Label("덱 교체 설정", EditorStyles.boldLabel);
            ChanngeDeck();
         
            //CreateBoardSetting();
        }

        /// <summary>
        /// 손패를 교체합니다. (인덱스, 카드번호)
        /// </summary>
        private void ChangeCard()
        {
    
            GUILayout.BeginHorizontal();
            HandIndex = EditorGUILayout.IntField("", HandIndex, GUILayout.Width(50), GUILayout.Height(20)); 

            HandCardNum = EditorGUILayout.IntField("", HandCardNum, GUILayout.Width(50), GUILayout.Height(20));
            if (GUILayout.Button("교체", GUILayout.Width(200), GUILayout.Height(20)))
            {
                var board = sceneGostop.board;
                if (board != null)
                {
                    
                }
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 덱에 있는 패를 교체합니다.
        /// </summary>
        private void ChanngeDeck()
        {
            GUILayout.BeginHorizontal();
            DeckIndex = EditorGUILayout.IntField("", DeckIndex, GUILayout.Width(50), GUILayout.Height(20));

            DeckCardNum = EditorGUILayout.IntField("", DeckCardNum, GUILayout.Width(50), GUILayout.Height(20));
            if (GUILayout.Button("교체", GUILayout.Width(200), GUILayout.Height(20)))
            {
                var board = sceneGostop.board;
                if (board != null)
                {

                }
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 보드 세팅 파일 생성용 함수.
        /// </summary>
        private void CreateBoardSetting()
        {
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            
            if (GUILayout.Button("보드 세팅", GUILayout.Width(200), GUILayout.Height(40)))
            {
                CreateBoardSettingAsset();
            }

            GUILayout.EndVertical();
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
