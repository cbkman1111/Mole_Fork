using System.Collections.Generic;
using UnityEngine;

namespace Common.Utils.Pool
{
    [System.Serializable]
    public class PoolObject
    {
        [SerializeField]  public Transform Prefab;
        [SerializeField]  public int Count;
    }

    public class PoolManager : MonoBehaviour
    {
        [SerializeField]
        public PoolObject[] poolList;

        public Dictionary<string, Pool<Transform>> dic = null;

        private void Awake()
        {
            dic = new Dictionary<string, Pool<Transform>>();

            for (int i = 0; i < poolList.Length; i++)
            {
                var info = poolList[i];
                var key = info.Prefab.name;
                var prefab = info.Prefab;
                var size = info.Count;

                dic.Add(key, Pool<Transform>.Create(prefab, transform, size));
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
            bool success = false;
            if (dic.TryGetValue(obj.name, out Pool<Transform> pool) == true)
            {
                success = pool.ReturnObject(obj);
            }
        }
    }
}