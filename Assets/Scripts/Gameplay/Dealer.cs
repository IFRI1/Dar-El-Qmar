using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    private List<Card> deck = new List<Card>();

    void Awake()
    {
        CreateDeck();
        ShuffleDeck();
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

    public void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            Card temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    //temporary debug output to verify shuffle order
    void Start()
    {
        foreach (Card card in deck)
        {
            Debug.Log(card);
        }
    }
}