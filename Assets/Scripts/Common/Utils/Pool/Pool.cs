using System.Collections.Generic;
using UnityEngine;

namespace Common.Utils.Pool
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pool<T> where T : Object
    {
        private T _prefab = null;
        private Queue<T> _queue;
        private Transform _parent;
        public int Max { get; private set; } = 0;
        private List<T> ActiveList { get; set; }

        /// <summary>
        /// 오브젝트 풀.
        /// </summary>
        /// <param name="prefab">프리팹 리소스</param>
        /// <param name="count">최대 수량</param>
        /// <returns></returns>
        public static Pool<T> Create(T prefab, Transform parent, int max)
        {
            var pool = new Pool<T>();
            if(pool.Init(prefab, parent, max))
            {
                return pool;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public bool Init(T prefab, Transform parent, int max) 
        {
            if (prefab == null)
                return false;

            this._prefab = prefab;
            this.Max = max;
            this._queue = new Queue<T>();
            this.ActiveList = new List<T>();
            this._parent = parent;

            for(int i = 0; i < max; i++)
            {
                var obj = Object.Instantiate(prefab, parent);
                if (obj != null)
                {
                    var trans = obj as Transform;
                    if (trans != null)
                    {
                        trans.name = prefab.name;
                        trans.gameObject.SetActive(false);
                    }

                    _queue.Enqueue(obj);
                }
            }
     
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T GetObject()
        {
            T obj = null;
            if (_queue.Count > 0)
            {
                obj = _queue.Dequeue();
                var trans = obj as Transform;
                if (trans != null)
                {
                    trans.gameObject.SetActive(true);
                }
            
                ActiveList.Add(obj);
                return obj;
            }
            else
            {
                var insertObj = Object.Instantiate(_prefab, _parent);
                if (insertObj == true)
                {
                    var trans = insertObj as Transform;
                    if (trans != null)
                    {
                        trans.name = _prefab.name;
                        trans.gameObject.SetActive(false);
                    }

                    _queue.Enqueue(insertObj);
                    return GetObject();
                }
            }
            
            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public bool ReturnObject(T obj) 
        {
            if (ActiveList.Contains(obj) == true)
            {
                _queue.Enqueue(obj);
                ActiveList.Remove(obj);

                var trans = obj as Transform;
                if (trans != null)
                {
                    trans.gameObject.SetActive(false);
                }

                return true;
            }

            return false;
        }

        public int ActiveCount()
        {
            return ActiveList.Count;
        }

    }
}
