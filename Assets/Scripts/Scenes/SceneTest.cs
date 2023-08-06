using SweetSugar.LeanTween.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class SceneTest : SceneBase
{
    private List<GameObject> tileList = null;
    private List<Vector3> tilePosition = null;
    private List<UIEtcGoal> goals = null;

    public Ease tweenType = Ease.Linear;
    public float speed = 0;
    public float inverval = 0f;
    public float scaleTime = 0f;

    private UIMenuTest menu = null;
    private int width = 0;
    private int height = 0;
    private float size = 1.0f;

    public PoolManager poolManager = null;

    public MeshRenderer meshRender = null;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool Init(JSONObject param)
    {
        Application.targetFrameRate = 100;

        tileList = new List<GameObject>();
        goals = new List<UIEtcGoal>();
        tilePosition = new List<Vector3>();
        width = 10;
        height = 10;
        size = 1.0f;
        tweenType = Ease.InOutCubic;
        speed = 60f;
        inverval = 0.25f;
        scaleTime = 0.1f;

        menu = UIManager.Instance.OpenMenu<UIMenuTest>("UIMenuTest");
        if (menu != null)
        {
            menu.InitMenu();
        }

        var cube = ResourcesManager.Instance.LoadInBuild<GameObject>("Cube");

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var obj = Instantiate<GameObject>(cube, null);
                var pos = new Vector2(x * size, -y * size);
                pos.x -= width * 0.5f;
                pos.y += height * 0.5f;

                obj.transform.position = pos;
                tilePosition.Add(pos);
                tileList.Add(obj);
            }
        }

        meshRender.material.color = new Color(1, 1, 1, 0.3f);
        //meshRender.sharedMaterial.color = new Color(1, 1, 1, 0.3f);
        return true;
    }

    /*

    */

    /// <summary>
    /// 
    /// </summary>
    private void CreateGoal()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        {
            int x = menu.x;
            int y = menu.y;

            int rand = Random.Range(1, 100) % 2;
            int totoal = Random.Range(3, 6);
            if (x < 0 || x >= width)
                return;

            if (y < 0 || y >= height)
                return;

            int index = x + y * width;

            for (int i = 0; i < totoal; i++)
            {
                int nextIndex = 0;
                if (rand == 0)
                {
                    nextIndex = index + i;
                }
                else
                {
                    nextIndex = index + i * width;
                }

                if (tileList.Count < 0 || tileList.Count <= nextIndex)
                {
                    continue;
                }

                var tile = tileList[nextIndex];
                if (tile == null)
                    continue;

                var obj = poolManager.GetObject("UIEtcGoal");
                if (obj != null)
                {
                    var goal = obj.GetComponent<UIEtcGoal>();
                    var startPoint = MainCamera.WorldToScreenPoint(tile.transform.position);
                    var endPoint = menu.to.transform.position;

                    goal.Start = startPoint;
                    goal.End = endPoint;
                    goal.ScaleComplete = false;
                    goal.transform.position = startPoint;
                    goal.transform.localScale = Vector3.zero;
                    goal.transform.SetParent(menu.to.transform);
                    goal.transform.DOScale(Vector3.one, scaleTime).
                    //goal.transform.LeanScale(Vector3.one, scaleTime).
                    //LeanTween.scale(goal.gameObject, c
                        OnComplete(() => {
                            goal.ScaleComplete = true;
                        });

                    goals.Add(goal);
                }
            }

            MEC.Timing.RunCoroutine(MoveGoal(inverval));
        }
    }

    private IEnumerator<float> MoveGoal(float interval)
    {
        while (goals.Count > 0)
        {
            var goal = goals.Where(g => g.ScaleComplete == true).FirstOrDefault();
            if (goal != null)
            {
                float distance = Vector3.Distance(goal.Start, goal.End) * 0.10f;
                var list = new List<Vector3>();

                list.Add(goal.Start);
                list.Add(goal.Start);

                list.Add(goal.Start + Vector3.right * size * 1);

                list.Add(goal.End);
                list.Add(goal.End);
                float time = distance / speed;
                goal.transform.DOPath(list.ToArray(), time).
                    SetEase(tweenType).
                    OnComplete(() => {
                        goal.transform.DOScale(Vector3.one * 0.1f, 0.4f).
                            OnComplete(() => {
                                poolManager.ReturnObject(goal.transform);
                            });
                    });

                
                goals.Remove(goal);
            }

            yield return MEC.Timing.WaitForSeconds(interval);
        }
        
    }

    public override void OnTouchBean(Vector3 position)
    {
        Ray ray = MainCamera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            var layer = hit.collider.gameObject.layer;
            if (layer == LayerMask.NameToLayer("Tile"))
            {
                var obj = hit.collider.gameObject;
                var index = tileList.FindIndex(tile => tile == obj);
                if (index != -1)
                {
                    menu.x = index % width;
                    menu.y = index / height;

                    CreateGoal();
                }
            }

            Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 5f);
            Debug.Log(hit.point);
        }
    }

    public override void OnTouchEnd(Vector3 position)
    {

    }

    public override void OnTouchMove(Vector3 position)
    {

    }
}