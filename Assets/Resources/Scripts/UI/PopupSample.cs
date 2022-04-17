using UnityEngine.UI;

public class PopupSample : PopupBase
{
    public override void OnClick(Button button)
    {
        if(button.name.CompareTo("Button - Ok") == 0)
        {
            Close();
        }
    }
}
