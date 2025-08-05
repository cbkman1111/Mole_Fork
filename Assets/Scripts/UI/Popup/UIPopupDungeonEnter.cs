using Common.Global;
using Common.UIObject;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup
{
    public class UIPopupDungeonEnter : PopupBase
    {
        public Action<int> OnEnter { get; set; }
        private int Id= 0;

        public bool Init(int id)
        {
            Id = id;
            return true;
        }

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
            if(name == "Button - Close")
            {
                Close();
            }
            else if (name == "Button - Ok")
            {
                if(OnEnter != null)
                {
                    OnEnter(Id);
                }
            }
        }
    }
}
