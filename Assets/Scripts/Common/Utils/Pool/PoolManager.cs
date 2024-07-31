using System.Collections.Generic;
using Common.Global.Singleton;
using UnityEngine;

namespace Common.Utils.Pool
{
    public class PoolObject
    {
        public Transform prefab;
    }

    public class PoolManager : MonoSingleton<PoolManager>
    {
        public Dictionary<string, Pool<Transform>> dic = null;
        
        protected override bool Init()
        {
            dic = new Dictionary<string, Pool<Transform>>();
            return true;
        }
        
        public void InitList(params Transform[] list)
        {
            dic.Clear();
            foreach (var prefab in list)
            {
                var key = prefab.name;
                dic.Add(key, Pool<Transform>.Create(prefab, transform, 1));
            }
        }

        public Transform GetObject(string key)
        {
            if (dic.TryGetValue(key, out Pool<Transform> pool) == true)
            {
                return pool.GetObject();
            }

            return null;
        }

        public void ReturnObject(Transform obj)
        {
            if (dic.TryGetValue(obj.name, out Pool<Transform> pool) == true)
            {
                pool.ReturnObject(obj);
            }
        }

        public void RemoveAll()
        {
            dic.Clear();
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject) ;
            }
        }

        public int GetObjectCount(string key)
        {
            if (dic.TryGetValue(key, out Pool<Transform> pool) == true)
            {
                return pool.ActiveCount();
            }

            return 0;
        }
    }

}