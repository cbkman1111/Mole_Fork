using System.Linq;
using Common.Global;
using Common.Scene;
using Common.Table;
using Common.Utils;
using Creature;
using Giant.Camera;
using UI.Menu;
using UI.Popup;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.EventSystems;
using static Creature.WorldObject;

namespace Giant.Shooting
{
    public class SceneShooting : SceneBase
    {
        private UIMenuShooting Menu = null;

        private Human Player = null;

        [SerializeField] private Map Map = null;
        [SerializeField] private Map MapDungeon = null;

        private WorldObjectList listCretures = new();
        private WorldObjectList listProbs = new();

        private TableHero tableHeros = null;
        [SerializeField] private Transform Rocks;
        [SerializeField] private Transform Woods;
        [SerializeField] private Transform Creatrues;

        public override bool Init(JSONObject json)
        {
            DataManager.Instance.Load();

            Menu = UIManager.Instance.OpenMenu<UIMenuShooting>();
            Menu.InitMenu();
            Menu.Joystick.OnMove += OnMove;
            Menu.Joystick.OnStop += OnStop;
            Menu.OnSpeedModify += OnSpeedModify;

            Map.Init();
            Map.OnTeleport += OnTelepotMap;

            tableHeros = DataManager.Instance.Get<TableHero>();
            var heroTable = tableHeros.Data.First();
            var key = heroTable.Key;
            var table = heroTable.Value;

            WorldObjectCreateParam param;
            param.Path = $"{table.PREFAB}";
            param.Parent = Creatrues;
            param.X = 0;
            param.Z = 0;
            param.ObjectTeam = ObjectTeam.Enemy;

            Player = WorldObject.Create(param) as Human;
            Player.transform.SetParent(Creatrues);
            listCretures.Add(Player);

            var cretures = listCretures.List();
            var probs = listProbs.List();
            foreach (var obj in listCretures)
            {
                var worldObj = obj.GetComponent<WorldObject>();
                if (worldObj != null)
                {
                    worldObj.BlackboardReference.SetVariableValue("Creatures", cretures);
                    worldObj.BlackboardReference.SetVariableValue("Probs", probs);
                }
            }

            var cameraControl = MainCamera.GetComponent<Giant.Camera.CameraControl>();
            cameraControl.SetBounds(MainCamera, Player, Map.CameraArea);
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
            MapDungeon.Init();
            MapDungeon.OnTeleport += OnTelepotHome;

            var cameraControl = MainCamera.GetComponent<Giant.Camera.CameraControl>();
            cameraControl.SetBounds(MainCamera, Player, MapDungeon.CameraArea);

            loading.Close();
        }

        private void OnTelepotHome(int id)
        {
            Map.SetActive(true);
            
            var cameraControl = MainCamera.GetComponent<Giant.Camera.CameraControl>();
            cameraControl.SetBounds(MainCamera, Player, Map.CameraArea);

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

