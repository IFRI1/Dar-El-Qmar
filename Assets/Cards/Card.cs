using UnityEngine;

[System.Serializable]
public class Card
{
    public Suit Suit;
    public int Value;

    public Card(Suit suit, int value)
    {
        Suit = suit;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Value} of {Suit}";
    }
}