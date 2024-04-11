using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    public string suit;
    public string rank;

    // Constructor
    public Card(string suit, string rank)
    {
        this.suit = suit;
        this.rank = rank;
    }

    // IsTrump property
    public bool IsTrump
    {
        get { return suit.Equals(CardDatabase.Instance.trumpSuit, System.StringComparison.OrdinalIgnoreCase); }
    }

    // Add a RankValue property to get a numeric value of the card's rank
    public int RankValue
    {
        get
        {
            switch (rank)
            {
                case "6": return 6;
                case "7": return 7;
                case "8": return 8;
                case "9": return 9;
                case "10": return 10;
                case "J": return 11;
                case "Q": return 12;
                case "K": return 13;
                case "A": return 14;
                default: return 0; // Error handling or default case
            }
        }
    }
}


