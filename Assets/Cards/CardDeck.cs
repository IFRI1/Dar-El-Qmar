using System.Collections.Generic;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    private List<int> deck = new List<int>();

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
            deck.Add(Random.Range(1, 13));
        }
    }

    public int DrawCard()
    {
        if (deck.Count == 0)
        {
            CreateDeck();
        }

        int index = Random.Range(0, deck.Count);
        int value = deck[index];
        deck.RemoveAt(index);

        return value;
    }
}
