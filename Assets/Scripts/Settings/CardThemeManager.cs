using UnityEngine;

public class CardThemeManager : MonoBehaviour
{
    private readonly string[] cardThemes = { "basic", "anime", "black","pixel","tough" };

    public void SetCardTheme(int cardThemeIndex)
    {
        if (cardThemeIndex >= 0 && cardThemeIndex < cardThemes.Length)
        {
            PlayerPrefs.SetInt("SelectedCardTheme", cardThemeIndex);
            PlayerPrefs.Save();
            Debug.Log("Card Theme set to: " + cardThemes[cardThemeIndex]);
        }
        else
        {
            Debug.LogError("Invalid Card Theme index: " + cardThemeIndex);
        }
    }

    public string GetCurrentThemePrefix()
    {
        int themeIndex = PlayerPrefs.GetInt("SelectedCardTheme", 0); 
        return cardThemes[themeIndex];
    }
}
