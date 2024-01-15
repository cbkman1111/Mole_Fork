using Common.Global;
using Common.Scene;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneMatch3 : SceneBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            UIMenuMatch3 menu = UIManager.Instance.OpenMenu<UIMenuMatch3>("UIMenuMatch3");
            if (menu != null)
            {
                menu.InitMenu();
                menu.OnStartGame = (int level) => {
                    //LevelManager.THIS.gameStatus = GameState.PrepareGame;
                    //GUIUtils.THIS.StartGame();
                };
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