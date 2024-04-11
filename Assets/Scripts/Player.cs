using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public List<Card> hand = new List<Card>();
    public bool isAI = false;
    public string name; // Added name property for identification

    public Player(string name, bool isAI = false)
    {
        this.name = name;
        this.isAI = isAI;
    }

    public void AddCardToHand(Card card)
    {
        hand.Add(card);
    }

    public void RemoveCardFromHand(Card card)
    {
        if (hand.Contains(card))
        {
            hand.Remove(card);
        }
    }
}
