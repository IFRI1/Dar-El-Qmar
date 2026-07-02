using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player3;
    public GameObject player4;

    void Start()
    {
        if (GameSettings.PlayerCount >= 3)
            player3.SetActive(true);

        if (GameSettings.PlayerCount >= 4)
            player4.SetActive(true);
    }
}
