using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PageManager : MonoBehaviour
{
    private Stack<string> visitedScenes = new Stack<string>();

    // Call this method when the game starts to load the last visited page
    void Start()
    {
        // Save the initial scene when the game starts
        SaveCurrentScene();
    }

    // Save the current scene
    private void SaveCurrentScene()
    {
        visitedScenes.Push(SceneManager.GetActiveScene().name);
    }

    // Go back to the previous scene
    public void GoBack()
    {
        if (visitedScenes.Count > 1)
        {
            // Pop the current scene from the stack
            visitedScenes.Pop();

            // Load the previous scene
            string previousScene = visitedScenes.Peek();
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("No previous scene to go back to.");
        }
    }


}
