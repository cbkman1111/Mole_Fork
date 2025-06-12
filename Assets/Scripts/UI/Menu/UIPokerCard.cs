using Common.Global;
using Common.UIObject;
using Common.Utils;
using Poker;
using UnityEngine;

namespace Pocker
{
    public class UIPokerCard : UIBase
    {
        private  const string ImageFront = "Image-Front";
        private  const string ImageBack = "Image-Back";
        private Card Card = null;

        public bool SetCard(Card card)
        {
            Card = card;
            if (card == null)
            {
                SetActive(ImageFront, false);
                SetActive(ImageBack, false);
                return false;
            }
            
            string[] pathKinds = { "Clubs", "Hearts", "Diamond", "Spades" };
            string path = $"Sprites/{Card.Kind.ToString()} {Card.Value}";

            var sprite = ResourcesManager.Instance.LoadAddressable<Sprite>(path);
            SetSprite(ImageFront, sprite);

            if (sprite == null)
            {
                GiantDebug.LogError($"sprite is null. path = {path}");
            }

            SetActive(ImageFront, false);
            SetActive(ImageBack, true);
            return true;
        }
        
        public bool OpenCard()
        {
            if (Card == null)
            {
                return false;
            }

            SetActive(ImageFront, Card.Hidden == false);
            SetActive(ImageBack, Card.Hidden == true);
            return true;
        }
    }
}

