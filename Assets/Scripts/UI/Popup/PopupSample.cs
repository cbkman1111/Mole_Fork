using Common.Global;
using Common.UIObject;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup
{
    public class PopupSample : PopupBase
    {
        public override void Close()
        {
            transform.DOMove(
                    new Vector3(transform.position.x, Screen.height * 2), 0.25f).
                SetEase(Ease.OutExpo).
                OnComplete(() => {
                    base.Close();
                });
        }

        protected override void OnClick(Button button)
        {
            string name = button.name;
            if(name == "Button - Ok")
            {
                Close();
            }
            else if (name == "Button - Other")
            {
                PopupNormal popup = UIManager.Instance.OpenPopup<PopupNormal>("UI/PopupNormal");
            }
        }
    }
}
