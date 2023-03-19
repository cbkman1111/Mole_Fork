using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupScrollViewTest : PopupBase
{
    private ScrollTest scrollView = null;

    public override void OnInit() 
    {
        transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 2.0f);

        transform.DOMove(
            new Vector3(transform.position.x, Screen.height * 0.5f), 0.4f).
            SetEase(Ease.OutExpo).
            OnComplete(() => {
            });

        // 
        List<ScrollData> list = new List<ScrollData>();
        for(int i = 0; i < 10; i++)
        {
            ScrollData data = new ScrollData();
            data.no = i;
            data.name = $"name_{i}";
            list.Add(data);
        }

        Transform trans = FindTransform(transform, "Scroll View - Test");
        if(trans != null)
        {
            RectTransform prefab = ResourcesManager.Instance.LoadBundle<RectTransform>("CellTest");
            scrollView = trans.GetComponent<ScrollTest>();
            scrollView.Init(prefab);
            scrollView.SetItems(list);
        }
    }

    public override void Close()
    {
        transform.DOMove(
            new Vector3(transform.position.x, Screen.height * 2f), 0.5f).
            SetEase(Ease.InOutExpo).
            OnComplete(() => {
                base.Close();
            });
    }


    protected override void OnClick(Button button)
    {
        string name = button.name;

        if(name == "Button - Ok")
        {
            Close();
        }
    }
}
