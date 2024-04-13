using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AIManager : MonoBehaviour
{
    public GameRules gameRules;
    public UIManager uiManager;
    public GameManagement gameManagement;

    public Player GetCurrentAttacker()
    {
        return gameManagement.currentAttacker;
    }

    public Player GetCurrentDefender()
    {
        return gameManagement.currentDefender;
    }

    public List<Card> GetPlayArea()
    {
        return gameManagement.playArea;
    }


    // Initiates a follow-up attack by the AI if a valid card is found.
    public void AIInitiateFollowUpAttack()
    {
        Player currentAttacker = GetCurrentAttacker();
        List<Card> playArea = GetPlayArea();
        Card followUpCard = currentAttacker.hand.FirstOrDefault(card => gameRules.CanPlayerAttackWithCard(card, playArea));

        if (followUpCard != null)
        {
            gameManagement.HandleAttack(currentAttacker, followUpCard);
        }
        else
        {
            Debug.Log("AI has no valid follow-up attack options.");
            gameManagement.HandlePlayerForfeit(); // Ensure this call is correct - consider the state before ending the turn
        }
    }


    // Decides AI's actions based on current game state, handling attack or defense.
    public void AITakeAction()
    {
        Player currentAttacker = gameManagement.GetCurrentAttacker();
        Player currentDefender = gameManagement.GetCurrentDefender();
        List<Card> playArea = gameManagement.GetPlayArea();

        if (currentAttacker.isAI && !playArea.Any()) // AI's turn to attack
        {
            AIAttack();
        }
        else if (currentDefender.isAI && playArea.Any()) // AI's turn to defend
        {
            AI_Defend();
        }
        else
        {
            Debug.LogError("AITakeAction called in an unexpected state.");
        }
    }

    // AI attacks by selecting the first playable card.
    public void AIAttack()
    {
        Player currentAttacker = gameManagement.GetCurrentAttacker();
        Card attackCard = AIChooseCardToAttack(currentAttacker);

        if (attackCard != null)
        {
            Debug.Log($"AI attacks with: {attackCard.rank} of {attackCard.suit}");
            gameManagement.HandleAttack(currentAttacker, attackCard);
        }
        else
        {
            Debug.Log("AI has no valid attack options.");
        }
    }

    // Returns the first card from the AI's hand that is playable.
    private Card AIChooseCardToAttack(Player currentAttacker)
    {
        return currentAttacker.hand.FirstOrDefault();
    }

    // Selects a card for the AI to use in defense based on the current attack.
    private Card AIChooseCardToDefend(Player currentDefender, List<Card> playArea)
    {
        Card attackingCard = playArea.LastOrDefault();
        if (attackingCard == null) return null;

        return currentDefender.hand
            .Where(card => card.suit == attackingCard.suit && card.RankValue > attackingCard.RankValue)
            .OrderBy(card => card.RankValue)
            .FirstOrDefault();
    }

    // AI defends using the selected card or picks up the cards if unable to defend.
    public void AI_Defend()
    {
        Player currentDefender = gameManagement.GetCurrentDefender();
        List<Card> playArea = gameManagement.GetPlayArea();
        Card defenseCard = AIChooseCardToDefend(currentDefender, playArea);

        if (defenseCard != null && gameRules.CanPlayerDefendWithCard(playArea.LastOrDefault(), defenseCard))
        {
            gameManagement.HandleDefense(currentDefender, defenseCard);
        }
        else
        {
            AIForfeitDefenseAndPickUpCards(currentDefender, playArea);
        }
    }

    // Handles the scenario where AI cannot defend, forcing it to pick up all cards in the play area.
    private void AIForfeitDefenseAndPickUpCards(Player currentDefender, List<Card> playArea)
    {
        currentDefender.hand.AddRange(playArea);
        playArea.Clear();
        uiManager.ClearPlayArea();
        Debug.Log("AI cannot defend and picks up the play area cards.");

        // Set flag to indicate defender picked up cards
        gameManagement.didDefenderPickUp = true;

        // Move to the next phase after picking up cards
        gameManagement.EndTurn();
    }

}
