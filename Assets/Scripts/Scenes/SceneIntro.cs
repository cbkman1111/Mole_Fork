using Common.Global;
using Common.Scene;
using Common.Utils;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneIntro : SceneBase
    {
        [SerializeField] GameObject Cube = null;
        public float RotationSpeed = 100f;

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

        public override void OnUpdate()
        {
            float horizontal = Input.GetAxis("Horizontal");
            Cube.transform.Rotate(1,1,1);
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

