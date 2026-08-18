using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndSceneController : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;

    void Start()
    {
        int winner = GameResult.WinnerPlayer;

        resultText.text = $"PLAYER {winner} WINS!";
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}