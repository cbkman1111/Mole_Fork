using BehaviorDesigner.Runtime.Tasks;
using Common.Global;
using Common.Scene;
using Common.UIObject.Scroll;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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

    private Pool<Transform> _pool = null;


    public override bool Init(JSONObject param)
    {
        var menu = UIManager.Instance.OpenMenu<UIMenuDotween>("UIMenuDotween");
        menu.InitMenu(OnShoot);

        _pool = Pool<Transform>.Create(_projectilePrefab.transform, transform, 10);
        return true;
    }

    public void OnShoot()
    {
        var path = _bezior.GetPath();
        var startPoint = _bezior.GetStartPoint();
        Gizmos.color = Color.blue;

        var obj = _pool.GetObject();//Instantiate<Transform>(_projectilePrefab.transform, startPoint, Quaternion.identity);
        obj.transform.position = startPoint;
        obj.transform.localScale = Vector3.one;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.DOPath(path, 1, PathType.CubicBezier, PathMode.Full3D, 10, Color.yellow).
            SetEase(Ease.Linear).
            OnComplete(() => {

                _pool.ReturnObject(obj);
                obj = null;
                
                //Destroy(obj.gameObject, 2f);
            });
    }

}
