using Singleton;
using UnityEngine;

public class ResourcesManager : MonoSingleton<ResourcesManager>
{
    public override bool Init()
    {
        gameObject.name = string.Format("singleton - {0}", TAG);
        return true;
    }

    public GameObject Load(string path) 
    {
        GameObject ret = Resources.Load(path) as GameObject;
        return ret;
    }
}
