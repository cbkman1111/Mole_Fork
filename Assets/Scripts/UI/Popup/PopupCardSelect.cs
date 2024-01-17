using System;
using Common.UIObject;
using UnityEngine.UI;
using Gostop;

namespace UI.Popup
{
    public class PopupCardSelect : PopupBase
    {
        Action<Card> OnSelect = null;
        Card cardA = null;
        Card cardB = null;

        public bool Init(Card a, Card b, Action<Card> select)
        {
            cardA = a;
            cardB = b;
            OnSelect = select;

            SetSprite("Image - Card A", cardA.GetSprite());
            SetSprite("Image - Card B", cardB.GetSprite());

            return true;
        }

        protected override void OnClick(Button button)
        {
            string name = button.name;

            if (name == "Button - A")
            {
                OnSelect?.Invoke(cardA);

                Close();
            }
            else if (name == "Button - B")
            {
                OnSelect?.Invoke(cardB);

                Close();
            }
        }
    }
}
