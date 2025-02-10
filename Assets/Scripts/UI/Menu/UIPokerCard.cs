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

            string[] pathKinds = { "Clubs", "Hearts", "Diamond", "Spades" };
            string path = $"Sprites/{Card.Kind.ToString()} {Card.Value}";

            var sprite = ResourcesManager.Instance.LoadInBuild<Sprite>(path);
            SetSprite(ImageFront, sprite);

            if (sprite == null)
            {
                GiantDebug.LogError($"sprite is null. path = {path}");
            }
            
            return true;
        }
        
        public bool OpenCard()
        {
            SetActive(ImageFront, true);
            SetActive(ImageBack, false);

            return true;
        }
    }
}

