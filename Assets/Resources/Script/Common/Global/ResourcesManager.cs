using Singleton;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourcesManager : MonoSingleton<ResourcesManager>
{
    string path = "Assets/AssetBundles/bundle";
    private AssetBundle bundle = null;
    List<Object> list = null;

    public override bool Init()
    {
        gameObject.name = string.Format("singleton - {0}", TAG);
        list = new List<Object>();

        return true;
    }
    public bool Load()
    {
        bundle = AssetBundle.LoadFromFile(path);

        return bundle != null;
    }

    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }
    
    public T[] LoadAll<T>(string path) where T : Object
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
}
