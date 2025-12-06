using Common.Global;
using Common.Scene;
using Common.Utils;
using Creature;
using Giant.Camera;
using NPOI.SS.Formula.Functions;
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
        [SerializeField] private Map MapDungeon = null;

        public override bool Init(JSONObject param)
        {
            Menu = UIManager.Instance.OpenMenu<UIMenuShooting>();
            Menu.InitMenu();
            Menu.Joystick.OnMove += OnMove;
            Menu.Joystick.OnStop += OnStop;
            Menu.OnSpeedModify += OnSpeedModify;

            Map.Init();
            Map.OnTeleport += OnTelepotMap;

            var cameraControl = MainCamera.GetComponent<Giant.Camera.CameraControl>();
            cameraControl.SetBounds(MainCamera, Map.CameraArea);
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
            string path = "Assets/AddressableAssets/Prefab/Map/MapDungeon_0001.prefab";

            UIManager.Instance.ClosePopup<UIPopupDungeonEnter>();
            var loading = UIManager.Instance.OpenPopup<UIPopupLoading>();

            Map.SetActive(false);

            MapDungeon = ResourcesManager.Instance.InstantiateAsync<Map>(path, null, Vector3.zero, Quaternion.identity);
            if (MapDungeon == null)
            {
                Debug.LogError($"[LoadMap] Failed to load map: {path}");
                loading.Close(); // 실패해도 닫아줘야 함
                return;
            }

            MapDungeon.Init();
            MapDungeon.OnTeleport += OnTelepotHome;

            var cameraControl = MainCamera.GetComponent<Giant.Camera.CameraControl>();
            cameraControl.SetBounds(MainCamera, MapDungeon.CameraArea);

            loading.Close();
        }

        private void OnTelepotHome(int id)
        {
            Map.SetActive(true);
            
            var cameraControl = MainCamera.GetComponent<Giant.Camera.CameraControl>();
            cameraControl.SetBounds(MainCamera, Map.CameraArea);

            Player.transform.position = Map.Teleports[0].transform.position;

            Destroy(MapDungeon.gameObject);
            MapDungeon = null;
        }

        private void OnSpeedModify(int speed)
        {
            IMove moveAble = Player as IMove;
            if (moveAble != null)
            {
                Player.Stat.SetStat(Stat.StatType.Speed, speed);
            }
        }

        public void OnMove(Vector3 angle, float f)
        {
            //GiantDebug.Log($"OnMove: {angle}, f: {f}");
            // 탑뷰 시점으로 변환.
            angle.z = angle.y;
            IMove moveAble = Player as IMove;
            if (moveAble != null)
            {
                moveAble.Move(angle);
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

