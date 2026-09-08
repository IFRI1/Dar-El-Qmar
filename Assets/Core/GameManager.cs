using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    public GameObject player3;
    public GameObject player4;
    public Dealer dealer;
    public Camera cinematicCamera;
    public TurnManager turnManager;
    public PlayableDirector cinematicDirector;

    private bool gameOver = false;

    void Start()
    {
        // Activate only the players selected in the Start Menu.
        player3.SetActive(GameSettings.PlayerCount >= 3);
        player4.SetActive(GameSettings.PlayerCount >= 4);

        float cinematicDuration;

        if (GameSettings.PlayerCount == 2)
            cinematicDuration = 11f;
        else if (GameSettings.PlayerCount == 3)
            cinematicDuration = 14f;
        else
            cinematicDuration = 17f;

        Invoke(nameof(StartGameplay), cinematicDuration);
    }

    void StartGameplay()
    {
        Debug.Log("CINEMATIC FINISHED — STARTING GAMEPLAY");

        cinematicDirector.Stop();

        cinematicCamera.gameObject.SetActive(false);

        dealer.StartGame();

        turnManager.BeginGameplay();

        Debug.Log("GAMEPLAY STARTED");
    }

    public void EndGame(int winningPlayerIndex)
    {
        if (gameOver)
            return;

        gameOver = true;

        GameResult.WinnerPlayer = winningPlayerIndex + 1;

        Debug.Log(
            $"GAME OVER! Player {winningPlayerIndex + 1} wins!"
        );

        SceneManager.LoadScene("EndScene");
    }
}