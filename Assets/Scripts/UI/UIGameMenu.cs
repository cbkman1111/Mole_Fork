using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameMenu : MenuBase
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
        if(name == "Button - A")
        {
            UIManager.Instance.OpenPopup<PopupSample>("PopupSample");
            SetText("Text - title", "a clicked");
        }
        else if (name == "Button - B")
        {
            SoundManager.Instance.PlayEffect("EFF_shoot");
            SetText("Text - title", "b clicked");
        }
        else if (name == "Button - C")
        {
            SoundManager.Instance.PlayEffect("single_coin_fall_on_wood");
            SetText("Text - title", "c clicked");

            UIManager.Instance.OpenPopup<PopupScrollViewTest>("PopupScrollViewTest");
        }
    }
}
