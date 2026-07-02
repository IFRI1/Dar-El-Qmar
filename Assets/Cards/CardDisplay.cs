using TMPro;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    public CardDeck deck;
    public TextMeshPro cardText;

    public Card DrawNewCard()
    {
        Card card = deck.DrawCard();
        cardText.text = card.Value.ToString();

        return card;
    }
}