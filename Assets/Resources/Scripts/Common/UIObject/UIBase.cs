using UnityEngine.UI;

public abstract class UIBase : UIObject
{
    public override void Close()
    {
        UIManager.Instance.CloseMenu(name);
    }

    public override void OnClick(Button button) {}
}
