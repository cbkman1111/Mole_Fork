using Common.Global;
using Common.Scene;
using Creature;
using UI.Menu;
using UI.Popup;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Giant.Shooting
{
    public class SceneShooting : SceneBase
    {
        private UIMenuShooting Menu = null;

        [SerializeField] private Human Player = null;
        [SerializeField] private Map Map = null;

        public override bool Init(JSONObject param)
        {
            Menu = UIManager.Instance.OpenMenu<UIMenuShooting>();
            Menu.InitMenu();
            Menu.Joystick.OnMove = OnMove;
            Menu.Joystick.OnStop = OnStop;

            Map.Init();
            Map.OnTeleport = OnTelepotMap;
            return true;
        }

        private void OnTelepotMap(int id)
        {
            if (UIManager.Instance.FindPopup<UIPopupDungeonEnter>() == true)
                return;

            var popup = UIManager.Instance.OpenPopup<UIPopupDungeonEnter>();
            if (popup != null && popup.Init(id) == true)
            {
                popup.OnEnter = LoadMap;
            }
        }

        private void LoadMap(int id)
        {
            var map = ResourcesManager.Instance.LoadInBuild<Map>("Assets/AddressableAssets/Prefab/Map/MapDungeon_0001.prefab");
        }

        public void OnMove(Vector3 angle, float f)
        {
            // 탑뷰 시점으로 변환.
            angle.z = angle.y;
            IMove moveAble = Player as IMove;
            if (moveAble != null)
            {
                moveAble.Move(angle);
            }
        }

        public void OnDash(Vector3 angle)
        {
            // 탑뷰 시점으로 변환.
            angle.z = angle.y;
            IMove moveAble = Player as IMove;
            if (moveAble != null)
            {
                moveAble.Dash(angle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnStop()
        {
            IMove moveAble = Player as IMove;
            if (moveAble != null)
            {
                moveAble.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public override void OnTouchBean(Vector3 position)
        {
            if (Menu == null || Menu.Joystick == null)
                return;

            Menu.Joystick.TouchBegin(position);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public override void OnTouchEnd(Vector3 position)
        {
            if (Menu == null || Menu.Joystick == null)
                return;

            Menu.Joystick.TouchEnd(position);

            if (EventSystem.current.IsPointerOverGameObject() == true)
            {
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public override void OnTouchMove(Vector3 position, Vector2 deltaPosition)
        {
            if (Menu == null || Menu.Joystick == null)
                return;

            Menu.Joystick.TouchMove(position);
        }
    }
}

