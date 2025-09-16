using Common.Global;
using Common.Scene;
using UI.Menu;
using UnityEngine;

public class SceneAI : SceneBase
{
    public override bool Init(JSONObject param)
    {
        var menu = UIManager.Instance.OpenMenu<UIMenuAI>();
        if (menu != null)
        {
            menu.InitMenu();
        }


        return true;
    }
}
