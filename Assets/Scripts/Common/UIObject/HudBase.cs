using Common.Global;
using UnityEngine.UI;

namespace Common.UIObject
{
    public class HudBase : UIObject
    {

        protected override void OnClick(Button button) { }
        public override void Close()
        {
            UIManager.Instance.CloseHud(name);
        }
    }
}

