using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class Pool<T> where T : Object
{
    private T prefab = null;
    private int max = 0;
    private Queue<T> queue;
    private List<T> list; // 사용.
    private Transform parent;
    public int Max => max;
    public List<T> ActiveList => list;

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

        this.prefab = prefab;
        this.max = max;
        this.queue = new Queue<T>();
        this.list = new List<T>();
        this.parent = parent;

        for(int i = 0; i < max; i++)
        {
            var obj = GameObject.Instantiate(prefab, parent);
            if (obj != null)
            {
                Transform trans = obj as Transform;
                if (trans != null)
                {
                    trans.name = prefab.name;
                    trans.gameObject.SetActive(false);
                }

                queue.Enqueue(obj);
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

        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
            Transform trans = obj as Transform;
            if (trans != null)
            {
                trans.gameObject.SetActive(true);
            }
            
            list.Add(obj);
            return obj;
        }

        return default(T);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    public bool ReturnObject(T obj)
    {
        if (list.Contains(obj) == true)
        {
            queue.Enqueue(obj);
            list.Remove(obj);

            Transform trans = obj as Transform;
            if (trans != null)
            {
                trans.gameObject.SetActive(false);
            }

            return true;
        }

        return false;
    }
}
