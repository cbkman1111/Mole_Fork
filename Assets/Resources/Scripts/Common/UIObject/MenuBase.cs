using UnityEngine.UI;

public abstract class MenuBase : UIObject
{
    public override void Close()
    {
        UIManager.Instance.CloseMenu(name);
    }

    public override void OnClick(Button button) {}
}
