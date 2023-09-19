using System.Collections.Generic;
using Common.Global;
using Common.Scene;
using DG.Tweening;
using Games.TileMap;
using TileMap;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneTileMap : SceneBase
    {
        private static readonly System.Random random = new System.Random();
        private static readonly object synlock = new object();
        
        private UIMenuTileMap _menu = null;

        private int width = 1000;
        private int height = 1000;
        private Dictionary<int, MapTile> mapData = new Dictionary<int, MapTile>();
        
        private Map _map = null;
        private Pige _pigeon = null;

        public class MapTile
        {
            public Color Color;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            _menu = UIManager.Instance.OpenMenu<UIMenuTileMap>("UI/UIMenuTileMap");
            if (_menu != null)
            {
                _menu.InitMenu(
                    move: (Vector3 angle) => {
                        OnMove(angle);
                    },
                    stop: () => {
                        OnStop();
                    });
            }
            
            // 맵 생성.
            var prefabMap = ResourcesManager.Instance.LoadInBuild<Map>("Map");
            _map = Object.Instantiate<Map>(prefabMap);
            _map.transform.position = Vector3.zero;
            _map.Init(mapData, 50, 50, width, height, 13, 10);
            
            // 캐릭터 생성.
            var prefab = ResourcesManager.Instance.LoadInBuild<Pige>("PigeonTemp");
            _pigeon = Object.Instantiate<Pige>(prefab);
            _pigeon.Init(50, 50);
            
            // 카메라 위치 초기화.
            Vector3 cameraPosition = MainCamera.transform.position;
            cameraPosition.x = _pigeon.transform.position.x;
            cameraPosition.z = _pigeon.transform.position.z;
            MainCamera.transform.position = cameraPosition;
            return true;
        }

        public static int RandomNumber(int min, int max)
        {
            lock (synlock)
            {
                return random.Next(min, max);
            }
        }
        
        /// <summary>
        /// 미리 로딩해야 할 데이터 처리.
        /// </summary>
        public async override void Load()
        {
            int w = width;
            int h = height;
            int total = w * h;
            Color[] colors =
            {
                //Color.red,
                //Color.blue,
                //Color.green,
                //Color.cyan,
                Color.gray,
                //Color.magenta,
                Color.white,
                //Color.yellow,
            };
            
            int count = 0;
            for (int x = 0; x < w; x++)
            {
                for (int z = 0; z < h; z++)
                {
                    int adress = x + z * w;
                    
                    var rand= RandomNumber(0, colors.Length);
                   
                    MapTile info = new MapTile();
                    info.Color = colors[rand];
                    mapData.TryAdd(adress, info);
                    
                    count++;
                    Amount = (float)count / (float)total;
                }
            }

            Amount = 1f;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        public void OnMove(Vector3 angle)
        {
            // 탑뷰 시점으로 변환.
            angle.z = angle.y;
            
            _pigeon.Move(angle);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnStop()
        {
  
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnUpdate()
        {
            if (_pigeon == true)
            {
                Vector3 cameraPosition = MainCamera.transform.position;
                cameraPosition.x = _pigeon.transform.position.x;
                cameraPosition.z = _pigeon.transform.position.z;
                //MainCamera.transform.position = cameraPosition;
                MainCamera.transform.DOMove(cameraPosition, 1f);
                
                if((int)_pigeon.transform.position.x != _pigeon.x ||
                   (int)_pigeon.transform.position.z != _pigeon.z)
                {
                    _pigeon.x = (int)_pigeon.transform.position.x;
                    _pigeon.z = (int)_pigeon.transform.position.z;
                    
                    // 새 좌표에 해당하는 타일로 업데이트.
                    _map.UpdateTiles(_pigeon.x, _pigeon.z);
                }
            }
            if (Input.GetKey(KeyCode.A))
            {
                OnMove(Vector3.left);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                OnMove(Vector3.right);
            }
            else if (Input.GetKey(KeyCode.W))
            {
                OnMove(Vector3.up);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                OnMove(Vector3.down);
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                OnStop();
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                OnStop();
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                OnStop();
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                OnStop();
            }
        }

        public override void OnTouchBean(Vector3 position)
        {
            _menu.joystick.TouchBegin(position);
        }

        public override void OnTouchEnd(Vector3 position)
        {
            _menu.joystick.TouchEnd(position);

            Ray ray = MainCamera.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                var layer = hit.collider.gameObject.layer;
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Card"))
                {
             
                }
            }
        }

        public override void OnTouchMove(Vector3 position)
        {
            _menu.joystick.TouchMove(position);
        }
    }
}