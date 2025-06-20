using Common.Global;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UIObject
{
    [Path("UI/Prefabs/Popup")]
    public class PopupBase : UIObject
    {

        protected override void OnClick(Button button) { }
        public override void Close()
        {
            UIManager.Instance.ClosePopup(name);
        }
    }
}