using Common.Global;
using Common.Scene;
using Games.TileMap;
using TileMap;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneTileMap : SceneBase
    {
        private UIMenuTileMap _menu = null;

        private Map _map = null;
        private Pige _pigeon = null;
        
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
            _map.Init(0, 0, 12, 7);
            
            // 캐릭터 생성.
            var prefab = ResourcesManager.Instance.LoadInBuild<Pige>("PigeonTemp");
            _pigeon = Object.Instantiate<Pige>(prefab);
            _pigeon.Init(0, 0);
            
            return true;
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
                MainCamera.transform.position = cameraPosition;
                
                if((int)_pigeon.transform.position.x != _pigeon.x ||
                   (int)_pigeon.transform.position.z != _pigeon.z)
                {
                    _pigeon.x = (int)_pigeon.transform.position.x;
                    _pigeon.z = (int)_pigeon.transform.position.z;
                    
                    // 새 좌표에 해당하는 타일로 업데이트.
                    _map.UpdateTiles(_pigeon.x, _pigeon.z);
                }
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