using Common.UIObject;
using UnityEngine;
using UnityEngine.UI;

public class UIJumpCoin : MenuBase
{
    public CanvasGroup canvasGroup = null;

    public Image GetIcon()
    {
        return List["Image - Icon"] as Image;
    }
}
