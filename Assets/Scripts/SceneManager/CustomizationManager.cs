using UnityEngine;
using TMPro;

public class CustomizationManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;

    public void SavePlayerName()
    {
        if (!string.IsNullOrWhiteSpace(nameInputField.text))
        {
            PlayerPrefs.SetString("PlayerName", nameInputField.text);
            PlayerPrefs.Save();  // Make sure to save the PlayerPrefs to make it persistent
            Debug.Log("Player name saved: " + nameInputField.text);
        }
    }
}
