using System.Collections;
using System.Collections.Generic;
using Common.Global;
using Common.Scene;
using UI.Menu;
using UnityEngine;

public class SceneShooting : SceneBase
{
    public override bool Init(JSONObject param)
    {
        var menu = UIManager.Instance.OpenMenu<UIMenuShooting>();
        if (menu != null)
        {
            menu.InitMenu();
        }

        return true;
    }
}
