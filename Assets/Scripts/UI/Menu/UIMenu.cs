using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MenuBase
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
        if(name == "Button - Start1")
        {
            AppManager.Instance.ChangeScene(SceneBase.SCENES.GAME);
        }
        else if (name == "Button - Start2")
        {
            AppManager.Instance.ChangeScene(SceneBase.SCENES.GAME_GOSTOP);
        }
    }
}
