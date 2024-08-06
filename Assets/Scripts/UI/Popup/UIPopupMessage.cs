using System;
using Common.UIObject;
using UnityEngine.UI;
using Gostop;
using System.Collections.Generic;
using Common.Global;

namespace UI.Popup
{
    public class UIPopupMessage : PopupBase
    {
        private string msg;
        private MEC.CoroutineHandle coClose;

        public bool Init(string message)
        {
            msg = message;

            SetText("Text - Message", msg);

            coClose = MEC.Timing.RunCoroutine(CoClose());
            return true;
        }
        public override void Close()
        {
            coClose.IsRunning = false;
            base.Close();
        }

        private IEnumerator<float> CoClose()
        {
            yield return MEC.Timing.WaitForSeconds(1.0f);
            Close();
        }

        protected override void OnClick(Button button)
        {
            string name = button.name;

            if (name == "Button - Close")
            {
                Close();
            }
        }
    }
}
