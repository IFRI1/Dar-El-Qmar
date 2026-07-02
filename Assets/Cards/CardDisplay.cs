using TMPro;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    public CardDeck deck;
    public TextMeshPro cardText;

    public int DrawNewCard()
    {
        int value = deck.DrawCard();
        cardText.text = value.ToString();
        return value;
    }
}
