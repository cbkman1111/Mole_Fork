using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuChat : MenuBase
{
    public bool InitMenu()
    {
        return true;
    }

    protected override void OnClick(Button btn)
    {
        string name = btn.name;
        if (name == "Button - Exit")
        {
            AppManager.Instance.ChangeScene(SceneBase.SCENES.SceneMenu);
        }
        else if (name == "Button - OpenScroll")
        {
            UIManager.Instance.OpenPopup<PopupScrollViewTest>("PopupScrollViewTest");
        }
    }

    public override void OnInit() { }
}
