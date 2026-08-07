using UnityEngine;

[System.Serializable]
public class Card
{
    public Suit Suit;
    public int Value;
    public ReactionType ReactionType;

    public Card(Suit suit, int value)
    {
        Suit = suit;
        Value = value;

        switch (value)
        {
            case 1:
                ReactionType = ReactionType.Hands;
                break;

            case 10:
                ReactionType = ReactionType.Salute;
                break;

            case 11:
                ReactionType = ReactionType.HelloSir;
                break;

            case 12:
                ReactionType = ReactionType.HelloMadam;
                break;

            default:
                ReactionType = ReactionType.None;
                break;
        }
    }

    public override string ToString()
    {
        return $"{Value} of {Suit} ({ReactionType})";
    }
}