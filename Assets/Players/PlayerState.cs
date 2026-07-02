using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [Header("Player Information")]
    [SerializeField] private int playerNumber;

    [Header("Gameplay")]
    [SerializeField] private int penalties = 0;

    public int PlayerNumber => playerNumber;
    public int Penalties => penalties;

    public void AddPenalty()
    {
        penalties++;
    }

    public void ResetPenalties()
    {
        penalties = 0;
    }
}