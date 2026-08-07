using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores all cards currently on the table.
/// Cards remain here until a player loses a reaction,
/// at which point the entire pile is collected.
/// </summary>
public class TablePile : MonoBehaviour
{
    private List<Card> cards = new List<Card>();

    /// <summary>
    /// Returns the number of cards currently on the table.
    /// </summary>
    public int CardCount => cards.Count;

    /// <summary>
    /// Adds a single card to the table.
    /// </summary>
    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    /// <summary>
    /// Adds multiple cards to the table.
    /// </summary>
    public void AddCards(IEnumerable<Card> newCards)
    {
        cards.AddRange(newCards);
    }

    /// <summary>
    /// Returns a copy of all cards currently on the table.
    /// </summary>
    public List<Card> GetAllCards()
    {
        return new List<Card>(cards);
    }

    /// <summary>
    /// Removes every card from the table.
    /// </summary>
    public void Clear()
    {
        cards.Clear();
    }
}