using Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    public override string TAG { get => "UIManager"; }
    
    private GameObject root = null;

    public override bool Init()
    {
        GameObject obj = ResourcesManager.Instance.Load("Prefabs/UI/UIRoot");
        root = Instantiate(obj, transform);
        root.name = "UIRoot";
        root.transform.position = new Vector3(100,0,0);

        gameObject.name = string.Format("singleton - {0}", TAG);
        return true;
    }

    public T OpenUI<T>(string name)
    {
        string path = string.Format("Prefabs/UI/{0}", name);

        GameObject ui = ResourcesManager.Instance.Load(path);
        Instantiate(ui, root.transform);

        T ret = gameObject.GetComponent<T>();
        return ret;
    }
}
