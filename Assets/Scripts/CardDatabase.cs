using UnityEngine;
using System.Collections.Generic;

public class CardDatabase : MonoBehaviour
{
    public static CardDatabase Instance;
    public List<Card> cardList = new List<Card>(); // Changed from static to instance variable
    public string trumpSuit; // Added trump suit property


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDeck();
            ShuffleDeck();
            SetTrumpSuit();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void InitializeDeck()
    {
        cardList.Clear();
        string[] suits = { "hearts", "diamond", "clubs", "spade" };
        string[] ranks = { "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

        foreach (var suit in suits)
        {
            foreach (var rank in ranks)
            {
                cardList.Add(new Card(suit, rank));
            }
        }
        Debug.Log("Deck initialized with " + cardList.Count + " cards");
    }


    public void ShuffleDeck()
    {
        int n = cardList.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            Card value = cardList[k];
            cardList[k] = cardList[n];
            cardList[n] = value;
        }
    }

    public void SetTrumpSuit()
    {
        if (cardList.Count > 0)
        {
            int randomIndex = Random.Range(0, cardList.Count);
            trumpSuit = cardList[randomIndex].suit; // Assign a random card's suit as the trump
            Debug.Log("Trump Suit: " + trumpSuit);
            UIManager.Instance.UpdateTrumpSuitDisplay();
        }
    }

    public Card GetNextCard()
    {
        if (cardList.Count > 0)
        {
            Card card = cardList[0];
            cardList.RemoveAt(0);
            return card;
        }
        return null;
    }
}
