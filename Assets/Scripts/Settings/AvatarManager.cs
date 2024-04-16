using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarManager : MonoBehaviour
{
    public Sprite[] avatarIcons; 
    public Image avatarDisplay; 

    void Start()
    {
        ApplySavedAvatar();
    }

    void ApplySavedAvatar()
    {
        int selectedAvatar = PlayerPrefs.GetInt("SelectedAvatar", 0);  
        if (avatarIcons != null && avatarIcons.Length > selectedAvatar)
        {
            avatarDisplay.sprite = avatarIcons[selectedAvatar];
        }
        else
        {
            Debug.LogError("Selected avatar index out of range or avatar icons not set properly.");
        }
    }
}
