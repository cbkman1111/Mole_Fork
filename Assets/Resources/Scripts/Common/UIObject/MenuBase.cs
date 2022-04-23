using UnityEngine.UI;

public abstract class MenuBase : UIObject
{
    public override void OnInit() { }
    protected override void OnClick(Button button) { }

    public override void Close()
    {
        UIManager.Instance.CloseMenu(name);
    }
}
