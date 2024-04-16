using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarManager : MonoBehaviour
{
    public Sprite[] avatarIcons; // Array of avatar icons
    public Image avatarDisplay;  // references to where avatar should be shown

    void Start()
    {
        ApplySavedAvatar();
    }

    void ApplySavedAvatar()
    {
        int selectedTheme = PlayerPrefs.GetInt("SelectedTheme", 0);  // Default to 0 if nothing is set
        if (avatarIcons != null && avatarIcons.Length > selectedTheme)
        {
            avatarDisplay.sprite = avatarIcons[selectedTheme];
        }
        else
        {
            Debug.LogError("Selected avatar index out of range or avatar icon not set properly.");
        }
    }
}
