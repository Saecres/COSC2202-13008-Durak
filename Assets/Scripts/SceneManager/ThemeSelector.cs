using UnityEngine;
using UnityEngine.UI;

public class ThemeSelector : MonoBehaviour
{
    public void SetBackground(int imageIndex)
    {
        PlayerPrefs.SetInt("SelectedTheme", imageIndex);
        PlayerPrefs.Save(); 
        Debug.Log("Theme set to: " + imageIndex);
    }

}
