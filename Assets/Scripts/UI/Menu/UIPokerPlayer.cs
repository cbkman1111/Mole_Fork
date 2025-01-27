using Common.UIObject;
using Poker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Scenes.ScenePoker;

namespace Pocker
{
    public class UIPokerPlayer : UIBase
    {
        [SerializeField]
        private UIPokerCard[] Cards;

        public bool InitCards(Player player)
        {
            if(player.Hand == null || player.Hand.Count != 7)
            {
                return false;
            }

            for(int i = 0; i < player.Hand.Count; i++)
            {
                bool ret = Cards[i].SetCard(player.Hand[i]);
                if(ret == false)
                {
                    return false;
                }
            }

            return true;
        }

        public void OpenCards(int count)
        {
            for (int i = 0; i < Cards.Length; i++)
            {
                if(i <= count)
                    Cards[i].OpenCard();
            }
        }

        public void SetHandRank(HandRank rank)
        {
            SetTextMeshPro("Text-HandRank", rank.ToString());
        }
    }
}

