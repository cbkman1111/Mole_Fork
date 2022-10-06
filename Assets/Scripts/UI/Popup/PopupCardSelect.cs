using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            if(OnSelect != null)
            {
                OnSelect(cardA);
            }

            Close();
        }
        else if (name == "Button - B")
        {
            if (OnSelect != null)
            {
                OnSelect(cardB);
            }

            Close();
        }
    }
}
