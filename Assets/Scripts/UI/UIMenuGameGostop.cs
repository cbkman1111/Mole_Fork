using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuGameGostop : MenuBase
{
    public override void OnInit()
    {

    }

    public bool InitMenu()
    {
        Button btn = GetObject<Button>("Button - A");
        return true;
    }

    

    protected override void OnClick(Button btn)
    {
        string name = btn.name;
        if(name == "Button - Exit")
        {
            
        }
    }
}
