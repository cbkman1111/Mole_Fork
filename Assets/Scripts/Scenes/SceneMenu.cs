using Common.Global;
using Common.Scene;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneMenu : SceneBase
    {
        public override bool Init(JSONObject param)
        {
            UIMenu menu = UIManager.Instance.OpenMenu<UIMenu>("UIMenu");
            if (menu != null)
            {
                menu.InitMenu();
            }


            SoundManager.Instance.Load();
            DataManager.Instance.LoadTables();

            SoundManager.Instance.PlayMusic("17856_1462216818");
            return true;
        }

        public override void OnTouchBean(Vector3 position)
        {
            SoundManager.Instance.PlayEffect("EFF_shoot");
        }

        public override void OnTouchEnd(Vector3 position)
        {
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
