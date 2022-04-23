using Singleton;
using UnityEngine;

public class ResourcesManager : MonoSingleton<ResourcesManager>
{
    public override bool Init()
    {
        gameObject.name = string.Format("singleton - {0}", TAG);
        return true;
    }

    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }
    
    public T[] LoadAll<T>(string path) where T : Object
    {
        return Resources.LoadAll<T>(path);
    }
}
