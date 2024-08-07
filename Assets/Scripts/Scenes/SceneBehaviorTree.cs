using Common.Global;
using Common.Scene;
using System;
using UI.Menu;

namespace Scenes
{
    public class SceneBehaviorTree : SceneBase
    {
        private UIMenuBehaviorTree menu = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            menu = UIManager.Instance.OpenMenu<UIMenuBehaviorTree>();
            if (menu != null)
            {
                menu.InitMenu();
            }

            return true;
        }

        /// <summary>
        /// 미리 로딩해야 할 데이터 처리.
        /// </summary>
  
        public async override void Load(Action<float> update)
        {
            //Amount = 1f;


            update(0.1f);
        }
   
    }
}