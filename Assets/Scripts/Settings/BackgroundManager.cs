using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public Sprite[] backgroundImages;  
    public Image gameplayBackground;   

    void Start()
    {
        ApplySavedTheme();
    }

    void ApplySavedTheme()
    {
        int selectedTheme = PlayerPrefs.GetInt("SelectedTheme", 0);  // Default to 0 if nothing is set
        if (backgroundImages != null && backgroundImages.Length > selectedTheme)
        {
            gameplayBackground.sprite = backgroundImages[selectedTheme];
        }
        else
        {
            Debug.LogError("Selected theme index out of range or background images not set properly.");
        }
    }
}
