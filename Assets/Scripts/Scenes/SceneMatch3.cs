using Common.Global;
using Common.Scene;
using Match3;
using System;
using UI.Menu;
using UnityEditor;
using UnityEngine;

namespace Scenes
{
    public class SceneMatch3 : SceneBase
    {
        private Level level = null;

        [SerializeField]
        private GameObject board = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            UIMenuMatch3 menu = UIManager.Instance.OpenMenu<UIMenuMatch3>();
            if (menu != null)
            {
                menu.InitMenu();
                menu.OnStartGame = (int level) => {
                };
            }

            level = new();

            var path = $"Level/Level_{1}";
            var levelScriptable = ResourcesManager.Instance.LoadInBuild<LevelContainer>(path);
            //var levelScriptable = Resources.Load("Level/Level_" + 1) as LevelContainer;
            if (levelScriptable != null)
            {
                level.DeepCopy(levelScriptable.level);
            }

            var col = level.Col;
            var row = level.Row;
            for(int i = 0; i < col; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    var square = level.field.Squares[i + j * col];

                    foreach (var blockData in square.block)
                    {
                        BlockBase blockPrefab = null;
                        switch (blockData)
                        {
                            case BlockTypes.Empty:
                                blockPrefab = ResourcesManager.Instance.LoadInBuild<BlockBase>("BlockEmpty");
                                break;
                            case BlockTypes.Sugar:
                                blockPrefab = ResourcesManager.Instance.LoadInBuild<BlockBase>("BlockSugar");
                                break;
                        }

                        if (blockPrefab != null)
                        {
                            var block = Instantiate<BlockBase>(blockPrefab, board.transform);
                            block.transform.position = new Vector3(i, -j, 0);
                        }
                    }


                    CandyColor prefab = null;
                    switch (square.candy)
                    {
                        case CandyTypes.Red:
                            prefab = ResourcesManager.Instance.LoadInBuild<CandyColor>("CandyRed");
                            break;
                        case CandyTypes.Yellow:
                            prefab = ResourcesManager.Instance.LoadInBuild<CandyColor>("CandyYellow");
                            break;
                        case CandyTypes.Green:
                            prefab = ResourcesManager.Instance.LoadInBuild<CandyColor>("CandyGreen");
                            break;
                        case CandyTypes.Purple:
                            prefab = ResourcesManager.Instance.LoadInBuild<CandyColor>("CandyPurple");
                            break;
                        case CandyTypes.Sky:
                            prefab = ResourcesManager.Instance.LoadInBuild<CandyColor>("CandySky");
                            break;
                        case CandyTypes.Orange:
                            prefab = ResourcesManager.Instance.LoadInBuild<CandyColor>("CandyOrange");
                            break;
                    }

                    if (prefab != null)
                    {
                        var candy = Instantiate<CandyColor>(prefab, board.transform);
                        candy.transform.position = new Vector3(i, -j, 0);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {

        }

        public override void OnTouchBean(Vector3 position)
        {

        }

        public override void OnTouchEnd(Vector3 position)
        {
       
        }

        public override void OnTouchMove(Vector3 position, Vector2 deltaPosition)
        {

        }
    }
}