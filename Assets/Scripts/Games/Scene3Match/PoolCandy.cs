using Common.Global.Singleton;
using Common.Utils.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{/// <summary>
 /// 타일 풀 관리.
 /// </summary>
 /// <typeparam name="T"></typeparam>
    public class PoolCandy<T> : MonoBehaviour where T : Component
    {
        public Dictionary<string, Pool<T>> dic = new();

        public void InitList(params T[] list)
        {
            dic.Clear();
            foreach (var prefab in list)
            {
                var key = prefab.name;
                dic.Add(key, Pool<T>.Create(prefab, transform, 1));
            }
        }

        public T GetObject(string key)
        {
            if (dic.TryGetValue(key, out Pool<T> pool) == true)
            {
                return pool.GetObject();
            }

            return null;
        }

        public void ReturnObject(T obj)
        {
            if (dic.TryGetValue(obj.name, out Pool<T> pool) == true)
            {
                pool.ReturnObject(obj);
            }
        }

        public void RemoveAll()
        {
            dic.Clear();
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        public int GetObjectCount(string key)
        {
            if (dic.TryGetValue(key, out Pool<T> pool) == true)
            {
                return pool.ActiveCount();
            }

            return 0;
        }

    }
}

