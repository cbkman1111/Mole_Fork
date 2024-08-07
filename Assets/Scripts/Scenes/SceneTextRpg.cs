using Common.Global;
using Common.Scene;
using Common.Table;
using UI.Menu;
using UnityEngine;

namespace Scenes
{
    public class SceneTextRpg : SceneBase
    {
        public override bool Init(JSONObject param)
        {
            UIMenuTextRpg menu = UIManager.Instance.OpenMenu<UIMenuTextRpg>();
            if(menu != null)
            {
                menu.InitMenu();
            }

            var table = DataManager.Instance.Get<TableTemp1>();
            if(table != null)
            {
                table.Data.ForEach(data => Debug.Log(data.Name));
            }

            var table2 = DataManager.Instance.Get<TableTemp2>();
            if(table2 != null)
            {
                table2.Data.ForEach(data => Debug.Log(data.Name));
            }

            return true;
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

