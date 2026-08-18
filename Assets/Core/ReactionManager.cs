using System.Collections.Generic;
using UnityEngine;

public class ReactionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController[] players;
    [SerializeField] private PlayerPile[] playerPiles;
    [SerializeField] private TablePile tablePile;
    [SerializeField] private Transform spawnedCards;
    [SerializeField] private TurnManager turnManager;

    [Header("Gameplay")]
    [SerializeField] private float resultDelay = 1f;

    private Dictionary<int, float> playerReactions =
        new Dictionary<int, float>();

    private bool windowOpen;
    private bool roundResolved;

    private int expectedAction;
    private int reactionPlayerIndex;
    private int nextPlayerIndex;

    void Update()
    {
        if (!windowOpen || roundResolved)
            return;

        CheckInput();
    }

    public void OpenWindow(int cardValue, int playerIndex)
    {
        expectedAction = cardValue;
        reactionPlayerIndex = playerIndex;

        playerReactions.Clear();

        windowOpen = true;
        roundResolved = false;

        Debug.Log(
            $"REACTION STARTED - Card value: {cardValue}. " +
            $"Player {playerIndex + 1} played the special card."
        );
    }

    private void CheckInput()
    {
        // Temporary keyboard controls.
        // These will later be replaced with analogue-stick gestures.

        if (Input.GetKeyDown(KeyCode.A))
        {
            RegisterCorrectReaction(0);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            RegisterCorrectReaction(1);
        }

        if (GameSettings.PlayerCount > 2 &&
            Input.GetKeyDown(KeyCode.Q))
        {
            RegisterCorrectReaction(2);
        }

        if (GameSettings.PlayerCount > 3 &&
            Input.GetKeyDown(KeyCode.P))
        {
            RegisterCorrectReaction(3);
        }
    }

    private void RegisterCorrectReaction(int playerIndex)
    {
        if (roundResolved)
            return;

        if (playerIndex >= GameSettings.PlayerCount)
            return;

        if (playerReactions.ContainsKey(playerIndex))
            return;

        float reactionTime = Time.time;

        playerReactions.Add(playerIndex, reactionTime);

        Debug.Log(
            $"Player {playerIndex + 1} performed the correct reaction."
        );

        TriggerAnimation(playerIndex);

        ResolveReaction(playerIndex);
    }

    private void ResolveReaction(int winnerIndex)
    {
        roundResolved = true;
        windowOpen = false;

        int loserIndex = GetLoser(winnerIndex);

        Debug.Log(
            $"Player {winnerIndex + 1} wins the reaction."
        );

        Debug.Log(
            $"Player {loserIndex + 1} loses and collects the table."
        );

        players[loserIndex].PlayLose();

        CollectTable(loserIndex);

        // The loser plays next.
        nextPlayerIndex = loserIndex;

        Invoke(
            nameof(FinishReaction),
            resultDelay
        );
    }

    private int GetLoser(int winnerIndex)
    {
        // Phase 1: two-player game.
        if (GameSettings.PlayerCount == 2)
        {
            return winnerIndex == 0 ? 1 : 0;
        }

        // Temporary fallback for 3/4 players.
        // Proper multiplayer reaction rules will be added next.
        for (int i = 0;
             i < GameSettings.PlayerCount;
             i++)
        {
            if (i != winnerIndex)
                return i;
        }

        return 0;
    }

    private void CollectTable(int playerIndex)
    {
        List<Card> cards = tablePile.GetAllCards();

        if (cards.Count == 0)
        {
            Debug.Log("Table is empty.");
            return;
        }

        playerPiles[playerIndex].AddCards(cards);

        tablePile.Clear();
        tablePile.ClearVisualCards(spawnedCards);

        Debug.Log(
            $"Player {playerIndex + 1} collected " +
            $"{cards.Count} cards."
        );
    }

    private void FinishReaction()
    {
        playerReactions.Clear();
        roundResolved = false;

        turnManager.EndReactionPhase(nextPlayerIndex);
    }

    private void TriggerAnimation(int playerIndex)
    {
        switch (expectedAction)
        {
            case 1:
                players[playerIndex].PlayHands();
                break;

            case 10:
                players[playerIndex].PlaySalute();
                break;

            case 11:
                players[playerIndex].PlayHelloSir();
                break;

            case 12:
                players[playerIndex].PlayHelloMadam();
                break;
        }
    }
}