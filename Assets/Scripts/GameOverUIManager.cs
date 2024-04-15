using UnityEngine;
using UnityEngine.UI;

public class GameOverUIManager : MonoBehaviour
{
    public Text gameOverText;

    private void Start()
    {
        // Check and display the winner message if available
        if (gameOverText != null && !string.IsNullOrEmpty(GameWinner.WinnerMessage))
        {
            gameOverText.text = GameWinner.WinnerMessage;
            GameWinner.ClearWinnerMessage();  // Clear the static message after displaying
        }
        else
        {
            Debug.LogError("GameOver text component is missing or winner message is empty.");
        }
    }
}
