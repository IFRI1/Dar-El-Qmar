using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    public Toggle addPlayer3Toggle;
    public Toggle addPlayer4Toggle;

    void Start()
    {
        // Default state = 2 players
        addPlayer3Toggle.isOn = false;
        addPlayer4Toggle.isOn = false;
    }

    public void OnTogglePlayer3(bool isOn)
    {
        if (isOn)
            addPlayer4Toggle.isOn = false;
    }

    public void OnTogglePlayer4(bool isOn)
    {
        if (isOn)
            addPlayer3Toggle.isOn = false;
    }

    public void StartGame()
    {
        int playerCount = 2;

        if (addPlayer3Toggle.isOn)
            playerCount = 3;
        else if (addPlayer4Toggle.isOn)
            playerCount = 4;

        GameSettings.PlayerCount = playerCount;
        SceneManager.LoadScene("Game");
    }
}
