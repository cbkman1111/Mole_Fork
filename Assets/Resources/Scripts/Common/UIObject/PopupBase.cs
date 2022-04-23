using UnityEngine;
using UnityEngine.UI;

public class PopupBase : UIObject
{
    public override void OnInit() { }
    protected override void OnClick(Button button) { }

    public virtual void ShowAnimation()
    {
        transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 2.0f);
        iTween.MoveTo( gameObject, 
            iTween.Hash( 
                "y", Screen.height * 0.5f, // center                     
                "time", 0.4f,       
                "easeType", "easeInOutExpo",                                                 
                "oncompletetarget", gameObject, 
                "oncomplete", "OnShowComplete" ));                           
    }

    public override void Close()
    {
        iTween.MoveTo(gameObject,
            iTween.Hash(
                "y", Screen.height * 5,
                "time", 0.4f,
                "easeType", "easeInOutExpo",
                "oncompletetarget", gameObject,
                "oncomplete", "OnCloseComplete"));
    }

    private void OnCloseComplete()
    {
        UIManager.Instance.ClosePopup(name);
    }
}

