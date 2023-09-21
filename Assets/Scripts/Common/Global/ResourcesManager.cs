using System.Collections.Generic;
using Common.Global.Singleton;
using UnityEngine;

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

            Load();
            return true;
        }
        public bool Load()
        {
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

            return default(T);
        }

        public T[] LoadBudleAll<T>() where T : Object
        {
            return bundle.LoadAllAssets<T>();
        }

        public T[] LoadBudleAll<T>(string path) where T : Object
        {
            return bundle.LoadAssetWithSubAssets<T>(path);
        }
    }
}
