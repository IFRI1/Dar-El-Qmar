using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Dealer dealer;
    [SerializeField] private TablePile tablePile;
    [SerializeField] private ReactionManager reactionManager;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardSpawnPoint;
    [SerializeField] private Transform spawnedCards;

    [Header("Gameplay")]
    [SerializeField] private float turnInterval = 3f;

    private float timer;
    private int currentPlayerIndex;
    private bool waitingForReaction;

    private List<PlayerPile> players = new List<PlayerPile>();

    void Start()
    {
        // Do not start gameplay while the cinematic is playing.
        enabled = false;
    }

    public void BeginGameplay()
    {
        players = dealer.GetPlayers();

        currentPlayerIndex = 0;
        timer = turnInterval;

        Debug.Log(
            $"TurnManager found {players.Count} active players."
        );

        enabled = true;
    }

    void Update()
    {
        if (waitingForReaction)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            PlayCurrentPlayerCard();
        }
    }

    private void PlayCurrentPlayerCard()
    {
        if (players.Count == 0)
            return;

        PlayerPile currentPlayer = players[currentPlayerIndex];

        Debug.Log(
            $"===== PLAYER {currentPlayer.PlayerNumber}'S TURN ====="
        );

        if (currentPlayer.CardsRemaining == 0)
        {
            gameManager.EndGame(currentPlayerIndex);
            return;
        }

        Card card = currentPlayer.PlayTopCard();

        if (card == null)
            return;

        tablePile.AddCard(card);
        SpawnCard(card);

        Debug.Log(
            $"Player {currentPlayer.PlayerNumber} played {card}. " +
            $"Cards remaining: {currentPlayer.CardsRemaining}"
        );

        if (IsSpecialCard(card))
        {
            waitingForReaction = true;

            Debug.Log(
                $"REACTION REQUIRED: {card}"
            );

            reactionManager.OpenWindow(
                card.Value,
                currentPlayerIndex
            );

            return;
        }

        if (currentPlayer.CardsRemaining == 0)
        {
            gameManager.EndGame(currentPlayerIndex);
            return;
        }

        MoveToNextPlayer();
    }

    private bool IsSpecialCard(Card card)
    {
        return card.Value == 1 ||
               card.Value == 10 ||
               card.Value == 11 ||
               card.Value == 12;
    }

    private void MoveToNextPlayer()
    {
        currentPlayerIndex++;

        if (currentPlayerIndex >= players.Count)
        {
            currentPlayerIndex = 0;

            Debug.Log(
                "===== ROUND COMPLETE ====="
            );
        }

        timer = turnInterval;
    }

    public void EndReactionPhase(int nextPlayerIndex)
    {
        waitingForReaction = false;

        if (nextPlayerIndex < 0 ||
            nextPlayerIndex >= players.Count)
        {
            Debug.LogError(
                $"Invalid next player index: {nextPlayerIndex}"
            );

            return;
        }

        currentPlayerIndex = nextPlayerIndex;

        Debug.Log(
            $"Reaction finished. Continuing with Player " +
            $"{players[currentPlayerIndex].PlayerNumber}"
        );

        timer = 0f;
    }

    public int GetPlayerCount()
    {
        return players.Count;
    }

    private void SpawnCard(Card card)
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-0.015f, 0.015f),
            tableSpawnHeight(),
            Random.Range(-0.015f, 0.015f)
        );

        GameObject newCard = Instantiate(
            cardPrefab,
            cardSpawnPoint.position + randomOffset,
            cardSpawnPoint.rotation,
            spawnedCards
        );

        CardVisual visual = newCard.GetComponent<CardVisual>();

        if (visual != null)
        {
            visual.DisplayCard(card);
        }
    }

    private float tableSpawnHeight()
    {
        return tablePile.CardCount * 0.0015f;
    }
}