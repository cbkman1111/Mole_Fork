using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuChatt : MenuBase
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
            AppManager.Instance.ChangeScene(SceneBase.SCENES.Menu);
        }
        else if (name == "Button - OpenScroll")
        {
            UIManager.Instance.OpenPopup<PopupScrollViewTest>("PopupScrollViewTest");
        }
    }

    public override void OnInit() { }
}
