using System.Collections.Generic;
using Common.Global.Singleton;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Common.Global
{
    public class ResourcesManager : MonoSingleton<ResourcesManager>
    {
        string path = "Assets/AssetBundles/bundle";
        private AssetBundle bundle = null;
        List<Object> list = null;

        protected override bool Init()
        {
            list = new List<Object>();

            //Load();
            
            //Addressables.InitializeAsync();
            
            return true;
        }
        
        public bool Load()
        {
            //AssetBundle.LoadFromMemory (Async optional)
            //AssetBundle.LoadFromFile (Async optional)
            //AssetBundle.LoadFromStream (Async optional)
            //UnityWebRequest's DownloadHandlerAssetBundle
            //WWW.LoadFromCacheOrDownload (on Unity 5.6 or older)
            //AssetBundleManifest manifest = (AssetBundleManifest)ab.LoadAsset("AssetBundleManifest");
            bundle = AssetBundle.LoadFromFile(path);

            return bundle != null;
        }

        public T LoadInBuild<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }
    
        public T[] LoadAllInBuild<T>(string path) where T : Object
        {
            return Resources.LoadAll<T>(path);
        }

        public T LoadBundle<T>(string path) where T : Object
        {
            T res = bundle.LoadAsset<T>(path);
            if(res != null)
            {
                return res;
            }

            GameObject obj = bundle.LoadAsset<GameObject>(path);
            if(obj != null)
            {
                return obj.GetComponent<T>();
            }

            return default;
        }

        public T[] LoadBudleAll<T>() where T : Object
        {
            return bundle.LoadAllAssets<T>();
        }

        public T[] LoadBudleAll<T>(string path) where T : Object
        {
            return bundle.LoadAssetWithSubAssets<T>(path);
        }

        /*
        public static AsyncOperationHandle<GameObject> InstantiateAsync(string path, Transform parent, Vector3 position, Quaternion rotation, bool isLocalRes = false)
        {
          return null;

          var rootPath = isLocalRes == false ? RemoteRootPath : LocalRootPath;
          var handle = Addressables.InstantiateAsync($"{rootPath}{path}", position, rotation, parent);

#if UNITY_EDITOR
          if (handle.IsValid())
              handle.Completed += (op => 
              {
                  GameObject go = op.Result;
              });
#endif
          return handle;
  
        }
        */

    }
}
