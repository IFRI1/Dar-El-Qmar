using System.Collections.Generic;
using UnityEngine;

public class PlayerPile : MonoBehaviour
{
    [SerializeField] private int playerNumber;
    public int PlayerNumber => playerNumber;

    private Queue<Card> cards = new Queue<Card>();

    public int CardsRemaining
    {
        get { return cards.Count; }
    }

    public void AddCard(Card card)
    {
        cards.Enqueue(card);
    }

    public void AddCards(IEnumerable<Card> newCards)
    {
        foreach (Card card in newCards)
        {
            cards.Enqueue(card);
        }
    }

    public Card PlayTopCard()
    {
        if (cards.Count == 0)
            return null;

        return cards.Dequeue();
    }

    public void CollectCards(List<Card> collectedCards)
    {
        foreach (Card card in collectedCards)
        {
            cards.Enqueue(card);
        }
    }

    public void ClearPile()
    {
        cards.Clear();
    }
}