using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCustomizeMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method to return to Main Menu
    public void BackMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    // Method to start the game and load the Gameplay scene
    public void LoadGameplayScene()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void Load1PlayerScene()
    {
        SceneManager.LoadScene("1P Customization");
    }

    public void Load2PlayerScene()
    {
        SceneManager.LoadScene("2P Customization");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeAvatarScene()
    {
        SceneManager.LoadSceneAsync("Avatar Select");
    }
}
