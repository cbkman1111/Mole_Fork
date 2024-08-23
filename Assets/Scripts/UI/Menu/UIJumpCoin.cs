using Common.UIObject;
using UnityEngine;
using UnityEngine.UI;

public class UIJumpCoin : MenuBase
{
    public CanvasGroup canvasGroup = null;
    public int Index;

    public void ShowText(bool enable)
    {
        SetActive("Text - Count", enable);
    }
}
