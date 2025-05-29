using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Common.UIObject;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupLoading : PopupBase
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
        var remain = Percent - slider.value;
        var amount = remain * 0.1f;
        while (slider.value < Percent)
        {
            slider.value += amount;
            if(slider.value > Percent)
            {
                slider.value = Percent;
                break;
            }

            yield return MEC.Timing.WaitForOneFrame;
        }
    }

    public bool Complete(float percent)
    {
        Slider slider = GetObject<Slider>("Slider - Percent");
        return slider.value >= percent || Mathf.Approximately(slider.value, percent);
    }
}
