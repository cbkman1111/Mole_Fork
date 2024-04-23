using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//  
public class DotweenBezior : MonoBehaviour
{
    //[DisplayAsString]
    [Header("Ãâ¹ß")]
    public Transform StartPoint = null;
    public Transform handl_a;

    [Header("Áß°£")]
    public Transform handl_b;
    public Transform wp_a = null;
    public Transform handl_c;

    [Header("µµÂø")]
    public Transform handl_d;
    public Transform wp_b = null;
    public float _gizmoDetail = 30;
    List<Vector3> _gizmoPoints = new List<Vector3>();

    //[Header("¾Þ±Û")]


    public Vector3[] GetPath()
    {
        Vector3[] path = new Vector3[6];

        // https://github.com/Demigiant/dotween/issues/75
        path.SetValue(wp_a.position, 0); ; // WP0
        path.SetValue(handl_a.transform.position, 1); // a

        path.SetValue(handl_b.transform.position, 2); // b
        path.SetValue(wp_b.transform.position, 3); //WP1
        path.SetValue(handl_c.transform.position, 4); // b

        path.SetValue(handl_d.transform.position, 5); // a
        return path;
    }

    public Vector3 GetStartPoint()
    {
        return StartPoint.position;
    }   

    private void SampleFun()
    {
        /*
        Vector3[] path = GetPath();
        Gizmos.color = Color.blue;

        var obj = Instantiate<Transform>(projectile, StartPoint.position, Quaternion.identity);
        obj.transform.DOPath(path, 3, PathType.CubicBezier, PathMode.Full3D, 10, Color.yellow).
            OnComplete(() => {
                Destroy(obj.gameObject);
            });
        */
    }
    private void OnDrawGizmos()
    {
        Vector3[] path = new Vector3[6];

        path.SetValue(wp_a.position, 0); ; // WP0
        path.SetValue(handl_a.transform.position, 1); // a

        path.SetValue(handl_b.transform.position, 2); // b
        path.SetValue(wp_b.transform.position, 3); //WP1
        path.SetValue(handl_c.transform.position, 4); // b

        path.SetValue(handl_d.transform.position, 5); // a

        _gizmoPoints.Clear();
        foreach (var point in path)
        {
            _gizmoPoints.Add(point);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(StartPoint.position, handl_a.position);
        Gizmos.DrawLine(wp_a.position, handl_b.position);
        Gizmos.DrawLine(wp_a.position, handl_c.position);
        Gizmos.DrawLine(wp_b.position, handl_d.position);

#if UNITY_EDITOR
        Handles.DrawBezier(StartPoint.position, wp_a.position, handl_a.position, handl_b.position, Color.red, null, 2);
        Handles.DrawBezier(wp_a.position, wp_b.position, handl_c.position, handl_d.position, Color.red, null, 2);
#endif
    }
}
