using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupScrollViewTest : PopupBase
{
    private ScrollTest scrollView = null;

    /// <summary>
    ///  
    /// </summary>
    public override void OnInit() 
    {
        base.OnInit();

        List<ScrollData> list = new List<ScrollData>();

        for (int i = 0; i < 5; i++)
        {
            ScrollData data = new ScrollData();
            data.name = $"[{i}] message~~~";
            list.Add(data);
        }
    
        Transform trans = FindTransform(transform, "Scroll View - Test");
        if(trans != null)
        {
            RectTransform prefab = ResourcesManager.Instance.LoadInBuild<RectTransform>("CellTest");

            scrollView = trans.GetComponent<ScrollTest>();
            scrollView.Init(prefab);
            scrollView.SetItems(list);
        }
    }

    private void Update()
    {
        SetText("Text - DeltaY", $"{scrollView.CurrIndex}");
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Close()
    {
        transform.DOMove(
            new Vector3(transform.position.x, Screen.height * 2f), 0.5f).
            SetEase(Ease.InOutExpo).
            OnComplete(() => {
                base.Close();
            });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="button"></param>
    protected override void OnClick(Button button)
    {
        string name = button.name;

        if (name == "Button - Ok")
        {
            Close();
        }
        else if (name == "Button - Insert")
        {
            ScrollData data = new ScrollData();
            data.name = $"insert message~~~";
            scrollView.InsertMessage(data);
        }
    }
}
