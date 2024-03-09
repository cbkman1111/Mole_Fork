using Common.Global;
using Common.Scene;
using Common.UIObject;
using System;
using UnityEngine.UI;

public class UIMenuDotween : MenuBase
{
    private Action OnShoot = null;
    public bool InitMenu(Action shoot)
    {
        OnShoot = shoot;
        return true;
    }
    protected override void OnClick(Button btn)
    {
        string name = btn.name;
        if (name == "Button - Shoot")
        {
            OnShoot();
        }
    }
}
