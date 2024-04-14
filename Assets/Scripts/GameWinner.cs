using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWinner : MonoBehaviour
{
    public static string WinnerMessage { get; private set; }  // Static variable to hold the winner message

    public static void DeclareWinner(string winnerName, bool isDurak)
    {
        if (isDurak)
        {
            WinnerMessage = $"{winnerName} is the Durak!";
        }
        else
        {
            WinnerMessage = $"Congratulations! {winnerName} has won the game.";
        }

        // Load the Game Over scene.
        SceneManager.LoadScene("Game Over");
    }

    public static void ClearWinnerMessage()
    {
        WinnerMessage = null;
    }
}
