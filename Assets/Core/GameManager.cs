using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player3;
    public GameObject player4;
    public Dealer dealer;

    private bool gameOver = false;

    void Start()
    {
        if (GameSettings.PlayerCount >= 3)
            player3.SetActive(true);

        if (GameSettings.PlayerCount >= 4)
            player4.SetActive(true);

        dealer.StartGame();
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