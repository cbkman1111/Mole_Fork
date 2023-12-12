using Common.UIObject;
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup
{
    public class PopupNormal : PopupBase
    {
        public Action _close { get; set; } = null;

        public void SetOnClose(Action close)
        {
            _close = close;
        }

        public void SetUI()
        {
            SetText("Text - Title", "GameOver");
            SetText("Text - Ok", "확인");
        }

        public override void OnInit() 
        {
            transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 2.0f);
            transform.DOMove(
                    new Vector3(transform.position.x, Screen.height * 0.5f), 0.4f).
                SetEase(Ease.OutExpo).
                OnComplete(() => { 
                });
        }

        public override void Close()
        {
            transform.DOMove(
                    new Vector3(transform.position.x, Screen.height * 2), 0.4f).
                SetEase(Ease.OutExpo).
                OnComplete(() => {
                    base.Close();


                });
        }

        public override void OnClose() 
        {
            if (_close != null)
            {
                _close();
                _close = null;
            }
        }

        protected override void OnClick(Button button)
        {
            string name = button.name;

            if(name == "Button - Ok")
            {
                Close();
            }
        }
    }
}
