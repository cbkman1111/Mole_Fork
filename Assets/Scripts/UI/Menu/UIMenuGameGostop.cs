using Common.UIObject;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuGameGostop : MenuBase
    {
        public override void OnInit()
        {
            
        }

        public bool InitMenu()
        {
            var btn = GetObject<Button>("Button - A");
            return true;
        }

        protected override void OnClick(Button btn)
        {
            var btnName = btn.name;
            if(btnName == "Button - Exit")
            {
            
            }
        }
    }
}
