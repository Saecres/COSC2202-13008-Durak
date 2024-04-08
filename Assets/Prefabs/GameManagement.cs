using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public List<Player> players = new List<Player>();

    void Start()
    {
        InitializePlayers(2); //initializing two players
        DealCards();
    }

    void InitializePlayers(int numberOfPlayers)
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            players.Add(new Player());
        }
    }

    void DealCards()
    {
        int cardsPerPlayer = 6;
        for (int card = 0; card < cardsPerPlayer; card++)
        {
            foreach (Player player in players)
            {
                if (CardDatabase.cardList.Count > 0)
                {
                    player.AddCardToHand(CardDatabase.cardList[0]);
                    CardDatabase.cardList.RemoveAt(0);
                }
            }
        }

        // After dealing cards to players, update the UI to show the cards
        UIManager uiManager = FindObjectOfType<UIManager>(); // Find the UIManager in the scene
        uiManager.UpdateHandDisplay(players[0].hand); // Update for the first player only for now
    }
}
