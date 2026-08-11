using UnityEngine;

public class CardTest : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;

    void Start()
    {
        GameObject card = Instantiate(cardPrefab);

        card.transform.position = new Vector3(0f, 1f, 0f);

        CardVisual visual = card.GetComponent<CardVisual>();

        Card testCard = new Card(Suit.Coins, 1);

        visual.DisplayCard(testCard);
    }
}