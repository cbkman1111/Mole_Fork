using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuTileMap : MenuBase
{
    [SerializeField]
    public Ant.Joystick Joystick = null;

    public override void OnInit()
    {

    }

    public bool InitMenu(Action<Vector3> move, Action stop)
    {
        Joystick.Init((Vector3 direct, float angle) => {
            SetText("Text - Debug", $"Angle : {angle}");

            if (move != null)
            {
                move(direct);
            }
        },
        () => {
            if (stop != null)
            {
                stop();
            }
        });

        return true;
    }

    protected override void OnClick(Button btn)
    {
        string name = btn.name;
        if(name == "Button - Back")
        {
            AppManager.Instance.ChangeScene(SceneBase.SCENES.SceneMenu);
        }
    }
}
