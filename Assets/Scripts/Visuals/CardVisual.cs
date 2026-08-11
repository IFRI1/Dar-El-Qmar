using UnityEngine;

/// <summary>
/// Displays the correct Moroccan card texture.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class CardVisual : MonoBehaviour
{
    private Renderer cardRenderer;
    private MaterialPropertyBlock propertyBlock;

    void Awake()
    {
        cardRenderer = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    public void DisplayCard(Card card)
    {
        string texturePath = $"Cards/{card.Suit}/{card.Value}";

        Texture texture = Resources.Load<Texture>(texturePath);

        if (texture == null)
        {
            Debug.LogError($"Could not load texture: {texturePath}");
            return;
        }

        cardRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetTexture("_BaseMap", texture);
        cardRenderer.SetPropertyBlock(propertyBlock);

        Debug.Log($"Displaying: {card.Suit} {card.Value}");
    }
}