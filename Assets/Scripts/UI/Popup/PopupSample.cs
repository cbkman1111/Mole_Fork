using UnityEngine;
using UnityEngine.UI;

public class PopupSample : PopupBase
{
    public override void Close()
    {
        iTween.MoveTo(gameObject,
               iTween.Hash(
                   "y", Screen.height * 2,
                   "time", 0.25f,
                   "easeType", "easeInOutExpo",
                   "oncompletetarget", gameObject,
                   "oncomplete", "OnCloseComplete"));
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
        else if (name == "Button - Other")
        {
            PopupNormal popup = UIManager.Instance.OpenPopup<PopupNormal>("UI/PopupNormal");
        }
    }
}
