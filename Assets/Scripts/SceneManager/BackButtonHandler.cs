using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButtonHandler : MonoBehaviour
{
    public void OnBackButtonClick()
    {
        // Find the SceneNavigator script in the scene
        PageManager pageManager = FindObjectOfType<PageManager>();

        // Call the GoBack method if the SceneNavigator is found
        if (pageManager != null)
        {
            pageManager.GoBack();
        }
        else
        {
            Debug.LogError("pageManager not found in the scene.");
        }
    }
}
