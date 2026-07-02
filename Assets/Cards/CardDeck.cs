using System.Collections.Generic;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    private List<Card> deck = new List<Card>();

    void Awake()
    {
        CreateDeck();
    }

    void CreateDeck()
    {
        deck.Clear();

        // 40-card deck, values 1–12
        for (int i = 0; i < 40; i++)
        {
            deck.Add(new Card(Random.Range(1, 13)));
        }
    }

    public Card DrawCard()
    {
        if (deck.Count == 0)
        {
            CreateDeck();
        }

        int index = Random.Range(0, deck.Count);

        Card card = deck[index];
        deck.RemoveAt(index);

        return card;
    }
}
