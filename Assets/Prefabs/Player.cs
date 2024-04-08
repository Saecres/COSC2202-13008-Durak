using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public List<Card> hand = new List<Card>();

    public void AddCardToHand(Card card)
    {
        hand.Add(card);
    }

    // Additional methods as needed for gameplay will go here
}
