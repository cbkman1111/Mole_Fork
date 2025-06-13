using System.Diagnostics;
using Common.Global;
using Common.Scene;
using UI.Menu;
using UnityEngine;
using Common.Utils;

namespace Scenes
{
    public class SceneBundle : SceneBase
    {
        private UIMenuBundle menu = null;
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            menu = UIManager.Instance.OpenMenu<UIMenuBundle>();
            if (menu != null)
            {
                menu.InitMenu();
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //var textAsset = ResourcesManager.Instance.LoadInBuild<TextAsset>("TableJsonTest");
            //var jsonString = textAsset.ToString();
            //JSONObject json = new JSONObject(jsonString); // 지난 시간 : 00:00:04.7732221 - http://www.opensource.org/licenses/lgpl-2.1.php
            //JObject json = JObject.Parse(jsonString); // 지난 시간 : 00:00:01.3933306 - Newton
            //var json = JsonMapper.ToObject(jsonString); // 지난 시간 : 00:00:02.2555544 - LitJson
            
            stopwatch.Stop();
            GiantDebug.Log($"지난 시간 : {stopwatch.Elapsed}");
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