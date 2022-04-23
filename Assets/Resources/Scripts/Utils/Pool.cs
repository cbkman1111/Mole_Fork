using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : Object
{
    private T prefab = null;
    private int max = 0;
    private Queue<T> queue;
    private Transform parent;

    /// <summary>
    /// 오브젝트 풀.
    /// </summary>
    /// <param name="prefab">프리팹 리소스</param>
    /// <param name="count">최대 수량</param>
    /// <returns></returns>
    public static Pool<T> Create(T prefab, Transform parent, int max)
    {
        var pool = new Pool<T>();
        if(pool != null && pool.Init(prefab, parent, max))
        {
            return pool;
        }

        return null;
    }

    public bool Init(T prefab, Transform parent, int max) 
    {
        this.prefab = prefab;
        this.max = max;
        this.queue = new Queue<T>();
        this.parent = parent;

        for(int i = 0; i < max; i++)
        {
            var obj = GameObject.Instantiate(prefab, parent);
            if (obj != null)
            {
                queue.Enqueue(obj);
            }
        }
     
        return true;
    }

    public T GetObject()
    {
        if(queue.Count > 0)
        {
            return queue.Dequeue();
        }

        return default(T);
    }

    public void ReturnObject(T obj)
    {
        queue.Enqueue(obj);
    }
}
