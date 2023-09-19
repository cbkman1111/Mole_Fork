using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Common.UIObject;
using UnityEngine;
using UnityEngine.UI;

public class UILoadingMenu : MenuBase
{
    private float Percent = 0f;
    private MEC.CoroutineHandle handler;

    
    public override void OnInit()
    {

    }

    public bool InitMenu()
    {
        return true;
    }

    public void SetPercent(float percent)
    {
        Percent = percent;

        handler.IsRunning = false;
        handler = MEC.Timing.RunCoroutine(AddPercent());
    }

    private IEnumerator<float> AddPercent()
    {
        Slider slider = GetObject<Slider>("Slider - Percent");
        float remain = Percent - slider.value;

        float addAmount = remain * 0.1f;
        while (slider.value < Percent)
        {
            float amount = slider.value + addAmount;
            if(amount >= 0.99f)
                amount = 1.0f;
            
            slider.value = amount;
            yield return MEC.Timing.WaitForOneFrame;
        }
    }

    public bool Complete()
    {
        Slider slider = GetObject<Slider>("Slider - Percent");
        return slider.value == 1.0f;
    }
}
