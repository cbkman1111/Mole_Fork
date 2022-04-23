using UnityEngine.UI;

public class PopupSample : PopupBase
{
    public override void Close()
    {
        //UIManager.Instance.ClosePopup(name);
        base.Close();
    }

    public override void OnClick(Button button)
    {
        string name = button.name;

        if(CompareTo(name, "Button - Ok") == 0)
        {
            Close();
        }
    }
}
