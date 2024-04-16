using UnityEngine;

public class CardThemeManager : MonoBehaviour
{
    private readonly string[] themes = { "basic", "anime", "black","pixel","tough" };

    public void SetCardTheme(int themeIndex)
    {
        if (themeIndex >= 0 && themeIndex < themes.Length)
        {
            PlayerPrefs.SetInt("SelectedTheme", themeIndex);
            PlayerPrefs.Save();
            Debug.Log("Card Theme set to: " + themes[themeIndex]);
        }
        else
        {
            Debug.LogError("Invalid theme index: " + themeIndex);
        }
    }

    public string GetCurrentThemePrefix()
    {
        int themeIndex = PlayerPrefs.GetInt("SelectedTheme", 0); 
        return themes[themeIndex];
    }
}
