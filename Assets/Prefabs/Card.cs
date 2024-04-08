using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    public string suit;
    public string rank;

    public Card(string suit, string rank)
    {
        this.suit = suit;
        this.rank = rank;
    }
}

