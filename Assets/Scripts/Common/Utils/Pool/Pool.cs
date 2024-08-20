using System.Collections.Generic;
using UnityEngine;

namespace Common.Utils.Pool
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pool<T> where T : Component
    {
        private T _prefab = null;
        private Queue<T> _queue;
        private Transform _parent;
        public int Max { get; private set; } = 0;
        private List<T> _activeList { get; set; }

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
            this._activeList = new List<T>();
            this._parent = parent;

            for(int i = 0; i < max; i++)
            {
                var obj = Object.Instantiate(prefab, parent);
                if (obj != null)
                {
                    obj.name = prefab.name;
                    obj.gameObject.SetActive(false);
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
                obj.gameObject.SetActive(true);
                _activeList.Add(obj);
                return obj;
            }
            else
            {
                var insertObj = GameObject.Instantiate<T>(_prefab, _parent);
                if (insertObj == true)
                {
                    insertObj.name = _prefab.name;
                    insertObj.gameObject.SetActive(false);
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
            if (_activeList.Contains(obj) == true)
            {
                _queue.Enqueue(obj);
                _activeList.Remove(obj);
                obj.transform.SetParent(_parent);
                obj.gameObject.SetActive(false);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 객체를 queue로 반환하고, destory가 true면 모두 제거합니다.
        /// </summary>
        /// <param name="destroy"></param>
        public void ReturnAll()
        {
            // 큐로 옮기기.
            while (_activeList.Count > 0)
            {
                var obj = _activeList[0];
                ReturnObject(obj);
            }
        }

        public int ActiveCount()
        {
            return _activeList.Count;
        }
    }
}
