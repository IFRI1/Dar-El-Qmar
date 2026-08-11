using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OldManager : MonoBehaviour
{
    public PlayerController[] players;
    public PlayerState[] playerStates;
    public Transform[] penaltyAnchors;

    public GameObject penaltyPrefab;
    public int maxPenalties = 5;

    public float reactionWindow = 1.5f;

    //private int[] penalties;
    private Dictionary<int, float> playerReactions = new Dictionary<int, float>();

    private bool windowOpen = false;
    private bool roundResolved = false;
    private bool gameOver = false;

    private float windowTimer;
    private int expectedAction;

    void Start()
    {
        //penalties = new int[GameSettings.PlayerCount];
    }

    void Update()
    {
        if (gameOver) return;
        if (!windowOpen || roundResolved) return;

        windowTimer -= Time.deltaTime;

        if (windowTimer <= 0f)
        {
            ResolveNoReaction();
            return;
        }

        CheckInput();
    }

    public void OpenWindow(int cardValue)
    {
        expectedAction = GetExpectedAction(cardValue);

        playerReactions.Clear();
        windowOpen = true;
        roundResolved = false;
        windowTimer = reactionWindow;
    }

    void CheckInput()
    {
        float reactionTime = reactionWindow - windowTimer;

        if (Input.GetKeyDown(KeyCode.A))
            RegisterReaction(0, reactionTime);

        if (Input.GetKeyDown(KeyCode.L))
            RegisterReaction(1, reactionTime);

        if (GameSettings.PlayerCount > 2 && Input.GetKeyDown(KeyCode.Q))
            RegisterReaction(2, reactionTime);

        if (GameSettings.PlayerCount > 3 && Input.GetKeyDown(KeyCode.P))
            RegisterReaction(3, reactionTime);
    }

    void RegisterReaction(int playerIndex, float time)
    {
        if (roundResolved) return;
        if (playerReactions.ContainsKey(playerIndex)) return;

        playerReactions.Add(playerIndex, time);
        TriggerAnimation(playerIndex);

        ResolveRound(playerIndex);
    }

    void ResolveRound(int winnerIndex)
    {
        roundResolved = true;
        windowOpen = false;

        for (int i = 0; i < GameSettings.PlayerCount; i++)
        {
            if (i != winnerIndex)
                ApplyPenalty(i);
        }

        Invoke(nameof(ResetRound), 1.2f);
    }

    void ResolveNoReaction()
    {
        roundResolved = true;
        windowOpen = false;

        if (expectedAction != -1)
        {
            for (int i = 0; i < GameSettings.PlayerCount; i++)
                ApplyPenalty(i);
        }

        Invoke(nameof(ResetRound), 1.2f);
    }

    void ResetRound()
    {
        playerReactions.Clear();
        roundResolved = false;
    }

    void ApplyPenalty(int playerIndex)
    {
        if (gameOver) return;

        playerStates[playerIndex].AddPenalty();

        players[playerIndex].PlayLose();

        SpawnPenalty(
            penaltyAnchors[playerIndex],
            playerStates[playerIndex].Penalties
        );

        if (playerStates[playerIndex].Penalties >= maxPenalties)
        {
            EndGame(playerIndex);
        }
    }

    void SpawnPenalty(Transform anchor, int count)
    {
        Vector3 offset = new Vector3(0, 0, -0.15f * (count - 1));
        Instantiate(penaltyPrefab, anchor.position + offset, Quaternion.identity);
    }

    void TriggerAnimation(int playerIndex)
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

    int GetExpectedAction(int card)
    {
        switch (card)
        {
            case 1: return 1;
            case 10: return 10;
            case 11: return 11;
            case 12: return 12;
            default: return -1;
        }
    }

    void EndGame(int losingPlayerIndex)
    {
        if (gameOver) return;
        gameOver = true;

        GameResult.LosingPlayer = losingPlayerIndex + 1;

        GameResult.Penalties = new int[GameSettings.PlayerCount];

        for (int i = 0; i < GameSettings.PlayerCount; i++)
        {
            GameResult.Penalties[i] = playerStates[i].Penalties;
        }

        SceneManager.LoadScene("EndScene");
    }
}
