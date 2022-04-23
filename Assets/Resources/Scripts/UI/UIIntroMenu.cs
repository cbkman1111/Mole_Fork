using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIntroMenu : UIBase
{
    public bool Init()
    {
        return true;
    }

    public override void OnClick(Button btn)
    {
        string name = btn.name;
        if(CompareTo(name, "Button - Start") == 0)
        {
            AppManager.Instance.ChangeScene(SceneBase.SCENES.GAME);
        }
    }
}
