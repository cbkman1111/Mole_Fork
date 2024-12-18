using Common.Global;
using Common.Scene;
using UI.Menu;

namespace Scenes
{
    public class SceneAdMob : SceneBase
    {
        private UIMenuAdMob menu = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            menu = UIManager.Instance.OpenMenu<UIMenuAdMob>();
            if (menu != null)
            {
                menu.InitMenu();
                
            }


            return true;
        }

    }
}