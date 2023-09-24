using Common.Global;
using UnityEngine.UI;

namespace Common.UIObject
{
    public class HudBase : UIObject
    {
        public override void OnInit() { }
        protected override void OnClick(Button button) { }
        public override void Close()
        {
            UIManager.Instance.CloseHud(name);
        }
    }
}

