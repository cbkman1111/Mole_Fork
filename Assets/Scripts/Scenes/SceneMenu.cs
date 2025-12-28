using System;
using Common.Global;
using Common.Scene;
using Common.Utils;
using Network;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneMenu : SceneBase
    {
        public override bool Init(JSONObject param)
        {
            UIMenu menu = UIManager.Instance.OpenMenu<UIMenu>();
            if (menu != null)
            {
                menu.InitMenu();
            }

            SoundManager.Instance.StopMusics();
            SoundManager.Instance.StopEffect();

            SoundManager.Instance.PlayMusic("17856_1462216818");
            return true;
        }

        public override void Load(Action<float> update)
        {
            try
            {
                SoundManager.Instance.Load();
                DataManager.Instance.Load();
                //ResourcesManager.Instance.Load();

            }
            catch (System.Exception e)
            {
                // handled below
                GiantDebug.LogError($"{name} - {e.ToString()}");
            }

            update(1f);
        }

        public override void UnLoad()
        {
            
        }
        public override void OnTouchBean(Vector3 position)
        {
            SoundManager.Instance.PlayEffect("EFF_shoot");
            //NetworkManager.Instance.Send("Hello from client");
        }

        public override void OnTouchEnd(Vector3 position)
        {
            if (MainCamera == false)
                return;

            var world = MainCamera.ScreenToWorldPoint(position);
            Ray ray = MainCamera.ScreenPointToRay(world);
            //RaycastHit2D hit = Physics2D.Raycast(world, transform.forward);
            //if (hit.collider != null)
            //{
            //}
            //Debug.DrawRay(world, transform.forward * 100, Color.red, 1.3f);
        }

        public override void OnTouchMove(Vector3 position, Vector2 deltaPosition)
        {

        }
    }
}
