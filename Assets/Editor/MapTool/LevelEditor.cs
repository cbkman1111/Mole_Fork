using Common.Global;
using SweetSugar.Scripts.Level;
using UnityEditor;
using UnityEngine;

namespace Match3
{
    /// <summary>
    /// 탭.
    /// </summary>
    public enum TabTypes
    {
        Direction = 0,
        Candy,
        Block,
        Teleport,
        Generate,

        Max,
    }

    /// <summary>
    /// 맵 에디터.
    /// </summary>
    public partial class LevelEditor : EditorWindow
    {
        private const string MenuNameRoot = "매치3/스테이지 에디터";
        private const string MenuNameOpen = MenuNameRoot + "/열기";

        private Level level { get; set; } = null;
        
        private string[] tabs = { "방향", "캔디", "블럭", "텔레포트", "생성" };
        private int TabIndex = (int)TabTypes.Block;
        private int SelectCandy = (int)CandyTypes.None;
        private int SelectBlock = (int)BlockTypes.None;
        private bool SelectBlockDelete = false; // 채우기, 제거, 지우기.

        private int SelectDirection = (int)DirectionTypes.Down;
        private int CurrLayer = 0;
        //private int Row = 9;
        //private int Col = 9;
        //private int Stage = 1;

        private Texture[] candyIcons = null;
        private Texture[] blockIcons = null;
        private Texture[] directions = null;

        /// <summary>
        /// 매치3 -> 스테이지 에디터 -> 열기
        /// </summary>
        [MenuItem(MenuNameOpen)]
        private static void Open()
        {
            var editorWindow = GetWindow(typeof(LevelEditor));
            editorWindow.titleContent = new GUIContent("스테이지 에디터");
            
            LevelEditor levelEditor = editorWindow as LevelEditor;
            levelEditor.Init();
        }

        /// <summary>
        /// 초기화.
        /// </summary>
        public void Init()
        {
            level = new();
            candyIcons = new Texture[(int)CandyTypes.Max];
            blockIcons = new Texture[(int)BlockTypes.Max];
            directions = new Texture[(int)DirectionTypes.Max];

            TabIndex = (int)TabTypes.Block;
            SelectCandy = (int)CandyTypes.None;
            SelectBlock = (int)BlockTypes.None;
            
            string pathItems = "Assets/SweetSugar/Textures_png/Items";
            candyIcons[0] = null;
            candyIcons[1] = (Texture)AssetDatabase.LoadAssetAtPath($"{pathItems}/item_01.png", typeof(Texture));
            candyIcons[2] = (Texture)AssetDatabase.LoadAssetAtPath($"{pathItems}/item_02.png", typeof(Texture));
            candyIcons[3] = (Texture)AssetDatabase.LoadAssetAtPath($"{pathItems}/item_03.png", typeof(Texture));
            candyIcons[4] = (Texture)AssetDatabase.LoadAssetAtPath($"{pathItems}/item_04.png", typeof(Texture));
            candyIcons[5] = (Texture)AssetDatabase.LoadAssetAtPath($"{pathItems}/item_05.png", typeof(Texture));
            candyIcons[6] = (Texture)AssetDatabase.LoadAssetAtPath($"{pathItems}/item_06.png", typeof(Texture));

            blockIcons[0] = null;
            blockIcons[1] = (Texture)AssetDatabase.LoadAssetAtPath($"{pathItems}/item_spot_01.png", typeof(Texture));
            blockIcons[2] = (Texture)AssetDatabase.LoadAssetAtPath($"{pathItems}/game_item_a_1.png", typeof(Texture));

            string pathEditor = "Assets/SweetSugar/Textures_png/EditorSprites";
            directions[(int)DirectionTypes.Down] = (Texture)AssetDatabase.LoadAssetAtPath($"{pathEditor}/arrow.png", typeof(Texture));
            directions[(int)DirectionTypes.Up] = (Texture)AssetDatabase.LoadAssetAtPath($"{pathEditor}/arrow_up.png", typeof(Texture));
            directions[(int)DirectionTypes.Left] = (Texture)AssetDatabase.LoadAssetAtPath($"{pathEditor}/arrow_left.png", typeof(Texture));
            directions[(int)DirectionTypes.Right] = (Texture)AssetDatabase.LoadAssetAtPath($"{pathEditor}/arrow_right.png", typeof(Texture));
        }

        /// <summary>
        /// 편집된 레벨 정보 에셋 저장.
        /// </summary>
        /// <param name="Stage"></param>
        public void SaveLevel()
        {
            //var levelScriptable = ResourcesManager.Instance.LoadInBuild<LevelContainer>("Level/Level_" + level.Stage);
            var levelScriptable = Resources.Load("Level/Level_" + level.Stage) as LevelContainer;
            if (levelScriptable != null)
            {
                levelScriptable.SetData(level);
                EditorUtility.SetDirty(levelScriptable);
            }
            else 
            {
                var path = "Assets/Scenes/Scene3Match/Resources/Level/";
                string fileName = "Level_" + level.Stage;
                var newLevelData = ScriptableObjectUtil.CreateAsset<LevelContainer>(path, fileName);
                newLevelData.SetData(level);
                EditorUtility.SetDirty(newLevelData);
                AssetDatabase.SaveAssets();
            }

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelNum"></param>
        public void LoadLevel(int levelNum)
        {
            //var levelScriptable = ResourcesManager.Instance.LoadInBuild<LevelContainer>("Level/Level_" + levelNum);
            var levelScriptable = Resources.Load<LevelContainer>("Level/Level_" + levelNum);// as LevelContainer;
            if (levelScriptable)
            {
                level.DeepCopy(levelScriptable.level);

                //Col = level.Col;
                //Row = level.Row;
                //Stage = level.Stage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
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