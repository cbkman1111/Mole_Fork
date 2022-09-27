using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoadingMenu : MenuBase
{
    public override void OnInit()
    {

    }

    public bool InitMenu()
    {
        return true;
    }

    public void SetPercent(float percent)
    {
        Slider slider = GetObject<Slider>("Slider - Percent");
        slider.value = percent;
    }

    protected override void OnClick(Button btn)
    {
        string name = btn.name;
    }
}
