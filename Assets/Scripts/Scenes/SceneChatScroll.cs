using Common.Global;
using Common.Scene;
using Common.Utils;
using Network;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneChatScroll : SceneBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            var menu = UIManager.Instance.OpenMenu<UIMenuChat>();
            if (menu != null)
            {
                menu.InitMenu();
            }
 
            NetworkManager.Instance.Connect();
            NetworkManager.Instance.OnConnectAction = (result) =>
            {
                GiantDebug.Log("OnConnectAction");
                NetworkManager.Instance.Send("Hello");
            };
            return true;
        }

        public override void UnLoad()
        {
            NetworkManager.Instance.Destroy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public override void OnTouchBean(Vector3 position)
        {
            Ray ray = MainCamera.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                var layer = hit.collider.gameObject.layer;
                if (layer == LayerMask.NameToLayer("Tile"))
                {
                    var obj = hit.collider.gameObject;
                }

                Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 5f);
                Debug.Log(hit.point);
            }
        }

        public override void OnTouchEnd(Vector3 position)
        {

        }

        public override void OnTouchMove(Vector3 position, Vector2 deltaPosition)
        {

        }
    }
}