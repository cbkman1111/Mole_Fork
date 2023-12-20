using Common.Global;
using Common.Scene;
using Common.UIObject;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenu : MenuBase
    {
        public override void OnInit()
        {

        }

        public bool InitMenu()
        {
            return true;
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
            else if (name == "Button - Spine")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.Demo);
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
        }
    }
}
