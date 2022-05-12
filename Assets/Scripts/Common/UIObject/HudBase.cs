using System;
using UnityEngine;
using UnityEngine.UI;

public class HudBase : UIObject
{
    public override void OnInit() { }
    protected override void OnClick(Button button) { }
    public override void Close()
    {
        UIManager.Instance.CloseHud(name);
    }
}

