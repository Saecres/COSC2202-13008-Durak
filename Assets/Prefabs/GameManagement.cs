using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public List<Player> players = new List<Player>(); 
    public UIManager uiManager;
    private List<Card> discardPile = new List<Card>(); // Discard Pile List
    private List<Card> playArea = new List<Card>(); // Consolidate play area cards here

    public bool isPlayerTurn;
    public Player currentAttacker;
    public Player currentDefender;
    private bool didDefenderPickUp = false;

    void Start()
    {
        InitializeGame();
    }

    // This method initializes the game by setting up players, dealing cards, and assigning initial roles.
    // It ensures that the game starts with the correct setup.
    void InitializeGame()
    {
        InitializePlayers(2, GameSettings.IsPlayingAgainstAI); // Initializing players based on game mode
        DealCards();
        AssignInitialRoles();
        uiManager = FindObjectOfType<UIManager>();
        uiManager.UpdateHandDisplay(currentAttacker.hand); // Update UI for current attacker
        uiManager.UpdateOpponentHandDisplay(currentDefender.hand); // Update UI for opponent's hand
    }


    // This method creates player instances based on the specified number of players.
    // It initializes the players with their names and whether they are controlled by AI.
    void InitializePlayers(int numberOfPlayers, bool isSecondPlayerAI)
    {
        players.Add(new Player("Player 1", false)); // First player is always human
        // For the second player, check if it is an AI or a human based on the isSecondPlayerAI parameter
        if (isSecondPlayerAI)
        {
            players.Add(new Player("AI", true)); // Second player as AI
        }
        else
        {
            players.Add(new Player("Player 2", false)); // Second player as human
        }
    }

    // This method assigns initial roles to the players, such as who is the attacker and who is the defender.
    // It sets up the initial game state before the first turn.
    void AssignInitialRoles()
    {
        // Initial role assignment, could be randomized or based on a condition (e.g., who has the lowest trump)
        currentAttacker = players[0];
        currentDefender = players[1];
    }

    /// <summary>
    /// Game Operation Scripts (Player)
    /// </summary>

    // This method switches the roles of the current attacker and defender.
    // It toggles the turn flag to indicate whose turn it is.
    void SwitchRoles()
    {
        // Swap the current attacker and defender roles.
        var temp = currentAttacker;
        currentAttacker = currentDefender;
        currentDefender = temp;

        // Toggle the turn flag.
        isPlayerTurn = !isPlayerTurn;

        uiManager.UpdateHandDisplay(currentAttacker.hand);
        uiManager.UpdateOpponentHandDisplay(currentDefender.hand);
    }

    // This method deals cards to the players from the deck.
    // It ensures that each player has the correct number of cards to start the game.
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
    }

    // This method handles an attack initiated by a player.
    // It executes the attack, updates the game state, and prompts for defense or follow-up attack.
    public void HandleAttack(Player player, Card cardToPlay)
    {
        if (CanPlayerAttackWithCard(cardToPlay))
        {
            ExecuteAttack(player, cardToPlay);
            // Check for AI defense or follow-up attack based on game state.
            if (currentDefender.isAI)
            {
                AI_Defend();
            }
            else
            {
                PromptPlayerForDefenseOrFollowUp();
            }
        }
        else
        {
            Debug.LogError("Invalid attack move.");
        }
    }

    // This method executes an attack by adding the played card to the play area and removing it from the player's hand.
    // It updates the UI to reflect the attack and logs the action.
    void ExecuteAttack(Player player, Card cardToPlay)
    {
        playArea.Add(cardToPlay);
        player.RemoveCardFromHand(cardToPlay);
        uiManager.MoveCardToPlayArea(cardToPlay);

        // Check if the current attacker is AI to decide which hand to update.
        if (player.isAI)
        {
            uiManager.UpdateOpponentHandDisplay(player.hand);
        }
        else
        {
            // Update the player's hand UI.
            uiManager.UpdateHandDisplay(player.hand);
        }

        // Since the defender might not change, consider updating this outside of the conditionals if the defender's hand can be affected otherwise.
        uiManager.UpdateOpponentHandDisplay(currentDefender.hand);

        Debug.Log($"{player.name} attacks with: {cardToPlay.rank} of {cardToPlay.suit}");
    }


    // This method handles a defense initiated by a player.
    // It executes the defense, updates the game state, and handles follow-up actions if necessary.
    public void HandleDefense(Player player, Card cardToPlay)
    {
        Card attackingCard = playArea.LastOrDefault();
        if (CanPlayerDefendWithCard(attackingCard, cardToPlay))
        {
            ExecuteDefense(player, cardToPlay);
            // After a successful defense, check if the attacker can follow up.
            if (CheckForFollowUpAttack(currentAttacker))
            {
                PromptPlayerForFollowUpAttack();
            }
            else
            {
                EndTurn(); // End turn if no follow-up attack is possible.
            }
        }
        else
        {
            Debug.LogError("Invalid defense move.");
        }
    }

    // This method executes a defense by adding the defending card to the play area and removing it from the player's hand.
    // It updates the UI to reflect the defense and logs the action.
    void ExecuteDefense(Player player, Card cardToPlay)
    {
        playArea.Add(cardToPlay);
        player.RemoveCardFromHand(cardToPlay);
        uiManager.MoveCardToPlayArea(cardToPlay);  // This moves the defending card to the play area visually.

        Debug.Log($"{player.name} defends with: {cardToPlay.rank} of {cardToPlay.suit}");

        // After a defense action, regardless of who defended, update both hands.
        // This ensures the game state is accurately reflected in the UI.
        if (player.isAI)
        {
            // If the AI was defending, update its display. Since the AI is the current defender, we update the opponent hand.
            uiManager.UpdateOpponentHandDisplay(player.hand); //The AI is always considered the 'opponent'.
        }
        else
        {
            // If a human player was defending, update their hand display directly.
            uiManager.UpdateHandDisplay(player.hand);
        }

        // Now, update the attacker's hand, as it might change if the attacker can follow up.
        Player attackingPlayer = (player == currentAttacker) ? currentDefender : currentAttacker; // Determine the current attacker.
        uiManager.UpdateOpponentHandDisplay(attackingPlayer.hand); // Update the attacker's hand as well for visibility.

        // Check for follow-up actions or end the turn.
        if (!CheckForFollowUpAttack(currentAttacker))
        {
            EndTurn(); // End the turn if no follow-up attack is possible.
        }
    }



    // This method ends the current turn by preparing for the next turn.
    // It handles role switching, dealing cards if needed, and initiating the next turn.
    void EndTurn()
    {
        // Deal cards if needed
        DealCardsIfNeeded();

        // Check if the attacker can continue or if the defender picked up cards
        if (!CanAttackerContinue() || DidDefenderPickUp())
        {
            SwitchRoles(); // Switch roles based on game state
        }

        // Reset the flag for the next turn
        didDefenderPickUp = false;

        // Initiate the next turn, setting up for the next phase of the game
        InitiateNextTurn();
    }


    // This method checks if the attacker can continue their attack based on the game state.
    // It determines if the attacker has valid cards to play.
    bool CanAttackerContinue()
    {
        //Check if the attacker has more cards that can be played.
        return currentAttacker.hand.Any(card => CanPlayerAttackWithCard(card));
    }


    // This method checks if the defender picked up cards in the last defense attempt.
    // It returns true if the defender picked up cards, false otherwise.
    bool DidDefenderPickUp()
    {
        // Returns true if the defender had to pick up cards in the last defense attempt.
        return didDefenderPickUp;
    }

    // This method prompts the player to choose between defense or a follow-up attack.
    // It displays a UI prompt to the player to make a decision.
    void PromptPlayerForDefenseOrFollowUp()
    {
        // This method would involve UI logic to prompt the player.
        Debug.Log("Prompting player for defense or follow-up attack.");
        // uiManager.PromptForDefenseOrFollowUp();
    }

    // This method deals cards to players if needed, ensuring they have the correct number of cards.
    // It checks if players need to draw cards and updates the UI accordingly.
    void DealCardsIfNeeded()
    {
        int cardsPerPlayer = 6;

        foreach (Player player in players)
        {
            while (player.hand.Count < cardsPerPlayer && CardDatabase.cardList.Count > 0)
            {
                // Draw the top card from the deck.
                Card cardToDeal = CardDatabase.cardList[0];
                CardDatabase.cardList.RemoveAt(0); // Remove the card from the deck.

                // Add the card to the player's hand.
                player.AddCardToHand(cardToDeal);

                // Update the UI to reflect the new card in the player's hand.
                uiManager.UpdateHandDisplay(player.hand);
            }
        }

        // After dealing cards, check if the deck is empty to handle any game logic related to that.
        if (CardDatabase.cardList.Count == 0)
        {
            Debug.Log("Deck is empty.");
            // Handle empty deck scenario
        }
    }

    bool PlayerHasPlayableAttackCards(Player player)
    {
        return player.hand.Any(card => CanPlayerAttackWithCard(card));
    }

    bool PlayerHasPlayableDefenseCards(Player player, Card attackingCard)
    {
        return player.hand.Any(card => CanPlayerDefendWithCard(attackingCard, card));
    }




    // This method attempts an attack by a player, handling the attack logic and follow-up actions.
    // It executes an attack if valid and handles follow-up actions if possible.
    void AttemptAttack(Player player, Card cardToPlay, int cardIndex)
    {
        if (CanPlayerAttackWithCard(cardToPlay))
        {
            playArea.Add(cardToPlay);
            player.RemoveCardFromHand(cardToPlay);
            uiManager.MoveCardToPlayArea(cardToPlay);
            uiManager.RemoveCardFromHandDisplay(cardIndex);
            Debug.Log($"{player.name} attacks with: {cardToPlay.rank} of {cardToPlay.suit}");

            if (player.isAI)
            {
                // If AI is attacking, update the opponent hand UI to reflect the removed card.
                uiManager.RemoveCardFromOpponentHandDisplay(cardIndex); 
            }
            uiManager.UpdateOpponentHandDisplay(currentDefender.hand);

            // Determine if a follow-up attack is possible
            if (CheckForFollowUpAttack(player))
            {
                if (!player.isAI)
                {
                    PromptPlayerForFollowUpAttack(); // Prompt human player for follow-up attack
                }
                else
                {
                    AIInitiateFollowUpAttack(); // AI automatically attempts follow-up attack
                }
            }
            else
            {
                // End the attack turn and prepare for the next turn
                EndAttackTurnAndPrepareForNext(false); // Attack was not successful
            }
        }
        else
        {
            Debug.LogError("Invalid attack move.");
        }
    }




    // This method attempts a defense by a player, handling the defense logic and follow-up actions.
    // It executes a defense if valid and handles follow-up actions if necessary.
    void AttemptDefense(Player player, Card cardToPlay, int cardIndex)
    {
        Card attackingCard = playArea.LastOrDefault();
        if (attackingCard != null && CanPlayerDefendWithCard(attackingCard, cardToPlay))
        {
            // Successful defense logic
            playArea.Add(cardToPlay);
            player.RemoveCardFromHand(cardToPlay);
            uiManager.MoveCardToPlayArea(cardToPlay);
            uiManager.RemoveCardFromHandDisplay(cardIndex);

            // Check for a follow-up attack here, after a successful defense
            if (player == currentDefender && !player.isAI) // Ensuring it's the defense phase and player is defending
            {
                // Allowing for follow-up attack consideration
                CheckAndPromptForFollowUpAttack(currentAttacker);
            }
        }
        else
        {
            // Failed defense logic
            // Pass false to indicate the attack was successful
            EndAttackTurnAndPrepareForNext(false);
        }
    }

    void CheckAndPromptForFollowUpAttack(Player attacker)
    {
        if (CheckForFollowUpAttack(attacker))
        {
            PromptPlayerForFollowUpAttack();
        }
        else
        {
            EndTurn();
        }
    }

    void DefenderPicksUpCards()
    {
        // AI or player picks up the play area cards.
        currentDefender.hand.AddRange(playArea);
        playArea.Clear();
        Debug.Log("Switching roles after defense failure.");
        EndTurn();
    }

    void InitiateNextTurn()
    {
        // Before proceeding with the AI's or player's turn, check if the current attacker has playable cards.
        if (currentAttacker.isAI)
        {
            // If the attacker is AI, we check for playable cards specifically for attacking.
            // For AI, we'll directly attempt an action or handle automatic forfeiture inside AITakeAction() for clarity.
            Debug.Log("AI's turn to attack/Defend.");
            AITakeAction();
        }
        else
        {
            // For the player, explicitly check if there are any valid attack moves before prompting for action.
            if (!PlayerHasPlayableAttackCards(currentAttacker))
            {
                // Log and handle automatic forfeiture if no valid moves.
                Debug.Log("Player has no valid attack options. Automatically forfeiting turn.");
                HandlePlayerForfeit(isAutomaticForfeit: true);
            }
            else
            {
                Debug.Log("Player's turn to attack. Select a card to play.");
            }
        }
    }



    // This method checks if the player can defend against the current attack with the specified card.
    // It verifies if the defending card is valid based on game rules and the current attack.
    public bool CanPlayerAttackWithCard(Card card)
    {
        // If the play area is empty, any card can initiate an attack.
        if (!playArea.Any()) return true;

        // For a follow-up attack, the card rank must match any in the play area.
        return playArea.Any(playAreaCard => playAreaCard.rank == card.rank);
    }


    public bool CanPlayerDefendWithCard(Card attackingCard, Card defendingCard)
    {
        if (attackingCard == null) return false;

        bool isTrumpAttack = attackingCard.IsTrump;
        bool isTrumpDefense = defendingCard.IsTrump;

        // Correct comparison for trumps.
        if (isTrumpAttack && !isTrumpDefense) return false; // Can't defend a trump with a non-trump.
        if (!isTrumpAttack && isTrumpDefense) return true;  // Can always defend with a trump if the attack is not a trump.
        if (isTrumpAttack && isTrumpDefense) 
        {
            // If both are trumps, compare RankValue.
            return defendingCard.RankValue > attackingCard.RankValue;
        }
        // If neither is a trump, they must be of the same suit, and the defending card must have a higher rank.
        return defendingCard.suit.Equals(attackingCard.suit, System.StringComparison.OrdinalIgnoreCase) && defendingCard.RankValue > attackingCard.RankValue;
    }

    // This method handles the player forfeiting their turn, either in attack or defense.
    // It processes the forfeit action, updates the game state, and ends the turn.
    public void HandlePlayerForfeit(bool isAutomaticForfeit = false, bool isAI = false)
    {
        if (isAI)
        {
            // If the forfeit comes from the AI, this is an automatic process, as the player doesn't manually trigger AI actions.
            Debug.Log("AI automatically forfeits the turn due to no playable cards.");

            if (currentAttacker.isAI) // AI is the attacker and automatically forfeits
            {
                Debug.Log("AI forfeits attack. Cards in play area are discarded.");
                ForfeitAttackAndDiscard();
            }
            else // AI is the defender and automatically forfeits
            {
                Debug.Log("AI forfeits defense. Picking up play area cards.");
                DefenderPicksUpCards();
            }
            EndTurn(); // Transition to the next phase of the game.
        }
        else if (isPlayerTurn)
        {
            if (currentAttacker == players[0]) // Player is the attacker
            {
                if (isAutomaticForfeit)
                {
                    // The player has no valid attack options and automatically forfeits the attack.
                    Debug.Log("Player automatically forfeits attack due to no valid cards.");
                }
                else
                {
                    // The player manually decides to forfeit the attack.
                    Debug.Log("Player forfeits attack. Cards in play area are discarded.");
                }
                ForfeitAttackAndDiscard();
                EndTurn(); // Use EndTurn to manage the transition properly.
            }
            else // Player is the defender
            {
                if (isAutomaticForfeit)
                {
                    // The player has no valid defense options and automatically picks up the play area cards.
                    Debug.Log("Player automatically picks up play area cards due to no valid defense.");
                }
                else
                {
                    // The player manually decides to forfeit the defense.
                    Debug.Log("Player forfeits defense. Picking up play area cards.");
                }
                DefenderPicksUpCards();
                // Cards are moved from play area to player's hand
                players[0].hand.AddRange(playArea);
                playArea.Clear();
                EndTurn(); // Roles are switched and the next turn is properly initiated.
            }
        }
        else
        {
            Debug.LogError("HandlePlayerForfeit called during an unexpected state - this should be handled within the game's logic.");
        }
    }





    // This method discards the cards in the play area when the player forfeits their attack.
    // It moves the cards from the play area to the discard pile and prepares for the next turn.
    public void ForfeitAttackAndDiscard()
    {
        // Add play area cards to the discard pile when an attack is forfeited.
        discardPile.AddRange(playArea);
        playArea.Clear();
        uiManager.ClearPlayArea();

        Debug.Log("Player forfeits attack. Cards in play area are discarded.");

        // Directly switch roles after discarding cards to give the turn over to the AI.
        SwitchRoles();

        // Ensure both players have the minimum required cards after the turn ends.
        DealCardsIfNeeded();

        // Now initiate the next turn, which should correctly set it to the AI's turn to attack.
        InitiateNextTurn();
    }


    // This method checks if the attacker can make a follow-up attack based on the current game state.
    // It determines if the attacker has valid cards in their hand to continue the attack.
    bool CheckForFollowUpAttack(Player attacker)
    {
        return attacker.hand.Any(card => playArea.Any(playAreaCard => playAreaCard.rank == card.rank));
    }

    // This method prompts the player to make a follow-up attack if possible.
    // It displays a UI prompt to the player to continue their attack with another card.
    void PromptPlayerForFollowUpAttack()
    {
        Debug.Log("Player can make a follow-up attack. Select a card to play.");
        // Update the UI accordingly to prompt the player.
    }

    // This method handles the end of an attack turn, preparing for the next turn based on the outcome.
    // It handles the game state after a successful or failed defense, continuing or ending the turn accordingly.
    void EndAttackTurnAndPrepareForNext(bool defenseSuccessful)
    {
        if (defenseSuccessful)
        {
            // Logic for successful defense; might allow the attacker to continue if rules permit.
            if (CheckForFollowUpAttack(currentAttacker)) 
            {
                PromptPlayerForFollowUpAttack();
            }
            else
            {
                EndTurn(); 
            }
        }
        else
        {
            // Defense failed, handle accordingly.
            DefenderPicksUpCards(); // This already calls SwitchRoles and InitiateNextTurn.
        }
    }

    /// <summary>
    /// Place All AI Functions Here (Will be seperated later)
    /// </summary>

    // This method initiates a follow-up attack by the AI.
    // It selects a card from the AI's hand to continue the attack if possible.
    void AIInitiateFollowUpAttack()
    {
        // Find a card in AI's hand that can continue the attack
        var followUpCard = currentAttacker.hand.FirstOrDefault(card => CanPlayerAttackWithCard(card));
        if (followUpCard != null)
        {
            int cardIndex = currentAttacker.hand.IndexOf(followUpCard);
            AttemptAttack(currentAttacker, followUpCard, cardIndex); // AI attempts follow-up attack
        }
        else
        {
            EndAttackTurnAndPrepareForNext(false); // No valid follow-up attack card found, end turn
        }
    }

    void AITakeAction()
    {
        // If AI is the attacker, proceed to attack.
        if (currentAttacker.isAI) // No cards in play area, AI's turn to attack.
        {
            AIAttack();
        }
        // If AI is the defender and there are cards to defend against, proceed to defend.
        else if (currentDefender.isAI) // Cards in play area, AI's turn to defend.
        {
            AI_Defend();
        }
        else
        {
            Debug.LogError("AITakeAction called in an unexpected state.");
        }
    }

    void AIAttack()
    {
        Card attackCard = AIChooseCardToAttack(); 
        if (attackCard != null)
        {
            Debug.Log($"AI attacks with: {attackCard.rank} of {attackCard.suit}");
            HandleAttack(currentAttacker, attackCard); 
        }
        else
        {
            Debug.Log("AI has no valid attack options.");
            // Handle scenario where AI cannot make a valid attack.
        }
    }

    Card AIChooseCardToAttack()
    {
        // Simplest strategy: choose the first card
        return currentAttacker.hand.FirstOrDefault();
    }


    Card AIChooseCardToDefend()
    {
        Card attackingCard = playArea.LastOrDefault(); // This is the card to defend against.
        if (attackingCard == null) return null; // No card to defend against.

        List<Card> possibleDefenseCards = currentDefender.hand
            .Where(card => card.suit == attackingCard.suit && card.RankValue > attackingCard.RankValue) // Same suit, higher rank
            .ToList();

        // If no valid defense card is found, the AI forfeits the turn and picks up the cards.
        if (possibleDefenseCards.Count == 0)
        {
            DefenderPicksUpCards();
            return null; // Indicate that the AI cannot defend and has picked up the cards.
        }

        // Choose the lowest value card that can defend successfully.
        return possibleDefenseCards.OrderBy(card => card.RankValue).FirstOrDefault();
    }


    void AI_Defend()
    {
        // Choose a card to defend with
        Card defenseCard = AIChooseCardToDefend();
        if (defenseCard != null && CanPlayerDefendWithCard(playArea.LastOrDefault(), defenseCard))
        {
            // Execute defense
            AttemptDefense(currentDefender, defenseCard, currentDefender.hand.IndexOf(defenseCard));
        }
        else
        {
            // AI must pick up the play area cards if no valid defense is found.
            AIForfeitDefenseAndPickUpCards();
        }
    }

    // This method handles the AI's defense when it cannot defend against the current attack.
    // It makes the AI pick up the cards in the play area and ends its turn.
    void AIForfeitDefenseAndPickUpCards()
    {
        // AI picks up the play area cards, adding them to its hand.
        currentDefender.hand.AddRange(playArea);
        Debug.Log("AI cannot defend and picks up the play area cards.");

        playArea.Clear(); // Clear the play area after picking up the cards.
        uiManager.ClearPlayArea();

        // Set the flag to true since the defender picked up cards
        didDefenderPickUp = true;

        // Proceed to the next turn in the game flow, which will check this flag
        EndTurn();
    }

}