using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuGostop : MenuBase
{
    public override void OnInit()
    {

    }

    public bool InitMenu()
    {
        return true;
    }

    protected override void OnClick(Button btn)
    {
        string name = btn.name;
        if(name == "Button - Exit")
        {
            AppManager.Instance.ChangeScene(SceneBase.SCENES.MENU);
        }
    }
}
