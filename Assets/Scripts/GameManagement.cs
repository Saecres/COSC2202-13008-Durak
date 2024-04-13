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
    public List<Card> discardPile = new List<Card>();

    public bool isPlayerTurn;
    public Player currentAttacker;
    public Player currentDefender;
    public bool didDefenderPickUp = false;
    public int discardPileCount = 0;


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
        UIManager.Instance.UpdateDeckCountDisplay(cardDatabase.cardList.Count);
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
        Debug.Log($"{player.name} successfully attacks with {cardToPlay.rank} of {cardToPlay.suit}");
        playArea.Add(cardToPlay);
        player.RemoveCardFromHand(cardToPlay);
        uiManager.MoveCardToPlayArea(cardToPlay);
        UpdateGameState();
    }

    // Handles a player's defense attempt.
    public void HandleDefense(Player player, Card cardToPlay)
    {
        Debug.Log($"{player.name} attempts to defend with {cardToPlay.rank} of {cardToPlay.suit}");
        Card attackingCard = playArea.LastOrDefault();

        if (gameRules.CanPlayerDefendWithCard(attackingCard, cardToPlay))
        {
            ExecuteDefense(player, cardToPlay);
            // After a successful defense, check for follow-up attacks
            if (!didDefenderPickUp)  // Ensure we only continue if the defender hasn't picked up cards
            {
                bool canFollowUp = gameRules.CheckForFollowUpAttack(currentAttacker, playArea);
                if (currentAttacker.isAI)
                {
                    if (canFollowUp)
                    {
                        aiManager.AIInitiateFollowUpAttack(); // AI attempts a follow-up attack
                    }
                    else
                    {
                        EndTurn(); // Ends AI's turn if no follow-up is possible
                    }
                }
                else
                {
                    if (canFollowUp)
                    {
                        gameRules.PromptPlayerForFollowUpAttack(); // Prompt human player for follow-up attack
                    }
                    else
                    {
                        EndTurn(); // Ends player's turn if no follow-up is possible
                    }
                }
            }
            else
            {
                EndTurn(); // End the turn if the defender picked up cards
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
        Debug.Log($"{player.name} successfully defends with {cardToPlay.rank} of {cardToPlay.suit}");
        playArea.Add(cardToPlay);
        player.RemoveCardFromHand(cardToPlay);
        uiManager.MoveCardToPlayArea(cardToPlay);
        UpdateGameState();
    }

    // Ends the current turn and prepares for the next.
    public void EndTurn()
    {
        // This flag will determine whether the game state needs to be updated and turn needs to be initiated.
        bool endTurnConditionsMet = false;

        if (!CanAttackerContinue())
        {
            Debug.Log("Attacker cannot continue, discarding play area cards.");
            DiscardPlayAreaCards();
            discardPileCount += playArea.Count;
            SwitchRoles();  // Switch roles for the next turn
            didDefenderPickUp = false;  // Reset the flag if set
            endTurnConditionsMet = true;
        }
        else if (DidDefenderPickUp())
        {
            Debug.Log("Defender picked up cards, switching roles.");
            SwitchRoles();
            didDefenderPickUp = false;  // Reset the flag after switching roles
            endTurnConditionsMet = true;
        }

        // Only perform these actions if one of the end-turn conditions has been met
        if (endTurnConditionsMet)
        {
            DiscardPlayAreaCards();
            discardPileCount += playArea.Count;
            DealCardsIfNeeded();  // Ensure players have the right number of cards
            InitiateNextTurn();  // Start the next turn
            UpdateGameState();  // Update the game state to reflect changes
        }
    }

    private void DiscardPlayAreaCards()
    {
        if (playArea.Count > 0)
        {
            discardPile.AddRange(playArea);  // Add all play area cards to the discard pile
            playArea.Clear();  // Clear the play area
            Debug.Log("Moved play area cards to discard pile.");
        }
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
        bool updated = false;  // Flag to track if any changes were made

        if (cardDatabase.cardList.Count == 0)
        {
            return; // Exit the method if the deck is empty.
        }

        foreach (Player player in playerManager.players)
        {
            while (player.hand.Count < cardsPerPlayer && cardDatabase.cardList.Count > 0)
            {
                Debug.Log("Refilling Hand");
                Card cardToDeal = cardDatabase.cardList[0];
                cardDatabase.cardList.RemoveAt(0);
                player.AddCardToHand(cardToDeal);
                updated = true;  // Set flag to indicate an update occurred
            }
        }

        if (updated)
        {
            foreach (Player player in players)
            {
                uiManager.UpdateHandDisplay(player.hand);
            }
            UpdateGameState();
        }
    }




    // Initiates the next turn, checking if the current attacker has playable cards.
    public void InitiateNextTurn()
    {
        if (currentAttacker.isAI)
        {
            UpdateGameState();
            aiManager.AITakeAction();
        }
        else
        {
            Debug.Log("Player's turn to attack. Select a card to play.");
        }
        UpdateGameState(); // Ensure UI reflects current state at the beginning of each turn
    }


    // Handles a player forfeiting their turn.
    public void HandlePlayerForfeit(bool isAutomaticForfeit = false, bool isAI = false)
    {
        Debug.Log($"{(isAI ? "AI" : "Player")} {(isAutomaticForfeit ? "automatically" : "manually")} forfeits the turn.");

        // If the current attacker is forfeiting
        if (currentAttacker.isAI == isAI)
        {
            Debug.Log($"{(isAI ? "AI" : "Player")} forfeits attack. Discarding play area cards.");
            ForfeitAttackAndDiscard();
            DealCardsIfNeeded();
        }
        else  // The current defender is forfeiting
        {
            Debug.Log($"{(isAI ? "AI" : "Player")} forfeits defense. Picking up play area cards.");
            didDefenderPickUp = true;
            DefenderPicksUpCards();
            DealCardsIfNeeded();
        }
    }


    // Handles cleaning up after a player forfeits an attack.
    public void ForfeitAttackAndDiscard()
    {
        Debug.Log("Forfeiting attack and moving cards to discard pile.");
        if (playArea.Count > 0)
        {
            discardPile.AddRange(playArea);
            discardPileCount += playArea.Count;  // Update the discard pile counter
            Debug.Log($"Cards moved to discard pile. Total discard count: {discardPileCount}");
            playArea.Clear();
            uiManager.ClearPlayArea();
            UpdateGameState();
        }
        else
        {
            Debug.LogError("Attempted to discard from an empty play area.");
        }
        UpdateGameState();
        EndTurn();
    }


    // Handles transitions between attack turns.
    public void EndAttackTurnAndPrepareForNext(bool defenseSuccessful)
    {
        if (defenseSuccessful)
        {
            if (gameRules.CheckForFollowUpAttack(currentAttacker, playArea))
            {
                gameRules.PromptPlayerForFollowUpAttack();
                UpdateGameState();
            }
            else
            {
                EndTurn();
                UpdateGameState();
            }
        }
        else
        {
            didDefenderPickUp = true;
            DefenderPicksUpCards();
            UpdateGameState();
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
        uiManager.ClearPlayArea();
        UpdateGameState();

        EndTurn();
    }

    public void SwitchRoles()
    {
        // Swap the attacker and defender roles
        Player temp = currentAttacker;
        currentAttacker = currentDefender;
        currentDefender = temp;
        isPlayerTurn = !isPlayerTurn;  // Toggle the turn flag

        // Update UI to reflect the new roles
        uiManager.UpdateHandDisplay(currentAttacker.hand);
        uiManager.UpdateOpponentHandDisplay(currentDefender.hand);

        UpdateGameState();
    }


    private void UpdateUI()
    {
        // Check if the current attacker is the first player (typically the human player)
        if (currentAttacker == playerManager.players[0])
        {
            uiManager.UpdateHandDisplay(currentAttacker.hand); // Update player's hand
            uiManager.UpdateOpponentHandDisplay(currentDefender.hand); // Update opponent's hand
        }
        else
        {
            uiManager.UpdateHandDisplay(currentDefender.hand); // Update opponent's hand now on the player side
            uiManager.UpdateOpponentHandDisplay(currentAttacker.hand); // Update player's hand now on the opponent side
        }
        UIManager.Instance.UpdateDeckCountDisplay(cardDatabase.cardList.Count);
    }

    private void CheckForGameEnd()
    {
        if (cardDatabase.cardList.Count == 0)
        {
            Player winner = playerManager.players.FirstOrDefault(p => p.hand.Count == 0);
            if (winner != null)
            {
                Debug.Log($"{winner.name} has won the game by emptying their hand first!");
                EndGame(winner, false);
                return;
            }

            List<Player> playersWithCards = playerManager.players.Where(p => p.hand.Count > 0).ToList();
            if (playersWithCards.Count == 1)
            {
                Player durak = playersWithCards[0];
                Debug.Log($"{durak.name} is the Durak for having cards left!");
                EndGame(durak, true);
            }
            else if (playersWithCards.Count > 1)
            {
                Debug.Log("Game continues, more than one player still has cards.");
            }
            else
            {
                Debug.LogError("Unexpected state: No cards in any hands but no winner declared.");
            }
        }
    }




    private void EndGame(Player player, bool isDurak)
    {
        if (isDurak)
        {
            Debug.Log($"Game Over. {player.name} is the Durak.");
        }
        else
        {
            Debug.Log($"Congratulations! {player.name} has won the game.");
        }
    }


    public void UpdateGameState()
    {
        CheckForGameEnd();
        UpdateUI();
    }


}