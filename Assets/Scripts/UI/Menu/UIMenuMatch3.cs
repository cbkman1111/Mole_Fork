using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuMatch3 : MenuBase
{
    int level = 1;

    public Action<int> OnStartGame { get; set;}

    public override void OnInit()
    {

    }

    public bool InitMenu()
    {

        return true;
    }

    public override void OnValueChanged(InputField input, string str) 
    {
        string name = input.name;
        if(name == "InputField - Level")
        {
            level = int.Parse(str);
        }
    }

    protected override void OnClick(Button btn)
    {
        string name = btn.name;
        if (name == "Button - Back")
        {
            AppManager.Instance.ChangeScene(SceneBase.SCENES.SceneMenu);
        }
        else if (name == "Button - StartGame")
        {
            if (OnStartGame != null)
                OnStartGame(level);


        }
    }
}
