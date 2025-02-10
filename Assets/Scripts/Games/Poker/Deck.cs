using Common.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poker
{
    public class Deck : MonoBehaviour
    {
        private Card[] StartCards = null;

        private Card[] Cards;
        private Queue<Card> DeckQueue = null;
        
        // suffle cards
        public void Shuffle()
        {
            const int MAX_CARD = 52;
            int index = 0;
            StartCards = new Card[MAX_CARD];

            for (Card.CardKind kind = Card.CardKind.Clubs; kind <= Card.CardKind.Spades; kind++)
            {
                StartCards[index] = new Card(kind, 2); index++;
                StartCards[index] = new Card(kind, 3); index++;
                StartCards[index] = new Card(kind, 4); index++;
                StartCards[index] = new Card(kind, 5); index++;
                StartCards[index] = new Card(kind, 6); index++;
                StartCards[index] = new Card(kind, 7); index++;
                StartCards[index] = new Card(kind, 8); index++;
                StartCards[index] = new Card(kind, 9); index++;
                StartCards[index] = new Card(kind, 10); index++;
                StartCards[index] = new Card(kind, 11); index++;
                StartCards[index] = new Card(kind, 12); index++;
                StartCards[index] = new Card(kind, 13); index++;
                StartCards[index] = new Card(kind, 14); index++;
            }
            
            Cards = new Card[StartCards.Length];
            for(int i = 0; i < StartCards.Length; i++)
            {
                Cards[i] = StartCards[i];
            }

            for (int i = 0; i < Cards.Length; i++)
            {
                var temp = Cards[i];
                int randomIndex = Random.Range(i, Cards.Length);
                Cards[i] = Cards[randomIndex];
                Cards[randomIndex] = temp;
            }
 

            DeckQueue = new();
            DeckQueue.Clear();
            for (int i = 0; i < Cards.Length; i++)
            {
                DeckQueue.Enqueue(Cards[i]);
            }
        }

        public Card DeQueue()
        {
            if (DeckQueue.Count == 0)
            {
                GiantDebug.Log("Deck is empty");
                return null;
            }

            return DeckQueue.Dequeue();
        }
    }
}

