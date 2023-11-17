using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Utils.Pool;

public class Done_DestroyByTime : MonoBehaviour
{
    public float lifetime;

    void Start()
    {
        MEC.Timing.RunCoroutine(Destroy(lifetime));
        //Destroy(gameObject, lifetime);
    }

    private IEnumerator<float> Destroy(float time)
    {
        yield return MEC.Timing.WaitForSeconds(time);
        PoolManager.Instance.ReturnObject(transform);
    }
}
