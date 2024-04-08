using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();

    void Awake()
    {
        InitializeDeck();
        ShuffleDeck();
    }

    void InitializeDeck()
    {
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
