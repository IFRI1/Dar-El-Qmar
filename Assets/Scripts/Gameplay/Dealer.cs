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

    public void StartGame()
    {
        DealCards();

        List<PlayerPile> players = GetActivePlayers();

        Debug.Log($"Players found: {players.Count}");

        foreach (PlayerPile player in players)
        {
            Debug.Log($"Player {player.PlayerNumber}: {player.CardsRemaining} cards");
        }
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
            1, 2, 3, 4, 5, 6, 7, 10, 11, 12
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

    private List<PlayerPile> GetActivePlayers()
    {
        PlayerPile[] allPlayers =
            Object.FindObjectsByType<PlayerPile>(FindObjectsSortMode.None);

        List<PlayerPile> activePlayers = new List<PlayerPile>();

        foreach (PlayerPile player in allPlayers)
        {
            if (player.PlayerNumber <= GameSettings.PlayerCount)
            {
                activePlayers.Add(player);
            }
        }

        activePlayers.Sort((a, b) => a.PlayerNumber.CompareTo(b.PlayerNumber));

        return activePlayers;
    }

    private void DealCards()
    {
        List<PlayerPile> players = GetActivePlayers();

        Debug.Log($"Players found while dealing: {players.Count}");

        int cardsPerPlayer = deck.Count / players.Count;
        int currentCard = 0;

        foreach (PlayerPile player in players)
        {
            player.ClearPile();

            for (int i = 0; i < cardsPerPlayer; i++)
            {
                player.AddCard(deck[currentCard]);
                currentCard++;
            }
        }
    }
}