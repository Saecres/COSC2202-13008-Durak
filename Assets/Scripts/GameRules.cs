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
        if (!playArea.Any()) return true; // Allow any card to attack if the play area is empty.

        bool hasTrumpBeenPlayed = playArea.Any(c => c.IsTrump);
        if (hasTrumpBeenPlayed)
        {
            // If a trump card has been played, follow-up trumps must be higher.
            int highestTrumpRank = playArea.Where(c => c.IsTrump).Max(c => c.RankValue);
            return playArea.Any(playAreaCard => playAreaCard.rank == card.rank) ||
                   (card.IsTrump && card.RankValue > highestTrumpRank);
        }
        else
        {
            // If no trump has been played, any trump or matching rank can attack.
            return playArea.Any(playAreaCard => playAreaCard.rank == card.rank) || card.IsTrump;
        }
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
        if (!playArea.Any()) return false; // No follow-up if the play area is empty

        bool hasTrumpBeenPlayed = playArea.Any(c => c.IsTrump);
        if (hasTrumpBeenPlayed)
        {
            // If a trump card has been played, only higher trumps or matching ranks are allowed.
            int highestTrumpRank = playArea.Where(c => c.IsTrump).Max(c => c.RankValue);
            return attacker.hand.Any(card => playArea.Any(playAreaCard => playAreaCard.rank == card.rank) ||
                                             (card.IsTrump && card.RankValue > highestTrumpRank));
        }
        else
        {
            // If no trump has been played, any trump or matching rank can attack.
            return attacker.hand.Any(card => playArea.Any(playAreaCard => playAreaCard.rank == card.rank) || card.IsTrump);
        }
    }


    /// <summary>
    /// This method prompts the player to make a follow-up attack if possible
    /// It displays a UI prompt to the player to continue their attack with another card.
    /// </summary>
    public void PromptPlayerForFollowUpAttack()
    {
        Debug.Log("Player can make a follow-up attack. Select a card to play.");
        ChatLogController.Log("Player can make a follow-up attack. Select a card to play.");
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
