using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropBase : MonoBehaviour
{
    Vector2Int coord = Vector2Int.zero;
    
    private void Start()
    {
        var scene = AppManager.Instance.GetCurrentScene();
        Vector3 v = scene.MainCamera.transform.position - transform.position;
        v.z = 0;
        v.y = 0;

        Transform trans = transform.GetChild(0);
        trans.LookAt(scene.MainCamera.transform.position - v);
    }



}
