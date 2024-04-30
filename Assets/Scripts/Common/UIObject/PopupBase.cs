using Common.Global;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UIObject
{
    public class PopupBase : UIObject
    {

        public override void OnInit() 
        {
            /*
            transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 2.0f);
            transform.DOMove(
                    new Vector3(transform.position.x, Screen.height * 0.5f), 0.4f).
                    SetEase(Ease.OutExpo);
            */
        }

        protected override void OnClick(Button button) { }
        public override void Close()
        {
            UIManager.Instance.ClosePopup(name);
        }
    }
}