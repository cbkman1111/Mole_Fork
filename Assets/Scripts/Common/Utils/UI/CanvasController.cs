using Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common.Global
{
    public class CanvasController : MonoBehaviour
    {
        private Canvas _canvas;
        private readonly List<UIObject.UIObject> _list = new List<UIObject.UIObject>();
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        public Transform GetTransform()
        {
            return _canvas.transform;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public T Open<T>(string name) where T : UIObject.UIObject
        {
            var trans = _canvas.transform.Find(name);
            T ret;

            if (trans == false)
            {
                string path = $"UI/{name}";
#if UNITY_EDITOR
                var prefab = ResourcesManager.Instance.LoadInBuild<T>(path);
#else
                T prefab = ResourcesManager.Instance.LoadBundle<T>(path);
                if (prefab == null)
                {
                    prefab = ResourcesManager.Instance.LoadInBuild<T>(path);
                }
#endif
                if (prefab == false)
                {
                    return default(T);
                }

                var clone = UnityEngine.Object.Instantiate(prefab, _canvas.transform);
                if (clone == false)
                {
                    Debug.LogError($"clone is null.");
                    return default(T);
                }

                clone.transform.SetAsLastSibling();
                clone.name = name;
                ret = clone;
            }
            else
            {
                ret = trans.GetComponent<T>();
            }

            if (_list.Contains(ret) == false)
            {
                _list.Add(ret);
            }

            return ret;
        }

        public void Close(string name)
        {
            var trans = _canvas.transform.Find(name);
            if (trans != true)
                return;

            var obj = trans.GetComponent<UIObject.UIObject>();
            obj.OnClose();

            if (_list != null && _list.Contains(obj))
            {
                _list.Remove(obj);
            }

            UnityEngine.Object.Destroy(trans.gameObject);
        }

        public void Close(Transform transform)
        {
            Close(transform.name);
        }

        public Transform Get(string name)
        {
            var ret = _list.Where(obj => string.Compare(obj.name, name, StringComparison.Ordinal) == 0).ToList();
            return ret.Count == 1 ? ret[0].transform : null;
        }

        public T Get<T>(string name) where T : UIObject.UIObject
        {
            var obj = _canvas.transform.Find(name);
            return obj.GetComponent<T>();
        }

        public int Count()
        {
            return _list.Count;
        }

        public Transform Last()
        {
            var index = _canvas.transform.childCount - 1;
            Transform trans = null;
            if (index >= 0)
            {
                trans = _canvas.transform.GetChild(index);
            }

            return trans;
        }

        public void Clear()
        {
            for (var i = 0; i < _canvas.transform.childCount; i++)
            {
                var trans = _canvas.transform.GetChild(0);
                UnityEngine.Object.Destroy(trans.gameObject);
            }
        }
    }
}
