using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public Transform playerHandTransform; 
    public GameObject cardPrefab;
    [SerializeField] private Sprite[] cardSprites;

    // Call this method to update the player's hand display
    public void UpdateHandDisplay(List<Card> playerHand)
    {
        // Clear out the old hand
        foreach (Transform child in playerHandTransform)
        {
            Destroy(child.gameObject);
        }

        // Instantiate a UI card for each card in the player's hand
        foreach (Card card in playerHand)
        {
            GameObject newCard = Instantiate(cardPrefab, playerHandTransform);
            Image cardImage = newCard.GetComponent<Image>();
            // This is where we set the card image based on your card's suit and rank.
            cardImage.sprite = GetCardSprite(card);
        }
    }

    private Sprite GetCardSprite(Card card)
    {
        // Construct the name of the sprite based on the card's suit and rank
        string spriteName = $"basic_{card.suit.ToLower()}_{card.rank}".ToLower();

        // Find the specific sprite by name in the manually assigned array
        foreach (Sprite sprite in cardSprites)
        {
            if (sprite.name.ToLower().Equals(spriteName))
            {
                return sprite;
            }
        }

        // If we don't find the sprite, log an error and return null
        Debug.LogError("Sprite not found for: " + spriteName);
        return null;
    }


}
