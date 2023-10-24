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
    public class SceneBundle : SceneBase
    {
        private UIMenuBundle menu = null;
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Init(JSONObject param)
        {
            menu = UIManager.Instance.OpenMenu<UIMenuBundle>("UI/UIMenuBundle");
            if (menu != null)
            {
                menu.InitMenu();
            }

            // 
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
            return true;
        }

        private IEnumerator<float> BundleCheck()
        {
            string key = "Test";
            Addressables.ClearDependencyCacheAsync(key);
            //Addressables.CleanBundleCache();
                
            //ddressables.ClearResourceLocators(); // 먼가 다 제거되서 객체 생성 안되게됨. 
            bool checkComplete = false;
            List<string> catalogsToUpdate = new List<string>();
            Addressables.CheckForCatalogUpdates().Completed += (op) =>
            {            
                if (op.Result.Count > 0)
                {
                    catalogsToUpdate.AddRange(op.Result);
                }
                
                checkComplete = true;
            };
            
            //AsyncOperationHandle<List<string>> checkHandle = Addressables.CheckForCatalogUpdates();
            if(checkComplete == false)
                yield return MEC.Timing.WaitForOneFrame;
            
            if (catalogsToUpdate.Count > 0)
            {  
                Debug.LogError("Available Update");   
                AsyncOperationHandle<List<IResourceLocator>> updateHandle = Addressables.UpdateCatalogs(catalogsToUpdate);
                if(updateHandle.IsDone == false)
                    yield return MEC.Timing.WaitForOneFrame;
            }
            else
            {
                Debug.LogError("No Available Update");
            }

            var sizeCheck = Addressables.GetDownloadSizeAsync(key);
            if(sizeCheck.IsDone == false)
                yield return MEC.Timing.WaitForOneFrame;

            if (sizeCheck.Result > 0)
            {      
                string sizeText = string.Concat(sizeCheck.Result, " byte");
                var downloadDependencies = Addressables.DownloadDependenciesAsync(key);
                if(downloadDependencies.IsDone == false)
                    yield return MEC.Timing.WaitForOneFrame;
            }
            
            

        }

        
        public override void OnTouchBean(Vector3 position)
        {

        }

        public override void OnTouchEnd(Vector3 position)
        {

        }

        public override void OnTouchMove(Vector3 position)
        {

        }
    }
}