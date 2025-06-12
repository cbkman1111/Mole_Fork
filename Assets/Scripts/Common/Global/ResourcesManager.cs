using System.Linq;
using Common.Global.Singleton;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Collections.Generic;

namespace Common.Global
{
    public class ResourcesManager : MonoSingleton<ResourcesManager>
    {
        //string path = "Assets/AssetBundles/AssetBundles";
        private string AddressableAssetPath = "Assets/AddressableAssets";
        //private string LocalPath = "Local";
        //private string RemotePath = "Remote";
        //private AssetBundle bundle = null;

        protected override bool Init()
        {
            Addressables.InitializeAsync();

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
            //if(bundle == null)
            //    bundle = AssetBundle.LoadFromFile(path);
            //return bundle != null;

            return true;
        }

        public bool ExistsInAddressablesRemote(string key)
        {
            IList<IResourceLocation> locations;
            bool found = Addressables.ResourceLocators.Any(locator => locator.Locate(key, typeof(UnityEngine.Object), out locations));
            return found;
        }

        public T LoadAddressable<T>(string path) where T : Object
        {
            string assetPath = $"{AddressableAssetPath}/{path}";
            var asyncOperation = Addressables.LoadAssetAsync<T>(assetPath);
            T obj = asyncOperation.WaitForCompletion();
            return obj;
        }

        public T InstantiateAsync<T>(string path) where T : Object
        {
            string assetPath = $"{AddressableAssetPath}/{path}";
            var asyncOperation = Addressables.InstantiateAsync(assetPath);
            GameObject go = asyncOperation.WaitForCompletion();
            if (go == null)
                return null;

            return go.GetComponent<T>();
        }
        public T[] LoadAddressableAll<T>(string label) where T : Object
        {
            var asyncOperation = Addressables.LoadAssetsAsync<T>(label, null);
            T[] array = asyncOperation.WaitForCompletion().ToArray();
            return array;
        }
        public T LoadResources<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }

        /*
        public T[] LoadInBuildAllI<T>(string path) where T : Object
        {
            return Resources.LoadAll<T>(path);
        }


        public T LoadInBuild<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }

        public T LoadBundle<T>(string path) where T : Object
        {
            if(bundle == null)
                return default;

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

        */
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
          return handle
        }
        */

    }
}
