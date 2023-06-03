using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBase : MonoBehaviour
{
    public static T Create<T>(ObjectData data, bool enableAgent = true) where T : ObjectBase
    {
        var prefab = ResourcesManager.Instance.LoadInBuild<GameObject>("Object");
        var obj = Instantiate<GameObject>(prefab);
        if (obj == null)
        {
            return default(T);
        }

        var monster = obj.AddComponent<T>();
        if (monster != null && monster.Init(data) == true)
        {
            return monster;
        }

        return default(T);
    }

    protected bool Init(ObjectData data)
    {
        if(LoadSprite() == false)
        {
            return false;
        }


        transform.position = data.position;

        return true;
    }

    protected virtual bool LoadSprite()
    {
        return true;
    }
}
