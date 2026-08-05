using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    private List<Card> deck = new List<Card>();

    void Awake()
    {
        CreateDeck();
    }

    void CreateDeck()
    {
        deck.Clear();

        Suit[] suits =
        {
            Suit.Coins,
            Suit.Cups,
            Suit.Swords,
            Suit.Clubs
        };

        int[] values =
        {
            1,2,3,4,5,6,7,10,11,12
        };

        foreach (Suit suit in suits)
        {
            foreach (int value in values)
            {
                deck.Add(new Card(suit, value));
            }
        }
    }
}