using UnityEngine;
using Common.UIObject;

namespace Common.Global.Singleton
{
    public abstract class Singleton<T> where T : class, new()
    {
        public string Tag = "Singleton - ";
        private static object _instanceLock = new object();
        private static volatile T _instance;
        public static T Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if(null == _instance)
                    {
                        _instance = new T();
                    }
                }

                return _instance;
            }
        }

        public abstract bool Init();
    }

    public abstract class Mono : MonoBehaviour
    {
        public abstract bool Init();
    }

    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance != false) return _instance;
                
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                _instance = GameObject.FindObjectOfType(typeof(T)) as T;
                if (_instance != false) return _instance;
                
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                DontDestroyOnLoad(_instance);

                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                _instance.Init();
                return _instance;
            }
        }

        protected abstract bool Init();
        public virtual void Destroy()
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }

        protected string Tag => name;
    }
}
