using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndSceneController : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI scoreText;

    void Start()
    {
        resultText.text = "PLAYER " + GameResult.LosingPlayer + " LOSES";

        string scores = "Penalties:\n";
        for (int i = 0; i < GameResult.Penalties.Length; i++)
        {
            scores += "P" + (i + 1) + ": " + GameResult.Penalties[i] + "\n";
        }

        scoreText.text = scores;
    }

    public void Replay()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
