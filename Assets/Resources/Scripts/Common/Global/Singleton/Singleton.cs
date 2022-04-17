using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Singleton
{
    public abstract class Singleton<T> where T : class, new()
    {
        public string TAG = "Singleton - ";
        protected static object instanceLock = new object();
        protected static volatile T instance;
        public static T Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if(null == instance)
                    {
                        instance = new T();
                    }
                }

                return instance;
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
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType(typeof(T)) as T;
                    if (instance == null)
                    {
                        instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                        DontDestroyOnLoad(instance);

                        instance.Init();
                    }
                }

                return instance;
            }
        }

        public abstract bool Init();
        public string TAG { get => name; }
    }
}
