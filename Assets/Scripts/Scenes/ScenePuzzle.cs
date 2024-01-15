using Common.Global;
using Common.Scene;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class ScenePuzzle : SceneBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            UIMenuPuzzle menu = UIManager.Instance.OpenMenu<UIMenuPuzzle>("UIMenuPuzzle");
            if (menu != null)
            {
                menu.InitMenu();
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {

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