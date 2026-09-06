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
    [SerializeField] private TMP_Text reactionText;

    [Header("Reaction Settings")]
    [SerializeField] private int minimumSequenceLength = 2;
    [SerializeField] private int maximumSequenceLength = 4;
    [SerializeField] private float resultDelay = 1f;

    private readonly List<ReactionInput> currentSequence =
        new List<ReactionInput>();

    private readonly Dictionary<int, int> playerProgress =
        new Dictionary<int, int>();

    private readonly Dictionary<int, float> completionTimes =
        new Dictionary<int, float>();

    private bool windowOpen;
    private bool roundResolved;

    private int expectedAction;
    private int reactionPlayerIndex;
    private int nextPlayerIndex;

    private float reactionStartTime;

    //Start reaction
    public void OpenWindow(int cardValue, int playerIndex)
    {
        expectedAction = cardValue;
        reactionPlayerIndex = playerIndex;

        GenerateRandomSequence();

        ResetPlayers();

        windowOpen = true;
        roundResolved = false;

        reactionStartTime = Time.time;

        Debug.Log($"REACTION STARTED - Card value: {cardValue}. " +$"Player {playerIndex + 1} played the special card.");

        Debug.Log($"Required reaction: {GetSequenceText()}");

        if (reactionText != null)
        { 
          reactionText.text = GetSequenceText();
        }

        TriggerAnimation(reactionPlayerIndex);
    }

    //Random sequence
    private void GenerateRandomSequence()
    {
        currentSequence.Clear();

        ReactionInput[] possibleInputs =
        {
            ReactionInput.FaceSouth,
            ReactionInput.FaceEast,
            ReactionInput.FaceWest,
            ReactionInput.FaceNorth,

            ReactionInput.L1,
            ReactionInput.L2,
            ReactionInput.L3,

            ReactionInput.R1,
            ReactionInput.R2,
            ReactionInput.R3,

            ReactionInput.DPadUp,
            ReactionInput.DPadDown,
            ReactionInput.DPadLeft,
            ReactionInput.DPadRight
        };

        int length = Random.Range(minimumSequenceLength,maximumSequenceLength + 1);

        for (int i = 0; i < length; i++)
        {
            ReactionInput randomInput =
                possibleInputs[
                    Random.Range(0, possibleInputs.Length)
                ];

            // Avoid generating the same input twice in a row.
            if (i > 0 && randomInput == currentSequence[i - 1])
            {
                i--;
                continue;
            }

            currentSequence.Add(randomInput);
        }
    }

    //Player input
    private void SubscribeToPlayers()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            if (playerInputs[i] != null)
            {
                playerInputs[i].OnReactionInput +=
                    HandlePlayerInput;
            }
        }
    }

    private void UnsubscribeFromPlayers()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            if (playerInputs[i] != null)
            {
                playerInputs[i].OnReactionInput -=
                    HandlePlayerInput;
            }
        }
    }

    private void HandlePlayerInput(PlayerInputHandler playerInput, ReactionInput input)
    {
        if (!windowOpen || roundResolved)
            return;

        int playerIndex = GetPlayerIndex(playerInput);

        if (playerIndex < 0)
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

    //Inputs
    private void ProcessInput(int playerIndex,ReactionInput input)
    {
        if (playerIndex >= GameSettings.PlayerCount)
            return;

        if (!playerProgress.ContainsKey(playerIndex))
            playerProgress[playerIndex] = 0;

        int progress = playerProgress[playerIndex];

        if (input != currentSequence[progress])
        {
            Debug.Log($"Player {playerIndex + 1} performed the WRONG reaction.");

            ResolveIncorrectReaction(playerIndex);
            return;
        }

        playerProgress[playerIndex]++;

        Debug.Log($"Player {playerIndex + 1} correct input: " + $"{input} ({playerProgress[playerIndex]}/" + $"{currentSequence.Count})");

        if (playerProgress[playerIndex] >= currentSequence.Count)
        {
            float completionTime =
                Time.time - reactionStartTime;

            completionTimes[playerIndex] =
                completionTime;

            Debug.Log($"Player {playerIndex + 1} completed the reaction " + $"in {completionTime:F3} seconds.");

            CheckForCompletion();
        }
    }

    //Completion
    private void CheckForCompletion()
    {
        int completedPlayers = 0;

        for (int i = 0;i < GameSettings.PlayerCount; i++)
        {
            if (completionTimes.ContainsKey(i))
                completedPlayers++;
        }

        if (completedPlayers < GameSettings.PlayerCount)
            return;

        int fastestPlayer = GetFastestPlayer();
        int lastPlayer = GetLastPlayer();

        Debug.Log($"Player {fastestPlayer + 1} was the fastest.");

        Debug.Log($"Player {lastPlayer + 1} was the last to react.");

        ResolveReaction(fastestPlayer,lastPlayer);
    }

    private int GetFastestPlayer()
    {
        int fastestPlayer = -1;
        float fastestTime = float.MaxValue;

        foreach (var result in completionTimes)
        {
            if (result.Value < fastestTime)
            {
                fastestTime = result.Value;
                fastestPlayer = result.Key;
            }
        }

        return fastestPlayer;
    }

    private int GetLastPlayer()
    {
        int lastPlayer = -1;
        float lastTime = float.MinValue;

        foreach (var result in completionTimes)
        {
            if (result.Value > lastTime)
            {
                lastTime = result.Value;
                lastPlayer = result.Key;
            }
        }

        return lastPlayer;
    }

    // Wrong reaction
    private void ResolveIncorrectReaction(int loserIndex)
    {
        roundResolved = true;
        windowOpen = false;

        Debug.Log( $"Player {loserIndex + 1} loses because " + $"they performed the wrong reaction." );

        ResolveLoser(loserIndex);
    }

    // Correct reaction
    private void ResolveReaction(int winnerIndex, int loserIndex)
    {
        roundResolved = true;
        windowOpen = false;

        Debug.Log($"Player {winnerIndex + 1} wins the reaction.");

        Debug.Log($"Player {loserIndex + 1} was last and loses.");

        ResolveLoser(loserIndex);
    }

    //Loser
    private void ResolveLoser(int loserIndex)
    {
        players[loserIndex].PlayLose();

        CollectTable(loserIndex);

        nextPlayerIndex = loserIndex;

        Invoke(nameof(FinishReaction), resultDelay);
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

        Debug.Log($"Player {playerIndex + 1} collected " + $"{cards.Count} cards.");
    }


    private void FinishReaction()
    {
        playerProgress.Clear();
        completionTimes.Clear();

        roundResolved = false;

        turnManager.EndReactionPhase(nextPlayerIndex);
    }


    private void ResetPlayers()
    {
        playerProgress.Clear();
        completionTimes.Clear();

        for (int i = 0;i < playerInputs.Length;i++)
        {
            if (playerInputs[i] != null)
            {
                playerInputs[i].ResetInput();
            }
        }
    }


    private string GetSequenceText()
    {
        StringBuilder builder = new StringBuilder();

        for (int i = 0;i < currentSequence.Count;i++)
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
            case ReactionInput.FaceSouth:
                return "X";

            case ReactionInput.FaceEast:
                return "O";

            case ReactionInput.FaceWest:
                return "□";

            case ReactionInput.FaceNorth:
                return "△";

            case ReactionInput.L1:
                return "L1";

            case ReactionInput.L2:
                return "L2";

            case ReactionInput.L3:
                return "L3";

            case ReactionInput.R1:
                return "R1";

            case ReactionInput.R2:
                return "R2";

            case ReactionInput.R3:
                return "R3";

            case ReactionInput.DPadUp:
                return "↑";

            case ReactionInput.DPadDown:
                return "↓";

            case ReactionInput.DPadLeft:
                return "←";

            case ReactionInput.DPadRight:
                return "→";
        }

        return input.ToString();
    }

    //Animation
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

    private void Start()
    {
        SubscribeToPlayers();
    }

    private void OnDestroy()
    {
        UnsubscribeFromPlayers();
    }
}