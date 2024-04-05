using BehaviorDesigner.Runtime.Tasks;
using Cinemachine;
using Common.Global;
using Common.Scene;
using Common.UIObject.Scroll;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Dotween을 이용한 베지어 곡선 이동 테스트.
/// </summary>
public class SceneDotween : SceneBase
{
    [SerializeField]
    private DotweenBezior _bezior = null;

    [SerializeField]
    private GameObject _projectilePrefab = null;

    [SerializeField]
    private GameObject _projectileTemp = null;

    private Pool<Transform> _pool = null;

    //public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public AnimationCurve rotationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 180)); // Min and Max rotation values
    public float duration = 2.0f;

    public override bool Init(JSONObject param)
    {
        var menu = UIManager.Instance.OpenMenu<UIMenuDotween>("UIMenuDotween");
        menu.InitMenu(OnShoot, OnSlide);

        _pool = Pool<Transform>.Create(_projectilePrefab.transform, transform, 10);
        return true;
    }

    public void OnSlide(float percent)
    {
        float currentRotationZ = rotationCurve.Evaluate(percent);
        _projectileTemp.transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentRotationZ));
    }

    public void OnShoot()
    {
        var path = _bezior.GetPath();
        var startPoint = _bezior.GetStartPoint();
        var endPoint = path.Last();
        Gizmos.color = Color.blue;

        var obj = _pool.GetObject();
        obj.transform.position = startPoint;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.DOPath(path, duration, PathType.CubicBezier, PathMode.Full3D, 10, Color.yellow).
            SetLookAt(0.1f).
            //SetEase(curve).
            OnUpdate(() =>
            {
                float totalDistance = Vector3.Distance(startPoint, endPoint);
                float remainingDistance = Vector3.Distance(obj.transform.position, endPoint);
                float remainingDistanceRatio = remainingDistance / totalDistance;

                float currentRotationZ = rotationCurve.Evaluate(1 - remainingDistanceRatio);
                obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentRotationZ));
            }).
            OnComplete(() =>
            {
                MEC.Timing.RunCoroutine(ReturnObject(obj));
            });
    }

    private IEnumerator<float> ReturnObject(Transform obj)
    {
        yield return MEC.Timing.WaitForSeconds(2);

        _pool.ReturnObject(obj);
        obj = null;
    }
}
