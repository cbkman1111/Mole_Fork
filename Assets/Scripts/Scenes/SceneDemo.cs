using System;
using System.Collections.Generic;
using System.Diagnostics;
using Common.Global;
using Common.Scene;
using LitJson;
using UI.Menu;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using Debug = UnityEngine.Debug;

namespace Scenes
{
    public class SceneDemo : SceneBase
    {
        private UIMenuDemo menu = null;
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            menu = UIManager.Instance.OpenMenu<UIMenuDemo>("UIMenuDemo");
            if (menu != null)
            {
                menu.InitMenu();
            }

            /*
            MEC.Timing.RunCoroutine(BundleCheck());

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var textAsset = ResourcesManager.Instance.LoadInBuild<TextAsset>("TableJsonTest");
            var jsonString = textAsset.ToString();
            
            //JSONObject json = new JSONObject(jsonString); // 지난 시간 : 00:00:04.7732221 - http://www.opensource.org/licenses/lgpl-2.1.php
            JObject json = JObject.Parse(jsonString); // 지난 시간 : 00:00:01.3933306 - Newton
            //var json = JsonMapper.ToObject(jsonString); // 지난 시간 : 00:00:02.2555544 - LitJson
            
            stopwatch.Stop();
            Debug.Log($"지난 시간 : {stopwatch.Elapsed}");
            */
            return true;
        }
    }
}