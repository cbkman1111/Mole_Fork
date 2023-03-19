using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupNormal : PopupBase
{
    public override void OnInit() 
    {
        transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 2.0f);
        transform.DOMove(
            new Vector3(transform.position.x, Screen.height * 0.5f), 0.4f).
            SetEase(Ease.OutExpo).
            OnComplete(() => { 
            });
    }

    public override void Close()
    {
        transform.DOMove(
            new Vector3(transform.position.x, Screen.height * 2), 0.4f).
            SetEase(Ease.OutExpo).
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
