using UnityEngine;
using UnityEngine.Pool;

namespace Unity.Behavior.Demo
{
    /// <summary>
    /// Provides product based on type (factory) and manages them in a Unity IObjectPool.
    /// </summary>
    public abstract class MonoBehaviourPool<T> : MonoBehaviour
        where T : class
    {
        public int reserveSize => m_InitialSize;

        [Header("Pool")]
        [SerializeField] private bool m_CreatePoolOnAwake = true;

        [SerializeField] private int m_InitialSize = 10;
        [SerializeField] private int m_MaxSize = 100;

        [Tooltip("Should an exception be thrown if we try to return an existing item, already in the pool?")]
        [SerializeField] private bool m_CollectionCheck = true;

        private IObjectPool<T> m_Pool;

        public virtual T Get()
        {
#if DEBUG
            if (m_Pool == null)
            {
                Debug.LogError("Object pool is not initialized.");
                return default(T);
            }
#endif
            return m_Pool.Get();
        }

        public virtual void GetAsync(System.Action<T> onCompleted)
        {
#if DEBUG
            if (m_Pool == null)
            {
                Debug.LogError("Object pool is not initialized.");
                return;
            }
#endif
            onCompleted?.Invoke(m_Pool.Get());
        }

        public virtual void Release(T obj)
        {
            m_Pool.Release(obj);
        }

        public void SetInitialSize(int value)
        {
            m_InitialSize = value;
        }

        public virtual void ResetPool()
        {
            if (m_Pool != null)
            {
                m_Pool.Clear();
                m_Pool = null;
            }

            m_Pool = new ObjectPool<T>(OnProductCreation, OnGetFromPool,
                OnProductReleased, OnProductDestruction, m_CollectionCheck, m_InitialSize, m_MaxSize);

            T[] products = new T[m_InitialSize];
            for (int i = 0; i < m_InitialSize; i++)
            {
                products[i] = m_Pool.Get();
            }

            for (int i = 0; i < m_InitialSize; i++)
            {
                m_Pool.Release(products[i]);
            }
        }

        protected virtual void Awake()
        {
            if (m_CreatePoolOnAwake)
            {
                ResetPool();
            }
        }

        // invoked when creating an item to populate the object pool
        protected abstract T OnProductCreation();

        // invoked when returning an item to the object pool
        protected abstract void OnProductReleased(T product);

        // invoked when retrieving the next item from the object pool
        protected abstract void OnGetFromPool(T product);

        // invoked when we exceed the maximum number of pooled items (i.e. destroy the pooled object)
        protected abstract void OnProductDestruction(T product);
    }
}