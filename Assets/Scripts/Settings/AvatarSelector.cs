using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSelector : MonoBehaviour
{
    public void SetAvatarIcon(int avatarIndex)
    {
        PlayerPrefs.SetInt("SelectedAvatar", avatarIndex);
        PlayerPrefs.Save();
        Debug.Log("Avatar set to: " + avatarIndex);

    }
}
