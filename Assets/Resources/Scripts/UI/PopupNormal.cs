using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupNormal : PopupBase
{
    public override void OnInit() 
    {
        transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 2.0f);

        var hash = iTween.Hash(
                "y", Screen.height * 0.5f,
                "time", 0.4f,
                "easeType", "easeInOutExpo");

        iTween.MoveTo(gameObject, hash);
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
