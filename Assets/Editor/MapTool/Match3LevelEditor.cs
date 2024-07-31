using Match3;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

namespace Match3
{
    public enum PuzzleSquareTypes
    {
        NONE = 0,
        EmptySquare,
        SugarSquare,
        /*
        WireBlock,
        SolidBlock,
        ThrivingBlock,
        JellyBlock,
        SpiralBlock,
        UndestrBlock,
        BigBlock,
        Marshmello,
        ChocoSpread
        */
    }

    /// <summary>
    /// 맵 에디터.
    /// </summary>
    public partial class LevelEditor : EditorWindow
    {
        private const string MenuNameRoot = "매치3/스테이지 에디터";
        private const string MenuNameOpen = MenuNameRoot + "/열기";

        Level level = null;// new Level();
        const int maxRows = 10;
        const int maxCols = 10;
        int layer = 0;
        int layers = 2;
        
        public string[] options = new string[] { "Cube", "Sphere", "Plane" };
        public int index = 0;
        public int tabIndex = 0;
        private string[] taps = { "Blocks", "Items", "Directions", "Teleports", "Spawners" };

        /// <summary>
        /// 매치3 -> 스테이지 에디터 -> 열기
        /// </summary>
        [MenuItem(MenuNameOpen)]
        private static void Open()
        {
            var window = GetWindow(typeof(LevelEditor));
            window.titleContent = new GUIContent("스테이지 에디터");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Stage"></param>
        public void SaveLevel(int Stage)
        {
            var levelScriptable = Resources.Load("Level/Level_" + Stage) as LevelContainer;
            if (levelScriptable != null)
            {
                levelScriptable.SetData(level.DeepCopy(level));
                EditorUtility.SetDirty(levelScriptable);
            }

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelNum"></param>
        public void LoadLevel(int levelNum)
        {
            var levelScriptable = Resources.Load("Level/Level_" + levelNum) as LevelContainer;
            if (levelScriptable)
            {
                level.DeepCopy(levelScriptable.level);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelNum"></param>
        protected void CreateMap(int levelNum)
        {

        }

        public void GenerateTile(int width, int height)
        {
            /*
            for (int i = TileRoot.transform.childCount - 1; i >= 0; i--)
            {
                var obj = TileRoot.transform.GetChild(i);
                DestroyObjectInEditor(obj.gameObject);
            }

            int count = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var adress = y * height + x;
                    var index = count % 2;
                    var bg = Instantiate<GameObject>(TileBG[index], TileRoot.transform);
                    bg.transform.localPosition = new Vector3(x, y, 0);

                    count++;
                }
            }
            */
        }



        void DestroyObjectInEditor(GameObject obj)
        {
            if (obj != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    // 에디터 모드에서 객체 삭제
                    DestroyImmediate(obj);
                }
                else
                {
                    // 플레이 모드에서 객체 삭제
                    Destroy(obj);
                }
#else
            // 플레이 모드에서 객체 삭제
            Destroy(obj);
#endif
            }
        }
    }
}