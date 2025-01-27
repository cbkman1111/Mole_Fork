using Common.Global;
using Common.Scene;
using Common.UIObject;
using Common.Utils;
using DG.Tweening;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenu : MenuBase
    {
        public enum eContents
        {
            Main,
            Shop,
            Upgrade,
            Gacha,
            Max,
        }

        public GameObject[] Contents = new GameObject[(int)eContents.Max];
        public eContents Content = eContents.Main;

        public bool InitMenu()
        {

            SetContent(eContents.Main);
            return true;
        }

        private void SetContent(eContents content)
        {
            Content = content;

            var screenW = Screen.width;
            
            for (int i = 0; i < (int)eContents.Max; i++)
            {
                if (i == (int)Content)
                {
                    Contents[i].transform.localPosition = new Vector3(screenW, 0, 0);
                    Contents[i].transform.DOLocalMoveX(0, 0.5f);
                    
                }
                else
                {
                    //Contents[i].SetActive(false);
                    Contents[i].transform.DOLocalMoveX(-screenW, 0.5f);
                }
            }
        }

        private void Foo()
        {
            long[] numbers = {
                0, 1, 12, 123, 1234, 12345, 123456, 1234567, 12345678, 123456789, 1234567890, 12345678901, 123456789012,
            };

            var cutureInfos = new System.Globalization.CultureInfo[]
            {
                new System.Globalization.CultureInfo("ko-KR"),
                new System.Globalization.CultureInfo("en-US"),
                new System.Globalization.CultureInfo("fr-FR")
            };

            foreach (var n in numbers)
            {
                //AppManager.Instance.CultureInfo = cultureInfo;
                //var str = number.ToString("#,###", AppManager.Instance.CultureInfo);
                //var str = number.ToString("##,##0.00", AppManager.Instance.CultureInfo);
                var str1 = n.ToString("##,##0");
                //var str2 = n.ToString("##,##0");
                GiantDebug.Log($"CultureInfo : 입력 = {n} --> 출력 = {str1}");
                //GiantDebug.Log($"CultureInfo : str2 = {str2}");
            }
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if (name == "Button - Start1")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneTileMap);
            }
            else if (name == "Button - Start2")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneGostop);
            }
            else if (name == "Button - Test")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneTest);
            }
            else if (name == "Button - Start4")
            {
                JSONObject jsonParam = new JSONObject();
                jsonParam.SetField("map_no", 3);
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneAntHouse, param: jsonParam);
            }
            else if (name.CompareTo("Button - Start5") == 0)
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.game);
            }
            else if (name.CompareTo("Button - ChattScroll") == 0)
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneChatScroll);
            }
            else if (name.CompareTo("Button - Bundle") == 0)
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneBundle);
            }
            else if (name == "Button - Maze")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMaze);
            }
            else if (name == "Button - Behavior")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneBehaviorTree);
            }
            else if (name == "Button - Puzzle")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.ScenePuzzle);
            }
            else if (name == "Button - Tetris")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneTetris);
            }
            else if (name == "Button - TextRpg")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneTextRpg);
            }
            else if (name == "Button - Dotween")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneDotween);
            }
            else if (name == "Button - 3Match")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMatch3);
            }
            else if (name == "Button - Test")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneTest);
            }
            else if (name == "Button - Hash")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneHash);
            }
            else if (name == "Button - AdMob")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneAdMob);
            }
            else if (name == "Button - Pocker")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.ScenePoker);
            }
            else if (name == "Button-Shop")
            {
                SetContent(eContents.Shop);
            }
            else if (name == "Button-Upgrade")
            {
                SetContent(eContents.Upgrade);
            }
            else if (name == "Button-Gacha")
            {
                SetContent(eContents.Gacha);
            }
            else if (name == "Button-Main")
            {
                SetContent(eContents.Main);
            }
        }
    }
}
