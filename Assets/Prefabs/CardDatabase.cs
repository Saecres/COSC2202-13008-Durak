using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static CardDatabase Instance;
    public static List<Card> cardList = new List<Card>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeDeck();
            ShuffleDeck();
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Another instance of CardDatabase was attempted to be created, but only one instance is allowed.");
        }
    }

    public string trumpSuit;

    void InitializeDeck()
    {
        // Clear the list to avoid duplicating cards if re-initialized
        cardList.Clear();

        // Adding cards to the deck
        string[] suits = new string[] { "hearts", "diamond", "clubs", "spade" };
        string[] ranks = new string[] { "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

        foreach (string suit in suits)
        {
            foreach (string rank in ranks)
            {
                cardList.Add(new Card(suit, rank));
            }
        }
        // Ensure the deck is not empty before choosing a trump suit
        if (cardList.Count > 0)
        {
            int trumpIndex = Random.Range(0, cardList.Count);
            trumpSuit = cardList[trumpIndex].suit;
            Debug.Log("Trump Suit: " + trumpSuit);
        }
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            Card temp = cardList[i];
            int randomIndex = Random.Range(0, cardList.Count);
            cardList[i] = cardList[randomIndex];
            cardList[randomIndex] = temp;
        }
    }
}
