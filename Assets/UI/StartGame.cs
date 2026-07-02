using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void Begin()
    {
        SceneManager.LoadScene("Game");
    }
}
