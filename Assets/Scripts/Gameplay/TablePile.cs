using System.Collections.Generic;
using UnityEngine;

public class TablePile : MonoBehaviour
{
    private List<Card> cards = new List<Card>();

    public int CardCount => cards.Count;

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public void AddCards(IEnumerable<Card> newCards)
    {
        cards.AddRange(newCards);
    }

    public List<Card> GetAllCards()
    {
        return new List<Card>(cards);
    }

    public void Clear()
    {
        cards.Clear();
    }

    // Removes all displayed cards from the table.
    public void ClearVisualCards(Transform spawnedCards)
    {
        foreach (Transform child in spawnedCards)
        {
            Destroy(child.gameObject);
        }
    }
}