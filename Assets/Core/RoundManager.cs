using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public CardDisplay cardDisplay;
    public ReactionManager reactionManager;
    public float timeBetweenCards = 3f;

    private float timer;
    private int currentCard;

    void Start()
    {
        timer = timeBetweenCards;
        DrawNewRound();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            DrawNewRound();
            timer = timeBetweenCards;
        }
    }

    void DrawNewRound()
    {
        currentCard = cardDisplay.DrawNewCard();
        reactionManager.OpenWindow(currentCard);
    }
}
