using Common.Global;
using Common.Scene;
using UI.Menu;

namespace Scenes
{
    public class ScenePoker : SceneBase
    {
        public override bool Init(JSONObject param)
        {
            var menu = UIManager.Instance.OpenMenu<UIMenuPoker>();
            if (menu != null)
            {
                menu.InitMenu();
            }

            return true;
        }
    }
}