using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameRules : MonoBehaviour
{
    /// <summary>
    /// Determines if a card can initiate or participate in an attack.
    /// </summary>

    public bool CanPlayerAttackWithCard(Card card, List<Card> playArea)
    {
        // Allow any card to attack if the play area is empty.
        if (!playArea.Any()) return true;

        // Check for rank match to validate follow-up attacks.
        return playArea.Any(playAreaCard => playAreaCard.rank == card.rank);
    }

    /// <summary>
    /// Checks if a card can defend against an attacking card according to the game rules.
    /// </summary>

    public bool CanPlayerDefendWithCard(Card attackingCard, Card defendingCard)
    {
        if (attackingCard == null) return false;

        bool isTrumpAttack = attackingCard.IsTrump;
        bool isTrumpDefense = defendingCard.IsTrump;

        // Defense logic based on whether cards are trump or not.
        if (isTrumpAttack && !isTrumpDefense) return false;
        if (!isTrumpAttack && isTrumpDefense) return true;
        if (isTrumpAttack && isTrumpDefense)
        {
            return defendingCard.RankValue > attackingCard.RankValue;
        }

        // Non-trump cards must match suit and have a higher rank to defend successfully.
        return defendingCard.suit.Equals(attackingCard.suit, System.StringComparison.OrdinalIgnoreCase) &&
               defendingCard.RankValue > attackingCard.RankValue;
    }

    /// <summary>
    /// Determines if an attacker has any valid follow-up attack options.
    /// </summary>
    public bool CheckForFollowUpAttack(Player attacker, List<Card> playArea)
    {
        return attacker.hand.Any(card => playArea.Any(playAreaCard => playAreaCard.rank == card.rank));
    }

    // This method prompts the player to make a follow-up attack if possible.
    // It displays a UI prompt to the player to continue their attack with another card.
    public void PromptPlayerForFollowUpAttack()
    {
        Debug.Log("Player can make a follow-up attack. Select a card to play.");
        ChatLogController.Log("Player can make a follow-up attack. Select a card to play.");
        // Update the UI accordingly to prompt the player.
    }

    public bool CheckForDefense(Player defender, Card attackingCard)
    {
        return defender.hand.Any(card => CanPlayerDefendWithCard(attackingCard, card));
    }

    public void PromptPlayerForDefense(Player defender, Card attackingCard)
    {
        if (CheckForDefense(defender, attackingCard))
        {
            Debug.Log("Defense is possible. Player should choose a card to defend.");
            ChatLogController.Log("Defense is possible. Player should choose a card to defend.");
        }
        else
        {
            Debug.Log("No defense possible. Player must take other actions (e.g., picking up the play area cards).");
            ChatLogController.Log("No defense possible. Player must take other actions (e.g., picking up the play area cards).");
        }
    }



}
