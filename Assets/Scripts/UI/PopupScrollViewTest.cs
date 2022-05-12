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

        var hash = iTween.Hash(
                "y", Screen.height * 0.5f,
                "time", 0.4f,
                "easeType", "easeInOutExpo");

        iTween.MoveTo(gameObject, hash);

        // 
        List<ScrollData> list = new List<ScrollData>();
        for(int i = 0; i < 2000; i++)
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
        var hash = iTween.Hash(
                   "y", Screen.height * 2,
                   "time", 0.5f,
                   "easeType", "easeInOutExpo",
                   "oncompletetarget", gameObject,
                   "oncomplete", "OnCloseComplete");

        iTween.MoveTo(gameObject,hash);
    }

    public void OnCloseComplete()
    {
        base.Close();
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
