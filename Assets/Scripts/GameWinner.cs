using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWinner : MonoBehaviour
{
    public static string WinnerMessage { get; private set; }  // Holds the Durak message

    public static void DeclareWinner(string durakName)
    {
        WinnerMessage = $"{durakName} is the Durak!";
        SceneManager.LoadScene("Game Over");  
    }

    public static void ClearWinnerMessage()
    {
        WinnerMessage = string.Empty;
    }
}
