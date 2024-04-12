using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public PlayerManager playerManager;
    public AIManager aiManager;
    public UIManager uiManager;
    public GameRules gameRules;
    public CardDatabase cardDatabase;

    public List<Player> players = new List<Player>();
    public List<Card> playArea = new List<Card>();
    private List<Card> discardPile = new List<Card>();

    public bool isPlayerTurn;
    public Player currentAttacker;
    public Player currentDefender;
    private bool didDefenderPickUp = false;

    void Start()
    {
        InitializeGame();
    }

    // Initializes players, deals cards, and sets initial roles.
    void InitializeGame()
    {
        playerManager.InitializePlayers(2, GameSettings.IsPlayingAgainstAI);
        playerManager.DealCards();
        AssignInitialRoles();
        uiManager = FindObjectOfType<UIManager>();
        uiManager.UpdateHandDisplay(currentAttacker.hand);
        uiManager.UpdateOpponentHandDisplay(currentDefender.hand);
    }


    // Assigns initial attacker and defender roles.
    // Modify AssignInitialRoles to use the player manager's list
    void AssignInitialRoles()
    {
        if (playerManager.players.Count >= 2)
        {
            currentAttacker = playerManager.players[0];
            currentDefender = playerManager.players[1];
        }
        else
        {
            Debug.LogError("Not enough players initialized");
        }
    }


    public Player GetCurrentAttacker()
    {
        return currentAttacker;
    }

    public Player GetCurrentDefender()
    {
        return currentDefender;
    }

    public List<Card> GetPlayArea()
    {
        return playArea;
    }

    // Handles a player's attack attempt.
    public void HandleAttack(Player player, Card cardToPlay)
    {
        if (gameRules.CanPlayerAttackWithCard(cardToPlay, playArea))
        {
            ExecuteAttack(player, cardToPlay);
            if (currentDefender.isAI)
            {
                aiManager.AI_Defend();  // AI takes its defense action automatically.
            }
            else
            {
                // Prompt the human player to take their defensive turn using the new method in GameRules
                gameRules.PromptPlayerForDefense(currentDefender, playArea.LastOrDefault());
            }
        }
        else
        {
            Debug.LogError("Invalid attack move.");
        }
    }




    // Executes an attack, updates play area and UI.
    void ExecuteAttack(Player player, Card cardToPlay)
    {
        playArea.Add(cardToPlay);
        player.RemoveCardFromHand(cardToPlay);
        uiManager.MoveCardToPlayArea(cardToPlay);
        uiManager.UpdateOpponentHandDisplay(currentDefender.hand);
    }

    // Handles a player's defense attempt.
    public void HandleDefense(Player player, Card cardToPlay)
    {
        Card attackingCard = playArea.LastOrDefault();
        if (gameRules.CanPlayerDefendWithCard(attackingCard, cardToPlay))
        {
            ExecuteDefense(player, cardToPlay);
            if (gameRules.CheckForFollowUpAttack(currentAttacker, playArea))
            {
                gameRules.PromptPlayerForFollowUpAttack();
            }
            else
            {
                EndTurn();
            }
        }
        else
        {
            Debug.LogError("Invalid defense move.");
        }
    }



    // Executes a defense, updates play area and UI.
    void ExecuteDefense(Player player, Card cardToPlay)
    {
        playArea.Add(cardToPlay);
        player.RemoveCardFromHand(cardToPlay);
        uiManager.MoveCardToPlayArea(cardToPlay);
        uiManager.UpdateHandDisplay(player.hand);
    }

    // Ends the current turn and prepares for the next.
    public void EndTurn()
    {
        if (!CanAttackerContinue() || DidDefenderPickUp())
        {
            playerManager.SwitchRoles();
            DealCardsIfNeeded();
        }
        didDefenderPickUp = false;
        InitiateNextTurn();
    }

    // Checks if the attacker has any valid moves left.
    public bool CanAttackerContinue()
    {
        return currentAttacker.hand.Any(card => gameRules.CanPlayerAttackWithCard(card, playArea));
    }

    // Checks if the defender had to pick up cards.
    public bool DidDefenderPickUp()
    {
        return didDefenderPickUp;
    }

    // Deals cards to players if needed to ensure they have the correct number of cards.
    public void DealCardsIfNeeded()
    {
        int cardsPerPlayer = 6;
        foreach (Player player in players)
        {
            while (player.hand.Count < cardsPerPlayer && cardDatabase.cardList.Count > 0)
            {
                Card cardToDeal = cardDatabase.cardList[0];
                cardDatabase.cardList.RemoveAt(0);
                player.AddCardToHand(cardToDeal);
                uiManager.UpdateHandDisplay(player.hand);
            }
        }
        if (cardDatabase.cardList.Count == 0)
        {
            Debug.Log("Deck is empty.");
        }
    }

    // Initiates the next turn, checking if the current attacker has playable cards.
    public void InitiateNextTurn()
    {
        // Initiate action directly without pre-checking for playable cards.
        if (currentAttacker.isAI)
        {
            Debug.Log("AI's turn to attack or defend.");
            aiManager.AITakeAction();
        }
        else
        {
            Debug.Log("Player's turn to attack. Select a card to play.");
        }
    }


    // Handles a player forfeiting their turn.
    public void HandlePlayerForfeit(bool isAutomaticForfeit = false, bool isAI = false)
    {
        if (isAI)
        {
            Debug.Log("AI automatically forfeits the turn.");
            if (currentAttacker.isAI)
            {
                Debug.Log("AI forfeits attack. Discarding play area cards.");
                ForfeitAttackAndDiscard();
            }
            else
            {
                Debug.Log("AI forfeits defense. Picking up play area cards.");
                DefenderPicksUpCards();
            }
            EndTurn();
        }
        else
        {
            if (isPlayerTurn)
            {
                if (currentAttacker == players[0])
                {
                    Debug.Log(isAutomaticForfeit ? "Player automatically forfeits attack." : "Player manually forfeits attack.");
                    ForfeitAttackAndDiscard();
                    EndTurn();
                }
                else
                {
                    Debug.Log(isAutomaticForfeit ? "Player automatically picks up play area cards." : "Player manually picks up play area cards.");
                    DefenderPicksUpCards();
                    EndTurn();
                }
            }
        }
    }

    // Handles cleaning up after a player forfeits an attack.
    public void ForfeitAttackAndDiscard()
    {
        discardPile.AddRange(playArea);
        playArea.Clear();
        uiManager.ClearPlayArea();
        Debug.Log("Cards in play area are discarded.");
        playerManager.SwitchRoles();
        DealCardsIfNeeded();
        InitiateNextTurn();
    }

    // Handles transitions between attack turns.
    public void EndAttackTurnAndPrepareForNext(bool defenseSuccessful)
    {
        if (defenseSuccessful)
        {
            if (gameRules.CheckForFollowUpAttack(currentAttacker, playArea))
            {
                gameRules.PromptPlayerForFollowUpAttack();
            }
            else
            {
                EndTurn();
            }
        }
        else
        {
            DefenderPicksUpCards();
        }
    }
    public void DefenderPicksUpCards()
    {
        // Log the event of the defender picking up cards.
        Debug.Log($"{currentDefender.name} has to pick up the play area cards.");

        // Add all cards from the play area to the defender's hand.
        currentDefender.hand.AddRange(playArea);

        // Clear the play area after transferring the cards.
        playArea.Clear();

        // Update UI to reflect the changes.
        uiManager.UpdateHandDisplay(currentDefender.hand);
        uiManager.ClearPlayArea();

        EndTurn();
    }



}
