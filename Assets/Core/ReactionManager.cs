using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class ReactionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController[] players;
    [SerializeField] private PlayerInputHandler[] playerInputs;
    [SerializeField] private PlayerPile[] playerPiles;
    [SerializeField] private TablePile tablePile;
    [SerializeField] private Transform spawnedCards;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private TMP_Text[] reactionTexts;

    [Header("Reaction Settings")]
    [SerializeField] private int minimumSequenceLength = 2;
    [SerializeField] private int maximumSequenceLength = 4;
    [SerializeField] private float resultDelay = 1f;

    private readonly List<ReactionInput> currentSequence = new List<ReactionInput>();
    private readonly Dictionary<int, int> playerProgress = new Dictionary<int, int>();
    private readonly HashSet<int> eliminatedPlayers = new HashSet<int>();
    private readonly List<int> completionOrder = new List<int>();

    private bool windowOpen;
    private bool roundResolved;
    private int expectedAction;
    private int reactionPlayerIndex;
    private int nextPlayerIndex;
    private float reactionStartTime;

    // Start reaction
    public void OpenWindow(int cardValue, int playerIndex)
    {
        expectedAction = cardValue;
        reactionPlayerIndex = playerIndex;

        GenerateRandomSequence();
        ResetPlayers();

        string sequenceText = GetSequenceText();

        for (int i = 0; i < reactionTexts.Length; i++)
        {
            if (reactionTexts[i] != null && i < GameSettings.PlayerCount)
            {
                reactionTexts[i].text = sequenceText;
                reactionTexts[i].gameObject.SetActive(true);
            }
        }

        windowOpen = true;
        roundResolved = false;
        reactionStartTime = Time.time;

        Debug.Log($"REACTION STARTED - Card value: {cardValue}. Player {playerIndex + 1} played the special card.");
        Debug.Log($"Required reaction: {GetSequenceText()}");
    }

    // Create the random sequence
    private void GenerateRandomSequence()
    {
        currentSequence.Clear();

        ReactionInput[] possibleInputs =
        {
            ReactionInput.FaceSouth, ReactionInput.FaceEast, ReactionInput.FaceWest, ReactionInput.FaceNorth,
            ReactionInput.L1, ReactionInput.L2, ReactionInput.L3,
            ReactionInput.R1, ReactionInput.R2, ReactionInput.R3,
            ReactionInput.DPadUp, ReactionInput.DPadDown, ReactionInput.DPadLeft, ReactionInput.DPadRight
        };

        int length = Random.Range(minimumSequenceLength, maximumSequenceLength + 1);

        for (int i = 0; i < length; i++)
        {
            ReactionInput randomInput = possibleInputs[Random.Range(0, possibleInputs.Length)];

            if (i > 0 && randomInput == currentSequence[i - 1])
            {
                i--;
                continue;
            }

            currentSequence.Add(randomInput);
        }
    }

    // Listen to every active player's controller
    private void SubscribeToPlayers()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            if (playerInputs[i] != null)
                playerInputs[i].OnReactionInput += HandlePlayerInput;
        }
    }

    private void UnsubscribeFromPlayers()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            if (playerInputs[i] != null)
                playerInputs[i].OnReactionInput -= HandlePlayerInput;
        }
    }

    private void HandlePlayerInput(PlayerInputHandler playerInput, ReactionInput input)
    {
        if (!windowOpen || roundResolved)
            return;

        int playerIndex = GetPlayerIndex(playerInput);

        if (playerIndex < 0 || playerIndex >= GameSettings.PlayerCount)
            return;

        // Ignore players who already finished or lost.
        if (eliminatedPlayers.Contains(playerIndex) || completionOrder.Contains(playerIndex))
            return;

        ProcessInput(playerIndex, input);
    }

    private int GetPlayerIndex(PlayerInputHandler playerInput)
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            if (playerInputs[i] == playerInput)
                return i;
        }

        return -1;
    }

    // Check the player's next input
    private void ProcessInput(int playerIndex, ReactionInput input)
    {
        if (!windowOpen || roundResolved)
            return;

        if (!playerProgress.ContainsKey(playerIndex))
            playerProgress[playerIndex] = 0;

        int progress = playerProgress[playerIndex];

        if (progress >= currentSequence.Count)
            return;

        // Wrong input means immediate elimination.
        if (input != currentSequence[progress])
        {
            Debug.Log($"Player {playerIndex + 1} performed the WRONG reaction.");
            ResolveIncorrectReaction(playerIndex);
            return;
        }

        playerProgress[playerIndex]++;

        Debug.Log($"Player {playerIndex + 1} correct input: {input} ({playerProgress[playerIndex]}/{currentSequence.Count})");

        // Sequence completed.
        if (playerProgress[playerIndex] >= currentSequence.Count)
        {
            float completionTime = Time.time - reactionStartTime;

            completionOrder.Add(playerIndex);

            Debug.Log($"Player {playerIndex + 1} COMPLETED the reaction in {completionTime:F3} seconds.");
            Debug.Log($"Player {playerIndex + 1} is currently #{completionOrder.Count}.");

            ResolveCorrectReaction(playerIndex);
        }
    }

    // Handle a correct sequence
    private void ResolveCorrectReaction(int playerIndex)
    {
        int playerCount = GameSettings.PlayerCount;

        // In 2-player mode, first correct player wins immediately.
        if (playerCount == 2)
        {
            int loserIndex = playerIndex == 0 ? 1 : 0;

            roundResolved = true;
            windowOpen = false;

            Debug.Log($"Player {playerIndex + 1} WINS the reaction!");
            Debug.Log($"Player {loserIndex + 1} LOSES the reaction!");

            PlayLoseAnimation(loserIndex);

            CollectTable(loserIndex);

            nextPlayerIndex = loserIndex;
            Invoke(nameof(FinishReaction), resultDelay);
            return;
        }

        // In 3/4-player mode, the first correct player takes the next winner position.
        CheckMultiPlayerResolution();
    }

    // Handle a wrong input
    private void ResolveIncorrectReaction(int loserIndex)
    {
        if (roundResolved)
            return;

        eliminatedPlayers.Add(loserIndex);

        Debug.Log($"Player {loserIndex + 1} is ELIMINATED because they performed the wrong reaction.");

        // The player who made the mistake dies immediately.
        PlayLoseAnimation(loserIndex);

        // In 2-player mode, the other player wins immediately.
        if (GameSettings.PlayerCount == 2)
        {
            int winnerIndex = loserIndex == 0 ? 1 : 0;

            roundResolved = true;
            windowOpen = false;

            Debug.Log($"Player {winnerIndex + 1} WINS because Player {loserIndex + 1} made a wrong input.");

            CollectTable(loserIndex);

            nextPlayerIndex = loserIndex;
            Invoke(nameof(FinishReaction), resultDelay);
            return;
        }

        // In 3/4-player mode, the remaining players continue.
        CheckMultiPlayerResolution();
    }

    // Resolve 3/4-player rounds
    private void CheckMultiPlayerResolution()
    {
        int playerCount = GameSettings.PlayerCount;
        int resolvedPlayers = completionOrder.Count + eliminatedPlayers.Count;
        int activePlayers = playerCount - resolvedPlayers;

        // If only one player remains, that player is the loser.
        if (activePlayers == 1)
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (!completionOrder.Contains(i) && !eliminatedPlayers.Contains(i))
                {
                    int loserIndex = i;

                    roundResolved = true;
                    windowOpen = false;

                    Debug.Log($"Player {i + 1} is the final remaining player and LOSES.");

                    PlayLoseAnimation(loserIndex);

                    // The first completed player wins the round.
                    int winnerIndex = completionOrder.Count > 0 ? completionOrder[0] : -1;

                    if (winnerIndex >= 0)
                    {
                        CollectTable(loserIndex);
                        nextPlayerIndex = loserIndex;
                    }
                    else
                    {
                        CollectTable(i);
                        nextPlayerIndex = i;
                    }

                    Invoke(nameof(FinishReaction), resultDelay);
                    return;
                }
            }
        }
    }

    // Trigger the death animation on the correct character
    private void PlayLoseAnimation(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= playerInputs.Length)
            return;

        if (playerInputs[playerIndex] != null)
        {
            Debug.Log($"Player {playerIndex + 1} -> PLAYING LOSE ANIMATION");
            playerInputs[playerIndex].PlayLoseAnimation();
        }
        else
        {
            Debug.LogError($"Player {playerIndex + 1} has no PlayerInputHandler assigned.");
        }
    }

    // Move the table cards to the winner
    private void CollectTable(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= playerPiles.Length)
            return;

        List<Card> cards = tablePile.GetAllCards();

        if (cards.Count == 0)
        {
            Debug.Log("Table is empty.");
            return;
        }

        playerPiles[playerIndex].AddCards(cards);
        tablePile.Clear();
        tablePile.ClearVisualCards(spawnedCards);

        Debug.Log($"Player {playerIndex + 1} collected {cards.Count} cards.");
    }

    private void FinishReaction()
    {
        for (int i = 0; i < reactionTexts.Length; i++)
        {
            if (reactionTexts[i] != null)
                reactionTexts[i].gameObject.SetActive(false);
        }

        playerProgress.Clear();
        completionOrder.Clear();
        eliminatedPlayers.Clear();

        roundResolved = false;
        windowOpen = false;

        turnManager.EndReactionPhase(nextPlayerIndex);
    }

    // Reset players for a new reaction
    private void ResetPlayers()
    {
        playerProgress.Clear();
        completionOrder.Clear();
        eliminatedPlayers.Clear();

        for (int i = 0; i < playerInputs.Length; i++)
        {
            if (playerInputs[i] != null)
                playerInputs[i].ResetInput();
        }
    }

    private string GetSequenceText()
    {
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < currentSequence.Count; i++)
        {
            if (i > 0)
                builder.Append(" + ");

            builder.Append(GetInputDisplayName(currentSequence[i]));
        }

        return builder.ToString();
    }

    private string GetInputDisplayName(ReactionInput input)
    {
        switch (input)
        {
            case ReactionInput.FaceSouth: return "X";
            case ReactionInput.FaceEast: return "O";
            case ReactionInput.FaceWest: return "□";
            case ReactionInput.FaceNorth: return "△";
            case ReactionInput.L1: return "L1";
            case ReactionInput.L2: return "L2";
            case ReactionInput.L3: return "L3";
            case ReactionInput.R1: return "R1";
            case ReactionInput.R2: return "R2";
            case ReactionInput.R3: return "R3";
            case ReactionInput.DPadUp: return "↑";
            case ReactionInput.DPadDown: return "↓";
            case ReactionInput.DPadLeft: return "←";
            case ReactionInput.DPadRight: return "→";
        }

        return input.ToString();
    }

    private void Start()
    {
        SubscribeToPlayers();
    }

    private void OnDestroy()
    {
        UnsubscribeFromPlayers();
    }
}