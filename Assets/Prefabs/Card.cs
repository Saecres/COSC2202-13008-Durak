using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    // Properties to store the suit and rank of the card
    public string CardSuit { get; private set; }
    public string CardRank { get; private set; }

    // We can also include a property for the card's sprite if needed
    public Sprite CardSprite { get; private set; }

    // Method to initialize the card's properties
    public void Initialize(string suit, string rank, Sprite sprite)
    {
        CardSuit = suit;
        CardRank = rank;
        CardSprite = sprite;

        // Update the sprite renderer with the correct card sprite
        GetComponent<SpriteRenderer>().sprite = CardSprite;
    }

    // A method to determine if this card beats another card in a trick
    //public bool Beats(Card otherCard)
    //{
        // Implement game-specific logic here to compare cards
        // Return true if this card beats the otherCard
    //}
}
