using UnityEngine;

public class PlayerSelectUI : MonoBehaviour
{
    public void Set2Players()
    {
        GameSettings.PlayerCount = 2;
    }

    public void Set3Players()
    {
        GameSettings.PlayerCount = 3;
    }

    public void Set4Players()
    {
        GameSettings.PlayerCount = 4;
    }
}
