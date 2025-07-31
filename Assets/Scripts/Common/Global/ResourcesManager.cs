using System.Collections.Generic;
using Common.Global.Singleton;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Common.Global
{
    public class ResourcesManager : MonoSingleton<ResourcesManager>
    {
        //string path = "Assets/AssetBundles/AssetBundles";
        //private AssetBundle bundle = null;
        //private Dictionary<string, AsyncOperationHandle<GameObject>> Handle;

        protected override bool Init()
        {
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
            Addressables.InitializeAsync();
            return true;//bundle != null;
        }

        public T LoadInBuild<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }
    
        public T[] LoadnBuildAllI<T>(string path) where T : Object
        {
            return Resources.LoadAll<T>(path);
        }

        /// <summary>
        /// 동기시긍로 주소로 오브젝트를 생성합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <param name="parent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public T InstantiateAsync<T>(string address, Transform parent, Vector3 position, Quaternion rotation) where T : Object
        {
            var op = Addressables.InstantiateAsync(address, position, rotation, parent);
            var obj = op.WaitForCompletion();

            if (obj != null)
                return obj.GetComponent<T>();
            else
                Addressables.Release(op);
            
            return default;
        }

        /// <summary>
        /// 주소로 오브젝트를 동기적으로 로드합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadBundle<T>(string path) where T : Object
        {
            var op = Addressables.LoadAssetAsync<T>(path);
            if (op.Status == AsyncOperationStatus.Succeeded)
                return op.WaitForCompletion();
            else
                Addressables.Release(op); 

            return default;
        }
    }
}
