using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Dealer dealer;
    [SerializeField] private TablePile tablePile;
    [SerializeField] private ReactionManager reactionManager;

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardSpawnPoint;
    [SerializeField] private Transform spawnedCards;

    [Header("Gameplay")]
    [SerializeField] private float turnInterval = 3f;

    private float timer;

    // True while players are reacting to a special card.
    private bool waitingForReaction = false;

    // Cards played during the current turn.
    private readonly List<Card> currentTurnCards = new();

    void Start()
    {
        timer = turnInterval;
    }

    void Update()
    {
        // Pause the game while a reaction is taking place.
        if (waitingForReaction)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            PlayTurn();
            timer = turnInterval;
        }
    }

    private void PlayTurn()
    {
        List<PlayerPile> players = dealer.GetPlayers();

        currentTurnCards.Clear();

        Debug.Log("===== NEW TURN =====");

        foreach (PlayerPile player in players)
        {
            Card card = player.PlayTopCard();

            if (card == null)
                continue;

            tablePile.AddCard(card);
            currentTurnCards.Add(card);

            SpawnCard(card);

            Debug.Log($"Player {player.PlayerNumber} played {card}");
        }

        Debug.Log($"Cards currently on table: {tablePile.CardCount}");

        CheckReaction();
    }

    private void CheckReaction()
    {
        foreach (Card card in currentTurnCards)
        {
            if (card.Value == 1 ||
                card.Value == 10 ||
                card.Value == 11 ||
                card.Value == 12)
            {
                waitingForReaction = true;

                Debug.Log($"Reaction required! ({card})");

                reactionManager.OpenWindow(card.Value);

                return;
            }
        }
    }

    // Called by ReactionManager when the reaction phase has finished.
    public void EndReactionPhase()
    {
        waitingForReaction = false;
        timer = turnInterval;
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