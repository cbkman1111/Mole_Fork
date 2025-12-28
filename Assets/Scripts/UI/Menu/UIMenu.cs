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
        public bool InitMenu()
        {
            return true;
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
            if (name == "Button - TileMap")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneTileMap);
            }
            else if (name == "Button - Gostop")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneGostop);
            }
            else if (name == "Button - AntHouse")
            {
                JSONObject jsonParam = new JSONObject();
                jsonParam.SetField("map_no", 3);
                AppManager.Instance.ChangeScene(Common.Scenes.SceneAntHouse, param: jsonParam);
            }
            else if (name.CompareTo("Button - Start5") == 0)
            {
                AppManager.Instance.ChangeScene(Common.Scenes.Game);
            }
            else if (name.CompareTo("Button - ChattScroll") == 0)
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneChatScroll);
            }
            else if (name.CompareTo("Button - AddressableBundle") == 0)
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneBundle);
            }
            else if (name == "Button - Maze")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneMaze);
            }
            else if (name == "Button - ExcelData")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneTextRpg);
            }
            else if (name == "Button - Puzzle")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.ScenePuzzle);
            }
            else if (name == "Button - Tetris")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneTetris);
            }
            else if (name == "Button - TextRpg")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneTextRpg);
            }
            else if (name == "Button - Dotween")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneDotween);
            }
            else if (name == "Button - 3Match")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneMatch3);
            }
            else if (name == "Button - Test")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneTest);
            }
            else if (name == "Button - Hash")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneHash);
            }
            else if (name == "Button - Loading")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneLoading);
            }
            else if (name == "Button - AdMob")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneAdMob);
            }
            else if (name == "Button - Pocker")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.ScenePoker);
            }
            else if (name == "Button - Test")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneTest);
            }
            else if (name == "Button - Shooting")
            {
                AppManager.Instance.ChangeScene(Common.Scenes.SceneShooting);
            }
        }
    }
}
