using UnityEngine;
using TMPro;

public class GameOverUIManager : MonoBehaviour
{
    public TextMeshProUGUI gameOverText; 

    private void Start()
    {
        // Check and display the winner message if available
        if (gameOverText != null && !string.IsNullOrEmpty(GameWinner.WinnerMessage))
        {
            gameOverText.text = GameWinner.WinnerMessage; 
            GameWinner.ClearWinnerMessage(); 
        }
        else
        {
            Debug.LogError("GameOver TextMeshProUGUI component is missing or winner message is empty.");
        }
    }
}

