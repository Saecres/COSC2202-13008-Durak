using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AvatarMenu : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeAvatarScene()
    {
        SceneManager.LoadSceneAsync("Avatar Select");
    }
    
}
