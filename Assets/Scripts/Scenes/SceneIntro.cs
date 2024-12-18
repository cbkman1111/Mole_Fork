using Common.Global;
using Common.Scene;
using Common.Utils;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneIntro : SceneBase
    {
        public override bool Init(JSONObject param)
        {
            GiantDebug.Log($"SceneIntro init. start.");

            UIMenuIntro menu = UIManager.Instance.OpenMenu<UIMenuIntro>();
            GiantDebug.Log($"SceneIntro init. {menu}");

            if(menu == null)
            {
                GiantDebug.LogError($"{tag} menu is null.");
                return false;
            }
            
            menu.InitMenu();
            return true;
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

