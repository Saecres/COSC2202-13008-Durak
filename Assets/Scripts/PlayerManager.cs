using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public UIManager uiManager;
    public GameManagement gameManagement;
    public CardDatabase cardDatabase;

    public List<Player> players { get; private set; } = new List<Player>();
    public Player CurrentAttacker { get; private set; }
    public Player CurrentDefender { get; private set; }
    public bool IsPlayerTurn { get; private set; }

    void Start()
    {

    }

    public void InitializePlayers(int numberOfPlayers, bool isSecondPlayerAI)
    {
        players.Clear();
        string playerName = PlayerPrefs.GetString("PlayerName", "Player 1");  // Default to "Player 1" if no name is set

        players.Add(new Player(playerName, false));  // First player uses the custom or default name

        if (isSecondPlayerAI)
        {
            players.Add(new Player("AI", true));  // Second player as AI
        }
        else
        {
            players.Add(new Player("Player 2", false));  // Second player as human
        }

        AssignInitialRoles();
    }

    private void AssignInitialRoles()
    {
        CurrentAttacker = players[0];
        CurrentDefender = players[1];
        IsPlayerTurn = true; // Start game with the first player's turn

        uiManager.UpdateHandDisplay(CurrentAttacker.hand);
        uiManager.UpdateOpponentHandDisplay(CurrentDefender.hand);
    }


    public void DealCards()
    {
        int cardsPerPlayer = 6;
        foreach (Player player in players)
        {
            while (player.hand.Count < cardsPerPlayer)
            {
                Card nextCard = cardDatabase.GetNextCard();  // Get the next card from the deck
                if (nextCard != null)
                {
                    player.AddCardToHand(nextCard);  // Add the card to the player's hand
                }
                else
                {
                    Debug.LogError("No more cards to deal");
                    break;  // Break if no more cards are available
                }
            }
            // Update UI after dealing cards
            uiManager.UpdateHandDisplay(player.hand);
        }
    }

}
