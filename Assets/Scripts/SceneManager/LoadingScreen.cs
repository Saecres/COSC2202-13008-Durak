using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public GameObject Loading;
    public Image LoadingBarFill;

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    IEnumerator LoadSceneAsync(int sceneId) 
    {
        // Console debugging
        Debug.Log("Loading...");

        Loading.SetActive(true);

        // Start loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        // Waits for 18 second
        yield return new WaitForSeconds(10000.0f);

        while (!operation.isDone) 
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            LoadingBarFill.fillAmount = progressValue;

            yield return null;
        }
    }
}
