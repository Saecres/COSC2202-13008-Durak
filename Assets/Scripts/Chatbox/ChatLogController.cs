using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class ChatLogController : MonoBehaviour
{
    public static ChatLogController Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI chatLogText;  // Reference to the TextMeshPro UI component
    private static string logContent = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Log(string message)
    {
        logContent += message + "\n"; // Append new message
        if (Instance && Instance.chatLogText)
            Instance.chatLogText.text = logContent; // Update UI Text
    }
}
