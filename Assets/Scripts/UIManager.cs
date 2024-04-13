using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public PlayerManager playerManager;
    public GameManagement gameManagement;
    public GameRules gameRules;

    public Transform playerHandTransform;
    public Transform playAreaTransform;
    public Transform opponentHandTransform;

    public Text deckCountText;
    public Image discardPileImage;
    public Text discardPileCountText;

    public GameObject cardPrefab;
    [SerializeField] private Sprite[] cardSprites;
    public static UIManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Another instance of UIManager was created and will be ignored. Only one instance should exist.");
        }
    }


    public void UpdateHandDisplay(List<Card> playerHand)
    {
        // Clear out the old hand
        foreach (Transform child in playerHandTransform)
        {
            Destroy(child.gameObject);
        }

        // Instantiate a UI card for each card in the player's hand
        for (int i = 0; i < playerHand.Count; i++)
        {
            Card card = playerHand[i];
            GameObject newCard = Instantiate(cardPrefab, playerHandTransform);
            Image cardImage = newCard.GetComponent<Image>();
            cardImage.sprite = GetCardSprite(card); // Set the card image

            // Add a click event listener using EventTrigger
            EventTrigger trigger = newCard.GetComponent<EventTrigger>();
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            int index = i; // Avoid capturing the loop variable
            entry.callback.AddListener((data) => { OnCardSelected(index); });
            trigger.triggers.Add(entry);
        }
    }

    public void UpdateDiscardPileDisplay(List<Card> discardPile)
    {
        if (discardPile.Count > 0)
        {
            Card topCard = discardPile.Last(); // Assumes the last card is the top of the pile
            discardPileImage.sprite = GetCardSprite(topCard); // Update image to show the top card
            discardPileImage.gameObject.SetActive(true); // Ensure the image is visible
        }
        else
        {
            discardPileImage.gameObject.SetActive(false); // Hide if no cards are in the pile
        }

        // Update count text
        if (discardPileCountText != null)
        {
            discardPileCountText.text = discardPile.Count.ToString();
        }
    }

    public void UpdateOpponentHandDisplay(List<Card> opponentHand)
    {
        // Clear out the old hand
        foreach (Transform child in opponentHandTransform)
        {
            Destroy(child.gameObject);
        }

        // Choose sprite based on game mode
        foreach (Card card in opponentHand)
        {
            GameObject newCard = Instantiate(cardPrefab, opponentHandTransform);
            Image cardImage = newCard.GetComponent<Image>();
            // Determine if showing the back of the card or the face
            if (GameSettings.IsPlayingAgainstAI)
            {
                // Show card back for AI mode
                cardImage.sprite = GetBackSprite();
            }
            else
            {
                // Show card face for PVP mode
                cardImage.sprite = GetCardSprite(card);
            }
        }
    }


    private Sprite GetBackSprite()
    {
        string backSpriteName = "basic_back_1";
        foreach (Sprite sprite in cardSprites)
        {
            if (sprite.name.Equals(backSpriteName, System.StringComparison.OrdinalIgnoreCase))
            {
                return sprite;
            }
        }
        Debug.LogError("Back sprite not found: " + backSpriteName);
        return null;
    }





    public void RemoveCardFromHandDisplay(int cardIndex)
    {
        // Ensure the hand display is assigned
        if (playerHandTransform == null)
        {
            Debug.LogError("Hand display transform is not assigned.");
            return;
        }

        // Ensure the card index is valid
        if (cardIndex < 0 || cardIndex >= playerHandTransform.childCount)
        {
            Debug.LogError("Invalid card index.");
            return;
        }

        // Destroy the card game object at the specified index
        Destroy(playerHandTransform.GetChild(cardIndex).gameObject);
    }

    public void RemoveCardFromOpponentHandDisplay(int cardIndex)
    {
        // Ensure the opponent hand display is assigned
        if (opponentHandTransform == null)
        {
            Debug.LogError("Opponent hand display transform is not assigned.");
            return;
        }

        // Ensure the card index is valid
        if (cardIndex < 0 || cardIndex >= opponentHandTransform.childCount)
        {
            Debug.LogError("Invalid opponent card index.");
            return;
        }

        // Destroy the card game object at the specified index in the opponent's hand
        Destroy(opponentHandTransform.GetChild(cardIndex).gameObject);
    }



    public void OnCardSelected(int cardIndex)
    {
        // Validate the index first
        if ((gameManagement.isPlayerTurn && cardIndex >= gameManagement.currentAttacker.hand.Count) ||
            (!gameManagement.isPlayerTurn && cardIndex >= gameManagement.currentDefender.hand.Count))
        {
            Debug.LogError("Card index out of range.");
            return;
        }

        Card selectedCard = gameManagement.isPlayerTurn ?
            gameManagement.currentAttacker.hand[cardIndex] :
            gameManagement.currentDefender.hand[cardIndex];

        if (gameManagement.isPlayerTurn)
        {
            gameManagement.HandleAttack(gameManagement.currentAttacker, selectedCard);
        }
        else if (!gameManagement.isPlayerTurn && gameManagement.currentDefender == playerManager.players[0])
        {
            gameManagement.HandleDefense(gameManagement.currentDefender, selectedCard);
        }
        else
        {
            Debug.LogError("Card selected at an inappropriate time.");
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

    public void MoveCardToPlayArea(Card card)
    {
        GameObject newCard = Instantiate(cardPrefab, playAreaTransform);
        Image cardImage = newCard.GetComponent<Image>();
        cardImage.sprite = GetCardSprite(card); // Use existing method to get the sprite
    }

    public void ClearPlayArea()
    {
        foreach (Transform child in playAreaTransform)
        {
            Destroy(child.gameObject); // Remove all card GameObjects from the play area
        }
    }

    public void HidePlayAreaCards()
    {
        foreach (Transform child in playAreaTransform)
        {
            child.gameObject.SetActive(false); // Hide the card
        }
    }
    public void MoveCardToPlayerHand(GameObject card, Transform playerHandTransform)
    {
        card.transform.SetParent(playerHandTransform);
        card.transform.localPosition = Vector3.zero;
        card.SetActive(true);
    }

    public void OnForfeitButtonClicked()
    {
        gameManagement.HandlePlayerForfeit(isAutomaticForfeit: false, isAI: false);
    }

    public void UpdateDeckCountDisplay(int count)
    {
        if (deckCountText != null)
            deckCountText.text = count.ToString();
        else
            Debug.LogError("Deck count Text UI component is not assigned.");
    }
}